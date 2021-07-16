
using System.ComponentModel.DataAnnotations;
using System;
using System.Security.AccessControl;
using OnlineExamPlatform.Models;

namespace OnlineExamPlatform.ViewModels
{
    public class StudentExamInformationViewModel
    {
        [EmailAddress]
        public string Email { get; set; }
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


        [Display(Name = "Examiner Name")]
        public string ExaminerName { get; set; }

        [Display(Name = "Start Time")]
        public TimeSpan? StartTime{ get; set; }
        public TimeSpan? EndTime{ get; set; }

        [Display(Name = "Start Date")]
        public DateTime? StartDate { get; set; }


        public decimal? Duration { get; set; }

        public string Department { get; set; }
        public string GradeYear { get; set; }
        public string GroupName { get; set; }

        public Guid ExamHeaderGuid { get; set; }
        public long StudentGroupId { get; set; }

        public ExamHeader ExamHeader { get; set; }
    }
}