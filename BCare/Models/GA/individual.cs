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
        public double fitnessGrade = 0.0;

        public Individual()
        {
            for(int i=0; i < indivSize; i++)
            {
                Genome gen = new Genome();
                genomeList.Add(gen);
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
