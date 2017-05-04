using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace BCare.Models
{
    public class User
    {
        [Key]
        //primary key
        public int UserID { get; set; }
        //foreign key
        public int HMOID { get; set; }
        public string FirstName { get; set; }

        public string LastName { get; set; }
        //enum F\M
        public Gender Gender { get; set; }
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:MM/dd/yyyy}")]
        public DateTime BirthDate { get; set; }
        public BloodType BloodType { get; set; }

        public string Address { get; set; }


    }
    public enum Gender
    {
        M,
        F
    }




    public enum BloodType
    {
        [Display(Name = "O+")]
        Oplus,
        [Display(Name = "O-")]
        Pminus,
        [Display(Name = "A+")]
        Aplus,
        [Display(Name = "A-")]
        Aminus,
        [Display(Name = "B+")]
        Bplus,
        [Display(Name = "B-")]
        Bminus,
        [Display(Name = "AB+")]
        ABplus,
        [Display(Name = "AB-")]
        ABminus,
        [Display(Name = "HH")]
        HH,
        [Display(Name = "Unknown")]
        Unknown
    }

}
