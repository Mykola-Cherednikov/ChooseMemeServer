namespace ChooseMemeServer
{
    public class Lobby
    {
        private int _id;

        private string _name;

        private Client host;

        private List<Client> _clients;

        private int _maxNumOfClients;

        private Game? game;

        private bool isHided;

        public Lobby(int id, string name, int maxNumOfClients, Client c)
        {
            _id = id;
            _name = name;
            host = c;
            _clients = new List<Client>();
            AddClient(c);
            _maxNumOfClients = maxNumOfClients;
        }

        public int getId()
        {
            return _id;
        }

        public string getName()
        {
            return _name;
        }

        public int getMaxNumOfClients()
        {
            return _maxNumOfClients;
        }

        public int getNumOfClients()
        {
            return _clients.Count;
        }

        public List<Client> getClients()
        {
            return _clients;
        }

        public void AddClient(Client c)
        {
            _clients.Add(c);
        }

        public void RemoveClient(Client c)
        {
            _clients.Remove(c);
        }

        public Client GetHost()
        {
            return host;
        }

        public void SetHost(Client c)
        {
            host = c;
        }

        public void SetGame(Game game)
        {
            this.game = game;
        }

        public Game? GetGame()
        {
            return game;
        }

        public bool getHided()
        {
            return isHided;
        }

        public void setHided(bool b)
        {
            isHided = b;
        }
    }
}
