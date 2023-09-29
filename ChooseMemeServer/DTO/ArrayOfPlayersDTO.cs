using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChooseMemeServer.DTO
{
    [Serializable]
    public class ArrayOfPlayersDTO
    {
        public PlayerDTO[]? playersDTO { get; set; }
    }
}
