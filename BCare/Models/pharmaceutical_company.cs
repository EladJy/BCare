using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BCare.Models
{
    public class pharmaceutical_company
    {

        //primary key//foreign key
        public int PharmID { get; set; }
        public string PharmName { get; set; }
        public string Origin { get; set; }
    }
}
