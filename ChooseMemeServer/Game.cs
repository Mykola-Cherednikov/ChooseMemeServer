using ChooseMemeServer.DTO;
using System.Text;

namespace ChooseMemeServer
{
    public enum GameStatus
    {
        StartingGame,
        ReadingTheQuestion,
        ChoseTheCard,
        AfterChosedCard,
        StartVoteForCard,
        VotingForCard,
        GivingAnotherCard,
        EndGame
    }

    public class Game
    {
        private Lobby lobby;

        private Stack<Card> deck;

        private List<Card> chosenCards;

        private Stack<int> questions;

        private List<Player> players;

        private static int NumOfCards = 60;

        private static int NumOfQuestions = 10;

        private static int NumOfCardsPerClient = 4;

        private static int NumOfTimeToChose = 60;

        private GameStatus gameStatus;

        public Game(Lobby lobby)
        {
            gameStatus = GameStatus.StartingGame;

            this.lobby = lobby;

            lobby.setHided(true);

            lobby.SetGame(this);

            Random rnd = new Random();

            chosenCards = new List<Card>();

            players = new List<Player>();

            foreach (var client in lobby.getClients())
            {
                Player p = new Player(client);
                players.Add(p);
            }

            List<int> questionsList = new List<int>();

            for (int i = 0; i < NumOfQuestions; i++) // questions creation
            {
                questionsList.Add(i);
            }

            for (int i = 0; i < questionsList.Count; i++) //Shuffle questions
            {
                int r = rnd.Next(0, NumOfQuestions);
                (questionsList[i], questionsList[r]) = (questionsList[r], questionsList[i]);
            }

            questions = new Stack<int>(questionsList);

            List<Card> cards = new List<Card>();

            for (int i = 0; i < NumOfCards; i++) // deck creation
            {
                Card card = new Card() { id = i };
                cards.Add(card);
            }

            for (int i = 0; i < cards.Count; i++) //Shuffle deck
            {
                int r = rnd.Next(0, NumOfCards);
                (cards[i], cards[r]) = (cards[r], cards[i]);
            }

            deck = new Stack<Card>(cards);


        }

        public async void StartGame()
        {
            try
            {
                #region StartGame

                foreach (var client in lobby.getClients()) // Send Started Game to all players
                {
                    string query = Queries.StartingGameQuery(client, lobby.getClients());

                    await client.GetTcpClient().GetStream().WriteAsync(Encoding.UTF8.GetBytes(query));
                }

                Thread.Sleep(3000);

                for (int i = 0; i < NumOfCardsPerClient; i++) // Send 4 cards to every player
                {
                    foreach (var player in players)
                    {
                        GiveCardToPlayerFromDeck(player, players, deck);

                        Thread.Sleep(300);
                    }
                }

                int numberOfRounds = questions.Count;

                #endregion

                for (int i = 0; i < numberOfRounds; i++) // Start game
                {
                    #region BeforeNewRound

                    foreach (var player in players)
                    {
                        player.ready = false;
                        player.voted = false;
                    }

                    chosenCards = new List<Card>();

                    #endregion

                    #region ReadingTheQuestion

                    gameStatus = GameStatus.ReadingTheQuestion;

                    int question = questions.Pop();

                    foreach (var player in players) //Send question
                    {
                        var clientReceiver = player.client;

                        string query = Queries.SendQuestionQuery(question);

                        await clientReceiver.GetTcpClient().GetStream().WriteAsync(Encoding.UTF8.GetBytes(query));
                    }

                    Thread.Sleep(5000);

                    #endregion

                    #region ChoseTheCard

                    gameStatus = GameStatus.ChoseTheCard;

                    foreach (var player in players) //Send query about need to choose card
                    {
                        var clientReceiver = player.client;

                        string query = MultiplayerEvents.ChoseTheCardGameEvent.ToString() + "\n";

                        await clientReceiver.GetTcpClient().GetStream().WriteAsync(Encoding.UTF8.GetBytes(query));
                    }


                    int time = NumOfTimeToChose + 1;
                    do
                    {
                        time -= 1;
                        Thread.Sleep(1000);
                    } while (time > 0 && !players.All(p => p.ready));

                    #endregion

                    #region AfterChosedCard

                    gameStatus = GameStatus.AfterChosedCard;

                    foreach (var player in players) //Send query about start vote
                    {
                        var clientReceiver = player.client;

                        string query = MultiplayerEvents.AfterChosedCardGameEvent.ToString() + "\n";

                        await clientReceiver.GetTcpClient().GetStream().WriteAsync(Encoding.UTF8.GetBytes(query));
                    }


                    foreach (var player in players) // Remove card which chosed
                    {
                        Card? chosenCard = chosenCards.FirstOrDefault(c => c.owner == player);

                        if (chosenCard == null) // If player didn't chose card
                        {
                            Random rnd = new Random();

                            chosenCard = player.hand[rnd.Next(0, 4)];

                            chosenCards.Add(chosenCard); // Chose random card
                        }

                        RemoveCardFromPlayer(player, players, chosenCard);
                    }

                    #endregion

                    #region StartVoteForCard

                    gameStatus = GameStatus.StartVoteForCard;

                    foreach (var player in players) //Send query about start vote
                    {
                        var clientReceiver = player.client;

                        string query = Queries.StartVoteForCardQuery(chosenCards, clientReceiver);

                        await clientReceiver.GetTcpClient().GetStream().WriteAsync(Encoding.UTF8.GetBytes(query));
                    }

                    Thread.Sleep(60000);

                    #endregion

                    #region VotingForCard

                    gameStatus = GameStatus.VotingForCard;

                    foreach (var player in players) //Send query about start vote
                    {
                        var clientReceiver = player.client;

                        string query = MultiplayerEvents.VotingForCardGameEvent.ToString() + "\n";

                        await clientReceiver.GetTcpClient().GetStream().WriteAsync(Encoding.UTF8.GetBytes(query));
                    }

                    Thread.Sleep(15000);

                    #endregion

                    #region AfterVotingForCard

                    foreach (var player in players)
                    {
                        if (player.votedCard != null)
                        {
                            player.votedCard.owner!.points++;
                        }

                        player.votedCard = null;
                    }

                    #endregion

                    #region GivingAnotherCard

                    gameStatus = GameStatus.GivingAnotherCard;

                    foreach (var player in players) //Send query about start vote
                    {
                        var clientReceiver = player.client;

                        string query = MultiplayerEvents.GivingAnotherCardGameEvent.ToString() + "\n";

                        await clientReceiver.GetTcpClient().GetStream().WriteAsync(Encoding.UTF8.GetBytes(query));
                    }


                    foreach (var player in players)
                    {
                        GiveCardToPlayerFromDeck(player, players, deck);

                        Thread.Sleep(300);
                    }

                    #endregion
                }
            }
            catch (Exception)
            {

            }

            #region EndGame

            gameStatus = GameStatus.EndGame;

            foreach (var player in players)
            {
                var clientReceiver = player.client;

                string query = Queries.SendEndGameQuery(players);

                await clientReceiver.GetTcpClient().GetStream().WriteAsync(Encoding.UTF8.GetBytes(query));
            }

            #endregion
        }

        private async void GiveCardToPlayerFromDeck(Player p, List<Player> pls, Stack<Card> d)
        {
            try
            {
                var card = d.Pop();

                card.points = 0;
                card.owner = p;

                p.hand.Add(card);

                string query = Queries.SendCardQuery(card, p.client);

                await p.client.GetTcpClient().GetStream().WriteAsync(Encoding.UTF8.GetBytes(query));

                /*foreach (var player in pls)
                {
                    if (p != player)
                    {
                        string q = Queries.SendCardQuery(new Card() { id = -1 }, p.client);

                        await player.client.GetTcpClient().GetStream().WriteAsync(Encoding.UTF8.GetBytes(q));
                    }
                }*/
            }
            catch (Exception)
            {

            }
        }

        private async void RemoveCardFromPlayer(Player p, List<Player> pls, Card c)
        {
            try
            {
                string query = Queries.RemoveCardQuery(c, p.client);

                await p.client.GetTcpClient().GetStream().WriteAsync(Encoding.UTF8.GetBytes(query)); // Remove card from player

                p.hand.Remove(c);

                /*foreach (var player in pls)
                {
                    if (p != player)
                    {
                        query = Queries.RemoveCardQuery(new Card() { id = -1 }, p.client);

                        await player.client.GetTcpClient().GetStream().WriteAsync(Encoding.UTF8.GetBytes(query));
                    }
                }*/
            }
            catch (Exception)
            {

            }
        }

        private void StartedGame()
        {

        }



        public void ClientChooseCard(Client client, CardDTO cardDTO)
        {
            if (gameStatus == GameStatus.ChoseTheCard)
            {
                Player? p = players.FirstOrDefault(x => x.client == client);
                Card? card = p?.hand.FirstOrDefault(f => f.id == cardDTO.id);

                if (card == null)
                {
                    return;
                }

                Card? previousCard = chosenCards.FirstOrDefault(c => c.owner == p);

                if (previousCard != null)
                {
                    chosenCards.Remove(previousCard);
                }

                chosenCards.Add(card);
            }
        }

        public void ClientVoteForCard(Client client, CardDTO cardDTO)
        {
            Player? player = players.FirstOrDefault(p => p.client == client);
            Card? card = chosenCards.FirstOrDefault(c => c.id == cardDTO.id);

            if (gameStatus != GameStatus.VotingForCard || player == null || card == null || card.owner == player)
            {
                return;
            }

            player.voted = true;
            player.votedCard = card;

        }

        public void DisconnectFromGame(Client c)
        {
            players.Remove(players.FirstOrDefault(p => p.client == c)!);
        }
    }
}
