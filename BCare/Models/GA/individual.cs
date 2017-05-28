using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BCare.Models.GA
{
    public class Individual
    {
        List<Genome> genomeList = new List<Genome>();
        public const int indivSize = 5;
        public float fitnessGrade = 0.0f;

        public Individual()
        {
            for(int i=0; i < indivSize; i++)
            {
                Genome gen = new Genome();
                genomeList.Add(gen);
            }
        }

        public Individual (Individual A, Individual B)
        {
            for (int i = 0; i < A.genomeList.Count / 2; i++)
            {
                genomeList.Add(A.genomeList[i]);
            }
            for(int i= B.genomeList.Count / 2; i < B.genomeList.Count; i++)
            {
                genomeList.Add(A.genomeList[i]);
            }
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
    }
}
