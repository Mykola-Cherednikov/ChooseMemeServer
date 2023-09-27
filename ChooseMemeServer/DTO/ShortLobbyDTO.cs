using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChooseMemeServer.DTO
{
    [Serializable]
    public class ShortLobbyDTO
    {
        public int id { get; set; }

        public string? name { get; set; }

        public int numOfClients { get; set; }

        public int maxNumOfClients { get; set; }
    }
}
