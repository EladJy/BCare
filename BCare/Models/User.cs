using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace BCare.Models
{
    public class User
    {
        [Key]
        //primary key
        [Range(000100000, 999999999)]
        public int UserID { get; set; }
        //foreign key
        public int HMOID { get; set; }
        [StringLength(50, MinimumLength = 2)]
        public string FirstName { get; set; }
        [StringLength(50, MinimumLength = 2)]
        public string LastName { get; set; }
        //enum F\M
        public Gender Gender { get; set; }
        [DataType(DataType.Date)]
        public DateTime BirthDate { get; set; }
        public BloodType BloodType { get; set; }
        public string Address { get; set; }
        public PremissionName PremissionType { get; set; }
        public string UserName { get; set; }
        //password
        public string PWHash { get; set; }
        public string Email { get; set; }
    }

    public enum PremissionName
    {
        Anonym,
        User,
        Doctor,
        Developer,
        Admin

    };
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
        Ominus,
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