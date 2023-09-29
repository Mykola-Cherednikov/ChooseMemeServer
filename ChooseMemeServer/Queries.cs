using Assets.Scripts.DTO;
using ChooseMemeServer;
using ChooseMemeServer.DTO;
using System.Text.Json;

public class Queries
{
    public static string ReturnLobbiesQuery(List<Lobby> lobbies)
    {
        List<ShortLobbyDTO> shortLobbiesDTO = new List<ShortLobbyDTO>();

        foreach (var lobby in lobbies)
        {
            ShortLobbyDTO shortLobbyDTO = new ShortLobbyDTO();
            shortLobbyDTO.id = lobby.getId();
            shortLobbyDTO.name = lobby.getName();
            shortLobbyDTO.numOfClients = lobby.getNumOfClients();
            shortLobbyDTO.maxNumOfClients = lobby.getMaxNumOfClients();
            shortLobbiesDTO.Add(shortLobbyDTO);
        }
        ArrayOfShortLobbiesDTO arrayOfShortLobbiesDTO = new ArrayOfShortLobbiesDTO();
        arrayOfShortLobbiesDTO.shortLobbiesDTO = shortLobbiesDTO.ToArray();

        return MultiplayerEvents.ReturnLobbies.ToString() + "\n" + JsonSerializer.Serialize(arrayOfShortLobbiesDTO) + "\n";
    }

    public static string ReturnLobbyQuery(Lobby lobby, Client c)
    {
        LobbyDTO lobbyDTO = new LobbyDTO();
        lobbyDTO.id = lobby.getId();
        lobbyDTO.name = lobby.getName();
        lobbyDTO.numOfClients = lobby.getNumOfClients();
        lobbyDTO.maxNumOfClients = lobby.getMaxNumOfClients();
        lobbyDTO.isHost = (c == lobby.GetHost());

        List<ClientDTO> clientsDTO = new List<ClientDTO>();
        foreach (var client in lobby.getClients())
        {
            ClientDTO clientDTO = new ClientDTO();
            clientDTO.id = client.getId();
            clientDTO.name = client.getNickname();
            clientsDTO.Add(clientDTO);
        }
        lobbyDTO.clientsDTO = new ArrayOfClientsDTO();
        lobbyDTO.clientsDTO.clientsDTO = clientsDTO.ToArray();

        return MultiplayerEvents.ReturnLobby.ToString() + "\n" + JsonSerializer.Serialize(lobbyDTO) + "\n";
    }

    public static string ClientConnectedToLobbyQuery(Client client)
    {
        ClientDTO clientDTO = new ClientDTO();
        clientDTO.id = client.getId();
        clientDTO.name = client.getNickname();

        return MultiplayerEvents.ClientConnectedToLobby.ToString() + "\n" + JsonSerializer.Serialize(clientDTO) + "\n";
    }

    public static string ClientDisconnectedFromLobbyQuery(Client client)
    {
        ClientDTO clientDTO = new ClientDTO();
        clientDTO.id = client.getId();
        clientDTO.name = client.getNickname();

        return MultiplayerEvents.ClientDisconnectedFromLobby.ToString() + "\n" + JsonSerializer.Serialize(clientDTO) + "\n";
    }

    public static string StartingGameQuery(Client client, List<Client> clients)
    {
        ClientDTO youDTO = new ClientDTO();
        youDTO.id = client.getId();
        youDTO.name = client.getNickname();

        List<ClientDTO> clientsDTO = new List<ClientDTO>();
        foreach(var c in clients)
        {
            clientsDTO.Add(new ClientDTO() { id = c.getId(), name = c.getNickname() });
        }

        ArrayOfClientsDTO arrayOfClientsDTO = new ArrayOfClientsDTO();
        arrayOfClientsDTO.clientsDTO = clientsDTO.ToArray();

        StartGameDTO startGameDTO = new StartGameDTO();
        startGameDTO.you = youDTO;
        startGameDTO.clientsDTO = arrayOfClientsDTO;

        return MultiplayerEvents.StartingGameGameEvent.ToString() + "\n" + JsonSerializer.Serialize(startGameDTO) + "\n";
    }

    public static string SendCardQuery(Card card, Client client)
    {
        ClientDTO clientDTO = new ClientDTO();
        clientDTO.id = client.getId();
        clientDTO.name = client.getNickname();

        CardDTO sendCardDTO = new CardDTO();
        sendCardDTO.owner = clientDTO;
        sendCardDTO.id = card.id;

        return MultiplayerEvents.AddCardGameEvent.ToString() + "\n" + JsonSerializer.Serialize(sendCardDTO) + "\n";
    }

    public static string SendQuestionQuery(int id)
    {
        QuestionDTO questionDTO = new QuestionDTO();
        questionDTO.id = id;

        return MultiplayerEvents.ReadingTheQuestionGameEvent.ToString() + "\n" + JsonSerializer.Serialize(questionDTO) + "\n";
    }

    public static string RemoveCardQuery(Card card, Client client)
    {
        ClientDTO clientDTO = new ClientDTO();
        clientDTO.id = client.getId();
        clientDTO.name = client.getNickname();

        CardDTO sendCardDTO = new CardDTO();
        sendCardDTO.owner = clientDTO;
        sendCardDTO.id = card.id;

        return MultiplayerEvents.RemoveCardGameEvent.ToString() + "\n" + JsonSerializer.Serialize(sendCardDTO) + "\n";
    }

    public static string StartVoteForCardQuery(List<Card> cards, Client client)
    {
        List<CardDTO> cardsDTO = new List<CardDTO>();
        
        foreach (Card card in cards)
        {
            CardDTO cardDTO = new CardDTO();
            cardDTO.id = card.id;
            if (client == card.owner?.client)
            {
                ClientDTO clientDTO = new ClientDTO();
                clientDTO.id = client.getId();
                clientDTO.name = client.getNickname();

                cardDTO.owner = clientDTO;
            }
            cardsDTO.Add(cardDTO);
        }

        ArrayOfCardsDTO arrayOfCardsDTO = new ArrayOfCardsDTO();
        arrayOfCardsDTO.cards = cardsDTO.ToArray();

        return MultiplayerEvents.StartVoteForCardGameEvent.ToString() + "\n" + JsonSerializer.Serialize(arrayOfCardsDTO) + "\n";
    }

    public static string SendEndGameQuery(List<Player> players)
    {
        List<PlayerDTO> playersDTO = new List<PlayerDTO>();

        foreach (Player player in players)
        {
            PlayerDTO playerDTO = new PlayerDTO();

            ClientDTO clientDTO = new ClientDTO();
            clientDTO.id = player.client.getId();
            clientDTO.name = player.client.getNickname();

            playerDTO.clientDTO = clientDTO;
            playerDTO.points = player.points;

            playersDTO.Add(playerDTO);
        }

        ArrayOfPlayersDTO arrayOfPlayersDTO = new ArrayOfPlayersDTO();
        arrayOfPlayersDTO.playersDTO = playersDTO.ToArray();


        return MultiplayerEvents.EndGameGameEvent.ToString() + "\n" + JsonSerializer.Serialize(arrayOfPlayersDTO) + "\n";
    }
}
