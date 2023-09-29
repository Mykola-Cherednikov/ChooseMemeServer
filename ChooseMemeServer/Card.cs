using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChooseMemeServer
{
    [Serializable]
    public class Card
    {
        public int id { get; set; }

        public Player? owner { get; set; }

        public int points { get; set; }
    }
}
