using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BCare.Models.GA
{
    public class Individual : IComparable
    {
        private static readonly Random random = new Random();
        private static readonly object syncLock = new object();
        List<Genome> genomeList = new List<Genome>();
        public const int indivSize = 4;
        public float fitnessGrade = 0.0f;

        public Individual()
        {
            for(int i=0; i < indivSize; i++)
            {
                Genome gen = new Genome();
                genomeList.Add(gen);
            }
        }
        public int CompareTo(object a)
        {
            Individual Gene1 = this;
            Individual Gene2 = (Individual)a;
            return Math.Sign(Gene2.fitnessGrade - Gene1.fitnessGrade);
        }
        public Individual (Individual A, Individual B)
        {
            for (int i = 0; i < A.genomeList.Count / 2; i++)
            {
                genomeList.Add(A.genomeList[i]);
            }
            for(int i= B.genomeList.Count / 2; i < B.genomeList.Count; i++)
            {
                genomeList.Add(B.genomeList[i]);
            }
        }
        public void Mutate()
        {
            int MutationIndex = RandomNumber(0, indivSize);
            Genome newGenome = new Genome();
            genomeList[MutationIndex] = newGenome;
        }

        public void CalculateFitness()
        {
            int sum = 0;
            int avg = 0;
            for (int i = 0; i < genomeList.Count; i++)
            {
                sum = sum + genomeList[i].number;
            }
            fitnessGrade = sum / genomeList.Count;
            //if (avg == 5)
            //{
            //    fitnessGrade = 10;
            //} else
            //{
            //    fitnessGrade = 0;
            //}
        }

        public override string ToString()
        {
            String str="";
            for(int i=0; i < genomeList.Count; i++)
            {
                str = str + " " + genomeList[i].number;
            }
            return str;
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
