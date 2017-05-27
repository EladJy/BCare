using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BCare.Models
{
    public class presComment
    {
        public int Pres_ID { get; set; }
        public int Rating { get; set; }
        public string review_text { get; set; }
        public int Amount_To_Consume_Per_Day { get; set; }
        public int Days_To_Consume { get; set; }
        public string pres_text { get; set; }
        public int Recomender_ID { get; set; }
        public DateTime Pres_Date { get; set; }
        public DateTime Review_Date { get; set; }
        public int RFUser_ID { get; set; }
    }
}
