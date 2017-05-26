using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BCare.Models.GA
{
    public class Genome
    {
        private static readonly Random random = new Random();
        private static readonly object syncLock = new object();
        public int number;
        public Genome()
        {
            number = RandomNumber(0, 10);
        }

        public static int RandomNumber(int min, int max)
        {
            lock (syncLock)
            { // synchronize
                return random.Next(min, max);
            }
        }
    }

}
