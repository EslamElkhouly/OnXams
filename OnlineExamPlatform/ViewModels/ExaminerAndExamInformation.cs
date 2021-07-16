
using System;
using System.ComponentModel.DataAnnotations;


namespace OnlineExamPlatform.ViewModels
{
    public class ExaminerAndExamInformation
    {
        [EmailAddress]
        public string Email { get; set; }
        public string Title { get; set; }

        [Display(Name = "Full Name")]
        public string FullName { get; set; }


        [Display(Name = "Birth Date")]
        public DateTime BirthDate { get; set; }

        [Phone]
        [Display(Name = "Phone Number")]
        public string PhoneNumber { get; set; }

        public string Gender { get; set; }


        [Display(Name = "Course Name")]
        public string CourseName { get; set; }


        [Display(Name = "Exam Name")]
        public string ExamName { get; set; }


        [Display(Name = "Start Date")]
        public DateTime? StartDate{ get; set; }


        [Display(Name = "Start Time")]
        public TimeSpan? StartTime{ get; set; }



        public decimal? Duration { get; set; }


        public int? NoOfGroups { get; set; }
        
  


      
    }

}