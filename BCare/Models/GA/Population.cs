using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BCare.Models.GA
{
    public class Population
    {
        public List<Individual> arrIndiv = new List<Individual>();
        const int populationSize = 150;
        int Generation = 1;
        public Population()
        {
            for(int i=0; i < populationSize; i++)
            {
                Individual arrGenome = new Individual();
                arrIndiv.Add(arrGenome);
                System.Diagnostics.Debug.WriteLine(arrGenome);
            }
        }

        public void NextGeneration()
        {
            Generation++;



        }
    }
}
