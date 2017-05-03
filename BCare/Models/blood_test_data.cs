using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BCare.Models
{
    public class blood_test_data
    {
        //primary key//foreign key
        public int BTestID { get; set; }
        //primary key//foreign key
        public int BCompID { get; set; }
        public double Value { get; set; }

    }
}
