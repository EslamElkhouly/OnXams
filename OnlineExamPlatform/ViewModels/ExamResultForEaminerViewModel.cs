
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using OnlineExamPlatform.Models;

namespace OnlineExamPlatform.ViewModels
{
    public class ExamResultForEaminerViewModel
    {
        [Display(Name = "Exam Name")]
        public long ExamId { get; set; }

        public string CourseName { get; set; }
        public string CourseCode { get; set; }
        public List<ExamHeader> ExamHeader { get; set; }
       
       

    }
}