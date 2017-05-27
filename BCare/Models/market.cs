using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BCare.Models
{
    public class market
    {
        //primary key//foreign key
        public int MarketID { get; set; }
        //foreign key
        public int MSomID { get; set; }
        public double Price { get; set; }
    }
}
