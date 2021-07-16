using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using OnlineExamPlatform.Models;

namespace OnlineExamPlatform.ViewModels
{
    public class NewProctorViewModel
    {
        public ProctorData ProctorData { get; set; }

        [Required]
        [RegularExpression(@"^([a-zA-Z0-9_\-\.]+)@((\[[0-9]{1,3}" +
                           @"\.[0-9]{1,3}\.[0-9]{1,3}\.)|(([a-zA-Z0-9\-]+\" +
                           @".)+))([a-zA-Z]{2,4}|[0-9]{1,3})(\]?)$",
            ErrorMessage = "Email is not valid")]
        [EmailExistsOrNotForProctor]
        [Remote("CheckIfEmailAddressExistsOrNot", "AdminAddProctor", ErrorMessage = "Email already exists .", AdditionalFields = "userEmailAddress")]
        public string EmailAddress { get; set; }

        [PasswordPropertyText]
        [Required]
        public string Password { get; set; }
        public IEnumerable<Gender> Gender { get; set; }
    }
}