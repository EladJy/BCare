using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace BCare.Models
{
    public class presCommentViewModel
    {
        [DataType(DataType.Date)]
        public DateTime bloodTest_Date { get; set; }
        public List<SOMConsumeViewModel> somcList { get; set; }
        public List<review_or_feedback_ViewModel> rofvmList { get; set; }
    }
    public class SOMConsumeViewModel
    {
        public supplements_or_medication_info SOMI { get; set; }
        public prescription_details pres { get; set; }
    }

    public class review_or_feedback_ViewModel
    {
        public review_or_feedback rof { get; set; }
        public string first_Name { get; set; }
        public string last_Name { get; set; }
    }
}
