using System;
namespace OnlineExamPlatform.ViewModels
{
    public class StudentCourseViewModel
    {
        public string CourseName { get; set; }
        public string CourseCode { get; set; }
        public string Department { get; set; }
        public string GroupName { get; set; }
        public Guid CourseGuid { get; set; }
    }
}