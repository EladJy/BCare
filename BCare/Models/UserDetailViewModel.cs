using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace BCare.Models
{
    public class UserDetailViewModel
    {
        public User user { get; set; }
        [Display(Name = "דוקטור?")]
        public Boolean isDoctor { get; set; }

    }
}
