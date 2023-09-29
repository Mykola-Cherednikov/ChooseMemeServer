using Assets.Scripts.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChooseMemeServer.DTO
{
    [Serializable]
    public class StartGameDTO
    {
        public ClientDTO? you { get; set; }

        public ArrayOfClientsDTO? clientsDTO { get; set; }
    }
}
