using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.Scripts.DTO
{
    [Serializable]
    public class ClientDTO
    {
        public int id { get; set; }

        public string? name { get; set; }
    }
}
