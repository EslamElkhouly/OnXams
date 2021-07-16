using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using OnlineExamPlatform.Models;
namespace OnlineExamPlatform.ViewModels
{
    public class NewStudentViewModel
    {
        public StudentData StudentData { get; set; }
        //public UserAuthentication UserAuthentication { get; set; }

        [Display(Name = "Email Address")]
        [Required]
        [RegularExpression(@"^([a-zA-Z0-9_\-\.]+)@((\[[0-9]{1,3}" +
                           @"\.[0-9]{1,3}\.[0-9]{1,3}\.)|(([a-zA-Z0-9\-]+\" +
                           @".)+))([a-zA-Z]{2,4}|[0-9]{1,3})(\]?)$",
            ErrorMessage = "Email is not valid")]
        [EmailExistsOrNotForStudent]
        [Remote("CheckIfEmailAddressExistsOrNot", "AdminAddStudent", ErrorMessage = "Email already exists .", AdditionalFields = "userEmailAddress")]
        public string EmailAddress { get; set; }

        [PasswordPropertyText]
        [Required]
        public string Password { get; set; }

        public IEnumerable<Gender> Gender { get; set; }
        public IEnumerable<Department> Department { get; set; }
        public IEnumerable<GradeYear> GradeYear { get; set; }


        public bool IsStudentPassed { get; set; }
    }
}