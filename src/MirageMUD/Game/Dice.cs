using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Mirage.Game
{
    public class Dice
    {
        private static Random _rand = new Random();

        private static Dice _default = new Dice();

        public static Dice Default
        {
            get
            {
                return _default;
            }
        }

        public int Roll(int min, int max)
        {
            return _rand.Next(min, max + 1);
        }
    }
}
