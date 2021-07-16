
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using OnlineExamPlatform.Models;
using System.ComponentModel;
using System.Web.Mvc;


namespace OnlineExamPlatform.ViewModels
{
    public class NewAdminViewModel
    {
        public AdminData  AdminData{ get; set; }

        [Display(Name = "Email Address ")]
        [Required]
        [RegularExpression(@"^([a-zA-Z0-9_\-\.]+)@((\[[0-9]{1,3}" +
                           @"\.[0-9]{1,3}\.[0-9]{1,3}\.)|(([a-zA-Z0-9\-]+\" +
                           @".)+))([a-zA-Z]{2,4}|[0-9]{1,3})(\]?)$",
            ErrorMessage = "Email is not valid")]
        [EmailExistsOrNotForAdmin]
        [Remote("CheckIfEmailAddressExistsOrNot", "SuperAdmin", ErrorMessage = "Email already exists .", AdditionalFields = "userEmailAddress")]
        public string EmailAddress { get; set; }


        [PasswordPropertyText]
        [Required]
        public string Password { get; set; }
        public IEnumerable<Gender> Gender { get; set; }
    }
}