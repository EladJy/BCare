using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BCare.Models.GA
{
    public class Population
    {
        private static readonly Random random = new Random();
        public List<Individual> arrIndiv = new List<Individual>();
        public List<Individual> nextArrIndiv = new List<Individual>();
        const int populationSize = 100;
        int Generation = 1;
        double fitnessSum;
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
            DoBabies(arrIndiv);
            arrIndiv = nextArrIndiv.ToList();

            for(int i = 0; i <arrIndiv.Count;i++)
            {
                // Mutate(arrIndiv[i]); // Mutation
            }

            for (int i = 0; i < arrIndiv.Count; i++)
            {
                // arrIndiv[i].CalculateFitness();
            }
        }

        public void DoBabies(List<Individual> indivList)
        {
            List<Individual> geneDads = new List<Individual>();
            List<Individual> geneMoms = new List<Individual>();

            for (int i = 0; i < indivList.Count; i++)
            {
                if(i % 2 == 0)
                {
                    geneDads.Add(indivList[i]);
                } else
                {
                    geneMoms.Add(indivList[i]);
                }
            }

            for(int i=0; i < indivList.Count /2; i++)
            {
                Individual geneBabyA = new Individual(geneDads[i], geneMoms[i]);
                Individual geneBabyB = new Individual(geneMoms[i], geneDads[i]);
                nextArrIndiv.Add(geneBabyA);
                nextArrIndiv.Add(geneBabyB);
            }
        }
    }
}
