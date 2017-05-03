using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BCare.Models
{
    public class supplements_data
    {
        //primary key
        public int AdtvID { get; set; }
        //foreign key//primary key
        public int SomID { get; set; }
        public int MyProperty { get; set; }
        public Double Value { get; set; }
        public string MeasurementUnit { get; set; }

    }
}
