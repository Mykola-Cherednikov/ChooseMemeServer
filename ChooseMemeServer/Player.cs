namespace ChooseMemeServer
{
    public class Player
    {
        public Client client;

        public List<Card> hand;

        public Card? votedCard;

        public int points;

        public bool ready;

        public bool voted;

        public Player(Client client)
        {
            this.client = client;
            hand = new List<Card>();
            points = 0;
        }
    }
}
