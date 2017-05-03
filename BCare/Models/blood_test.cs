using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BCare.Models
{
    public class blood_test
    {
        //primary key//foreign key
        public int BTestID { get; set; }
        //foreign key
        public int BUserID { get; set; }
        public DateTime BTestDate { get; set; }
    }
}
