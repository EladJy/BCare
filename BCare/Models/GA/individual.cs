using BCare.data;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BCare.Models.GA
{
    public class Individual : IComparable
    {
        static BcareContext contextIndv;
        static int bloodTestID;
        static Dictionary<int, double> avgDic = null;
        static List<active_component_effect_in_med> aceList = null;
        private static readonly Random random = new Random();
        private static readonly object syncLock = new object();
        public List<Genome> genomeList = new List<Genome>();
        static BloodTestViewModel BTVM;
        public const int indivSize = 6;
        public double fitnessGrade = 100.0;
        public const int EXECPTION = 50;
        public const int PRICE = 10;
        public const int MEDICAL_PRESCRIPTION = 5;
        public const int IN_HEALTH_PLAN = 5;
        public const int NUM_OF_MEDICATIONS = 30;
        public bool noExecptions = false;
        public string text = "";
        public HashSet<int> hashMed;

        public Individual(int btID, BcareContext context)
        {
            bloodTestID = btID;
            contextIndv = context;
            for (int i = 0; i < indivSize; i++)
            {
                Genome gen = new Genome(context);
                genomeList.Add(gen);
            }
        }
        public int CompareTo(object a)
        {
            Individual Gene1 = this;
            Individual Gene2 = (Individual)a;
            return Math.Sign(Gene2.fitnessGrade - Gene1.fitnessGrade);
        }
        public Individual(Individual A, Individual B)
        {
            for (int i = 0; i < A.genomeList.Count / 2; i++)
            {
                genomeList.Add(A.genomeList[i]);
            }
            for (int i = B.genomeList.Count / 2; i < B.genomeList.Count; i++)
            {
                genomeList.Add(B.genomeList[i]);
            }
        }
        public void Mutate()
        {
            int MutationIndex = RandomNumber(0, indivSize);
            Genome newGenome = new Genome(contextIndv);
            genomeList[MutationIndex] = newGenome;
        }
        public void CalculateFitness()
        {
            fitnessGrade = 100;
            if (BTVM == null)
            {
                BTVM = new BloodTestViewModel();
                BTVM = contextIndv.GetTestResultByID(bloodTestID);
                avgDic = contextIndv.GetAvgRating();
                aceList = contextIndv.getAllEffects();
            }
            double amountPerComp = EXECPTION / (double)BTVM.BTC.Count;
            double amountPerMed = NUM_OF_MEDICATIONS / (double)(indivSize-1);
            double amountOnPres = MEDICAL_PRESCRIPTION / (double)genomeList.Count;
            double amountOnPlan = IN_HEALTH_PLAN / (double)genomeList.Count;
            int countGoods = 0;
            for (int i = 0; i < BTVM.BTC.Count; i++)
            {
                double min = 0;
                double max = 0;
                double value = 0;
                if (BTVM.UserGender.Equals("M"))
                {
                    min = BTVM.BTC[i].BOAComp.MenMin;
                    max = BTVM.BTC[i].BOAComp.MenMax;
                }
                else if (BTVM.UserGender.Equals("F"))
                {
                    if (BTVM.IsPregnant.Equals("N"))
                    {
                        min = BTVM.BTC[i].BOAComp.WomenMin;
                        max = BTVM.BTC[i].BOAComp.WomenMax;
                    }
                    else
                    {
                        min = BTVM.BTC[i].BOAComp.PregnantMin;
                        max = BTVM.BTC[i].BOAComp.PregnantMax;
                    }
                }
                value = BTVM.BTC[i].btData.Value;
                if(value > min && value < max)
                {
                    countGoods++;
                }
                if(value < min)
                {
                    text = text + "<li>מחסור ב " + BTVM.BTC[i].BOAComp.BOA_Name + "</li>";
                } else if(value > min)
                {
                    text = text + "<li>עודף ב " + BTVM.BTC[i].BOAComp.BOA_Name + "</li>";
                }
                hashMed = new HashSet<int>();
                for (int j = 0; j < genomeList.Count; j++)
                {
                    if (!hashMed.Contains(genomeList[j].med.SomID))
                    {
                        for (int k = 0; k < aceList.Count; k++)
                        {
                            if (aceList[k].ACEM_BOA_ID == BTVM.BTC[i].BOAComp.BOA_ID && aceList[k].ACEM_SOM_ID == genomeList[j].med.SomID)
                            {
                                value = value + aceList[k].Effect * ((max - min) / 100);
                            }
                            //value = BTVM.BTC[i].btData.Value + contextIndv.GetEffectOnComp(BTVM.BTC[i].BOAComp.BOA_ID, genomeList[j].med.SomID) * ((max - min) / 100);
                        }
                        hashMed.Add(genomeList[j].med.SomID);
                    }
                    if (genomeList[j].med.InHealthPlan.ToString().Equals("N"))
                    {
                        fitnessGrade = fitnessGrade - amountOnPlan;
                    }
                    if(genomeList[j].med.WithMedicalPrescription.ToString().Equals("Y"))
                    {
                        fitnessGrade = fitnessGrade - amountOnPres;
                    }
                    double avgRating = 5;
                    avgDic.TryGetValue(genomeList[j].med.SomID,out double avgRate);
                    if(avgRate != 0)
                    {
                        avgRating = avgRate;
                    }
                    fitnessGrade = fitnessGrade - (PRICE - avgRating * 2);
                    
                }
                if (value < 0)
                    value = 0.000000000001;
                double avg = (min + max) / 2;
                if (value < min)
                {
                    fitnessGrade = fitnessGrade - ((1 - (value / avg)) * amountPerComp);
                }
                else if (value > max)
                {
                    fitnessGrade = fitnessGrade - ((1 - (avg / value)) * amountPerComp);
                }
                fitnessGrade = fitnessGrade - amountPerMed * (hashMed.Count-1); // 30% of fitness
            }
            if(countGoods == BTVM.BTC.Count)
            {
                noExecptions = true;
                text = "כל הבדיקות יצאו תקינות!";
            }

        }

        public override string ToString()
        {
            String str = "";
            for (int i = 0; i < genomeList.Count; i++)
            {
                str = str + " " + genomeList[i].med.SomID;
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

        public int getIndvSize()
        {
            return indivSize;
        }
    }
}
