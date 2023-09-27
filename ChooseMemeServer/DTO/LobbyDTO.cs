using ChooseMemeServer.DTO;
using System;

namespace Assets.Scripts.DTO
{
    [Serializable]
    public class LobbyDTO
    {
        public int id { get; set; }

        public string? name { get; set; }

        public int numOfClients { get; set; }

        public ArrayOfClientsDTO? clientsDTO { get; set; }

        public int maxNumOfClients { get; set; }

        public bool isHost { get; set; }
    }
}
