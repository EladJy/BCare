using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BCare.Models
{
    public class review_or_feedback
    {


        //primary key
        public int RFUserID { get; set; }
        //foreign key //primary key
        public int RFSomID { get; set; }
        //foreign key
        public int RFPresID { get; set; }
        public DateTime ReviewDate { get; set; }

        public Rating Rating { get; set; }
        public string Text { get; set; }
    }

    public enum Rating
    {
        a = 1,
        b = 2,
        c = 3,
        d = 4,
        e = 5,
        f = 6,
        g = 7,
        h = 8,
        i = 9,
        j = 10

    };


}
