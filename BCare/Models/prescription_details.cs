using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BCare.Models
{
    public class prescription_details
    {

        //primary key
        public int PDPresID { get; set; }
        //primary key//foreign key
        public int PDSom_ID { get; set; }
        public int AmountToConsume { get; set; }
        public int DaysToConsume { get; set; }
        public string Text { get; set; }
    }
}
