using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BCare.Models
{
    public class prescription
    {
        //primary key//foreign key
        public int PresID { get; set; }
        public int PBTestID { get; set; }
        public DateTime PresDate { get; set; }
    }
}
