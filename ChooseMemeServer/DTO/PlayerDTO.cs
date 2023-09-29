using Assets.Scripts.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChooseMemeServer.DTO
{
    [Serializable]
    public class PlayerDTO
    {
        public ClientDTO? clientDTO {  get; set; }

        public int points { get; set; }
    }
}
