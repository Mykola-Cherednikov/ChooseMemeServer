using Assets.Scripts.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChooseMemeServer.DTO
{
    [Serializable]
    public class CardDTO
    {
        public int id { get; set; }

        public ClientDTO? owner { get; set; }
    }
}
