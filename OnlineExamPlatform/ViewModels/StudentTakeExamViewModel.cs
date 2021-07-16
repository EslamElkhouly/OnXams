
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using OnlineExamPlatform.Models;

namespace OnlineExamPlatform.ViewModels
{
    public class StudentTakeExamViewModel
    {
        
        public IEnumerable<ExamDetail> ExamDetail { get; set; }
        public List<QuestionData> QuestionData { get; set; }
        public string CourseName { get; set; }
        public string CourseCode { get; set; }
        public DateTime StartDate { get; set; }
        public decimal Duration { get; set; }
        public string ExamName { get; set; }
        public string ExamDescription { get; set; }
        public string AcademicYear { get; set; }
        public long ExamHeaderId { get; set; }
        public string ExaminerName { get; set; }
        public string DepartmentName { get; set; }
        public string GradeYearName { get; set; }

        public string Semester { get; set; }
        public string GroupName { get; set; }

        public string Selected { get; set; }

        [Display(Name = "Your answer")]
        public string StudentAnswerWrite { get; set; }

        public TimeSpan StartTime { get; set; }
        public TimeSpan EndTime { get; set; }

    }
}