using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System;
using OnlineExamPlatform.Models;

namespace OnlineExamPlatform.ViewModels
{
    public class CourseTransactionViewModel
    {
        [Required]
        [Display(Name = "Department")]
        public int DepartmentId  { get; set; }

        [Display(Name = "Semester")]
        public int SemesterId  { get; set; }

        [Display(Name = "Grade Year")]
        public int GradeYearId  { get; set; }

        [Required]
        [Display(Name = "Course Data")]
        public long CourseDataId { get; set; }

        [Display(Name = "Select Examiner")]
        [Required]
        public long ExaminerId { get; set; }


        public IEnumerable<Department> Department { get; set; }
        public IEnumerable<Semester> Semester { get; set; }
        public IEnumerable<GradeYear> GradeYear { get; set; }
        public IEnumerable<CourseData> CourseData { get; set; }


        public IEnumerable<ExaminerData> ExaminerData { get; set; }
        public CourseTransaction CourseTransaction { get; set; }
        public StudentData StudentData { get; set; }


        [Display(Name = "Academic Year")]
        [Required]
        public DateTime? AcademicYear { get; set; }
        
    }
}