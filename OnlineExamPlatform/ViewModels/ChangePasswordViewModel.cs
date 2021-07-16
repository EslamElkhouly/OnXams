using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace OnlineExamPlatform.ViewModels
{
    public class ChangePasswordViewModel
    {
        [Required]
        [Display(Name = "Old Password")]
        [DataType(DataType.Password)]
        public string OldPassword { get; set; }


        [Required]
        [Display(Name = "New Password")]
        [DataType(DataType.Password)]
        [StringLength(255, ErrorMessage = "Must be between 5 and 255 characters", MinimumLength = 6)]
        public string NewPassword { get; set; }


        [Required]
        [Display(Name = "Confirm  Password")]
        [DataType(DataType.Password)]
        [StringLength(255, ErrorMessage = "Must be between 5 and 255 characters", MinimumLength = 6)]
        [Compare("NewPassword",ErrorMessage = "password must match")]
        public string ConfirmNewPassword { get; set; }
    }
}