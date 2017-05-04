using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BCare.Models
{
    public class blood_or_additive_component
    {
        //primary key
        //foreign key
        public int BOA_ID { get; set; }
        public string BOA_Name { get; set; }
        public string MeasurementUnit { get; set; }
        public double MenMin { get; set; }
        public double MenMax { get; set; }
        public double WomenMin { get; set; }
      
        public double WomenMax { get; set; }
        public double PregnantMin { get; set; }

        public double PregnantMax { get; set; }
        public string info { get; set; }

    }
}
