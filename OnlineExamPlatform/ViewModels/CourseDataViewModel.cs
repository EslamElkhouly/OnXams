using System.Collections.Generic;
using OnlineExamPlatform.Models;

namespace OnlineExamPlatform.ViewModels
{
    public class CourseDataViewModel
    {
        public CourseData CourseData { get; set; }


       


        public IEnumerable<Department> Department { get; set; }

        public IEnumerable<Semester> Semester { get; set; }


        public IEnumerable<GradeYear> GradeYear { get; set; }

    }
}