using BCare.data;
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
        private static readonly object syncLock = new object();
        public List<Individual> arrIndiv = new List<Individual>();
        public List<Individual> nextArrIndiv = new List<Individual>();
        public List<Individual> bestList = new List<Individual>();
        const int populationSize = 20;
        int generation = 1;
        public Population(int btID , BcareContext context)
        {
            for(int i=0; i < populationSize; i++)
            {
                Individual arrGenome = new Individual(btID , context);
                arrIndiv.Add(arrGenome);
                arrGenome.CalculateFitness();
            }
            arrIndiv.Sort();
        }

        public void NextGeneration()
        {
            generation++;
            DoBabies(arrIndiv);
            arrIndiv = nextArrIndiv.ToList();
            nextArrIndiv.Clear();
            for(int i = 0; i <arrIndiv.Count;i++)
            {
                Mutate(arrIndiv[i]); // Mutation
            }

            for (int i = 0; i < arrIndiv.Count; i++)
            {
                arrIndiv[i].CalculateFitness();
            }
            arrIndiv.Sort();
        }

        public void Mutate(Individual indiv)
        {
            if(RandomNumber(0,100) < 5)
            {
                indiv.Mutate();
            }
        }

        public static int RandomNumber(int min, int max)
        {
            lock (syncLock)
            { // synchronize
                return random.Next(min, max);
            }
        }
        public void DoBabies(List<Individual> indivList)
        {
            List<Individual> geneDads = new List<Individual>();
            List<Individual> geneMoms = new List<Individual>();

            for (int i = 0; i < indivList.Count; i++)
            {
                if(i < indivList.Count/2)
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
                //geneBabyA.CalculateFitness();
                //geneBabyB.CalculateFitness();
                nextArrIndiv.Add(geneBabyA);
                nextArrIndiv.Add(geneBabyB);
            }
        }

        public void WriteNextGeneration()
        {
            // just write the top 20
            System.Diagnostics.Debug.WriteLine("Generation {0}\n", generation);
            for (int i = 0; i < populationSize; i++)
            {
                System.Diagnostics.Debug.WriteLine(((Individual)arrIndiv[i]).ToString() + " fitness: " + ((Individual)arrIndiv[i]).fitnessGrade);
            }
        }
    }
}
