using System.Collections.Generic;
using OnlineExamPlatform.Models;

namespace OnlineExamPlatform.ViewModels
{
    public class AdminViewModel
    {
        public AdminData AdminData { get; set; }
        public UserAuthentication UserAuthentication { get; set; }
        public IEnumerable<Gender> Gender { get; set; }
    }
}