using BCare.data;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BCare.Models.GA
{
    public class Genome
    {
        BcareContext context;
        static List<supplements_or_medication_info> listMed = new List<supplements_or_medication_info>();
        private static readonly Random random = new Random();
        private static readonly object syncLock = new object();
        public supplements_or_medication_info med;
        public Genome(BcareContext context)
        {
            this.context = context;
            if(listMed.Count == 0)
            {
                listMed = context.GetAllMed();
            }
            med = listMed[RandomMedication(listMed.Count)];
        }
        public void setContext(BcareContext context)
        {
            this.context = context;
        }
        public static int RandomMedication(int number)
        {
            lock (syncLock)
            { // synchronize
                return random.Next(0,number);
            }
        }
    }

}
