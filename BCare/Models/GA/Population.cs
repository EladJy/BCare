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
        List<int> listOfIndexes = new List<int>();
        const int populationSize = 20;
        bool[] arrCheck = new bool[populationSize];
        int generation = 1;
        public Population(int btID , BcareContext context)
        {
            for(int i=0; i < populationSize; i++)
            {
                Individual arrGenome = new Individual(btID , context);
                arrIndiv.Add(arrGenome);
                arrGenome.CalculateFitness();
            }
            //arrIndiv.Sort();
            bestList.Add(arrIndiv[0]);
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
                arrIndiv[i].CalculateFitness();
            }

            //for (int i = 0; i < arrIndiv.Count; i++)
            //{
            //    arrIndiv[i].CalculateFitness();
            //}
            //arrIndiv.Sort();
            bestList.Add(arrIndiv[0]);
        }

        public void Mutate(Individual indiv)
        {
            if(RandomNumber(0,100) < 3)
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

        public int generateIfExist(int max)
        {
            int number = 0;
            do
            {
              number = RandomNumber(0, max);
            } while (arrCheck[listOfIndexes[number]]);
            arrCheck[listOfIndexes[number]] = true;
            return listOfIndexes[number];
        }
        public void DoBabies(List<Individual> indivList)
        {
            // 1. For on all indiv list
            // sum of fitness grade from all indiv
            // create array of sums 
            // 2. on half list random from 0 to sum
            // select with random number -->50babies
            // 3. the second half 

            //List<Individual> geneDads = new List<Individual>();
            //List<Individual> geneMoms = new List<Individual>();

            int sumOfIndiv = 0;
            for (int i = 0; i < indivList.Count; i++)
            {
                sumOfIndiv = sumOfIndiv + Convert.ToInt32(indivList[i].fitnessGrade);
                listOfIndexes.AddRange(Enumerable.Repeat(i, Convert.ToInt32(indivList[i].fitnessGrade)));
                //if(i < indivList.Count/2)
                //{
                //    geneDads.Add(indivList[i]);
                //} else
                //{
                //    geneMoms.Add(indivList[i]);
                //}
            }
            arrCheck = new bool[indivList.Count];
            for (int i = 0; i < indivList.Count / 2; i++)
            {
                int number = generateIfExist(sumOfIndiv);
                nextArrIndiv.Add(indivList[number]);

            }

            arrCheck = new bool[indivList.Count];
            for (int i = 0; i < indivList.Count / 2; i++)
            {
                int dad = generateIfExist(sumOfIndiv);
                int mom = generateIfExist(sumOfIndiv);
                Individual geneBaby = new Individual(indivList[dad], indivList[mom]);
                nextArrIndiv.Add(geneBaby);
            }

            //for (int i=0; i < indivList.Count /2; i++)
            //{
            //    Individual geneBabyA = new Individual(geneDads[i], geneMoms[i]);
            //    Individual geneBabyB = new Individual(geneMoms[i], geneDads[i]);
            //    //geneBabyA.CalculateFitness();
            //    //geneBabyB.CalculateFitness();
            //    nextArrIndiv.Add(geneBabyA);
            //    nextArrIndiv.Add(geneBabyB);
            //}
        }

        public void WriteNextGeneration()
        {
            bestList.Sort();
            // just write the top 20
            System.Diagnostics.Debug.WriteLine("Generation {0}\n", generation);
            for (int i = 0; i < bestList.Count; i++)
            {
                System.Diagnostics.Debug.WriteLine(((Individual)bestList[i]).ToString() + " fitness: " + ((Individual)bestList[i]).fitnessGrade);
            }
        }
    }
}
