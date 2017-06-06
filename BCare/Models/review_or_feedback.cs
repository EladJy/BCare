using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
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
        [Display(Name = "1")]
        a = 1,
        [Display(Name = "2")]
        b = 2,
        [Display(Name = "3")]
        c = 3,
        [Display(Name = "4")]
        d = 4,
        [Display(Name = "5")]
        e = 5,
    };
}
