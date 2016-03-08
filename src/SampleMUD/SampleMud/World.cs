using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SampleMud
{
    public class World
    {
        static World()
        {
            Players = new List<Player>();
        }
        public static List<Player> Players { get; set; }
    }
}
