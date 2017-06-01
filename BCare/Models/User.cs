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
        [Range(000100000, 999999999 , ErrorMessage = "עליך להזין תעודת זהות תקינה")]
        [Display(Name = "תעודת זהות")]
        [Required(ErrorMessage = "שדה חובה")]
        public int UserID { get; set; }
        //foreign key
        [Required(ErrorMessage = "שדה חובה")]
        [Display(Name = "קופת חולים")]
        public int HMOID { get; set; }
        [StringLength(50, MinimumLength = 2 , ErrorMessage = "מינימום 2 תווים")]
        [Display(Name = "שם פרטי")]
        public string FirstName { get; set; }
        [StringLength(50, MinimumLength = 2, ErrorMessage = "מינימום 2 תווים")]
        [Display(Name = "שם משפחה")]
        public string LastName { get; set; }
        //enum F\M
        [Display(Name = "מין")]
        public Gender Gender { get; set; }
        [DataType(DataType.Date)]
        [Required(ErrorMessage = "שדה חובה")]
        [Display(Name = "תאריך לידה")]
        public DateTime BirthDate { get; set; }
        [Display(Name = "סוג דם")]
        public BloodType BloodType { get; set; }
        [Required(ErrorMessage = "שדה חובה")]
        [Display(Name = "כתובת")]
        public string Address { get; set; }
        public PremissionName PremissionType { get; set; }
        [Required(ErrorMessage = "שדה חובה")]
        [Display(Name = "שם משתמש")]
        public string UserName { get; set; }
        //password
        [MinLength(6,ErrorMessage = "אנא הזן סיסמה באורך מינימילי של 6 תווים")]
        [Required(ErrorMessage = "שדה חובה")]
        [Display(Name = "סיסמה")]
        public string PWHash { get; set; }
        [EmailAddress(ErrorMessage = "אנא הזן כתובת מייל תקינה")]
        [Required(ErrorMessage = "שדה חובה")]
        [Display(Name = "כתובת מייל")]
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
        [Display(Name = "זכר")]
        M,
        [Display(Name = "נקבה")]
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