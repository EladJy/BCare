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
        private static readonly Random random = new Random();
        private static readonly object syncLock = new object();
        List<Genome> genomeList = new List<Genome>();
        static BloodTestViewModel BTVM;
        public const int indivSize = 6;
        public double fitnessGrade = 100.0;
        public const int EXECPTION = 50;
        public const int PRICE = 10;
        public const int MEDICAL_PRESCRIPTION = 5;
        public const int IN_HEALTH_PLAN = 5;
        public const int NUM_OF_MEDICATIONS = 30;

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

            if (BTVM == null)
            {
                BTVM = new BloodTestViewModel();
                BTVM = contextIndv.GetTestResultByID(bloodTestID);
            }
            double amountPerComp = EXECPTION / (double)BTVM.BTC.Count;
            double amountPerMed = NUM_OF_MEDICATIONS / (double)(indivSize-1);
            double amountOnPres = MEDICAL_PRESCRIPTION / (double)genomeList.Count;
            double amountOnPlan = IN_HEALTH_PLAN / (double)genomeList.Count;
            List<active_component_effect_in_med> aceList = contextIndv.getAllEffects();
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
                HashSet<int> hs = new HashSet<int>();
                for (int j = 0; j < genomeList.Count; j++)
                {
                    if (!hs.Contains(genomeList[j].med.SomID))
                    {
                        for (int k = 0; k < aceList.Count; k++)
                        {
                            if (aceList[k].ACEM_BOA_ID == BTVM.BTC[i].BOAComp.BOA_ID && aceList[k].ACEM_SOM_ID == genomeList[j].med.SomID)
                            {
                                value = value + aceList[k].Effect * ((max - min) / 100);
                            }
                            //value = BTVM.BTC[i].btData.Value + contextIndv.GetEffectOnComp(BTVM.BTC[i].BOAComp.BOA_ID, genomeList[j].med.SomID) * ((max - min) / 100);
                        }
                        hs.Add(genomeList[j].med.SomID);
                    }
                    if (genomeList[j].med.InHealthPlan.ToString().Equals("N"))
                    {
                        fitnessGrade = fitnessGrade - amountOnPlan;
                    }
                    if(genomeList[j].med.WithMedicalPrescription.ToString().Equals("N"))
                    {
                        fitnessGrade = fitnessGrade - amountOnPres;
                    }
                    double avgRating = contextIndv.GetAvgRatingBySOMID(genomeList[j].med.SomID);
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
                fitnessGrade = fitnessGrade - amountPerMed * (hs.Count-1); // 30% of fitness
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
    }
}
