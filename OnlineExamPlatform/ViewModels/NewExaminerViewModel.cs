using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using OnlineExamPlatform.Models;
using Gender = OnlineExamPlatform.Models.Gender;

namespace OnlineExamPlatform.ViewModels
{
    public class NewExaminerViewModel
    {
        public ExaminerData ExaminerData { get; set; }
        
        [Display(Name = "Email Address")]
        [Required]
        [RegularExpression(@"^([a-zA-Z0-9_\-\.]+)@((\[[0-9]{1,3}" +
                           @"\.[0-9]{1,3}\.[0-9]{1,3}\.)|(([a-zA-Z0-9\-]+\" +
                           @".)+))([a-zA-Z]{2,4}|[0-9]{1,3})(\]?)$",
            ErrorMessage = "Email is not valid")]
        [EmailExistsOrNotForExaminer]
        [Remote("CheckIfEmailAddressExistsOrNot", "AdminAddExaminer", ErrorMessage = "Email already exists .", AdditionalFields = "userEmailAddress")]
        public string EmailAddress { get; set; }


        [PasswordPropertyText]
        [Required]
        public string Password { get; set; }

        public IEnumerable<Gender> Gender { get; set; }
       
    }
}