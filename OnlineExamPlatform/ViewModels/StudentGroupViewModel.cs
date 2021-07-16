using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using OnlineExamPlatform.Models;

namespace OnlineExamPlatform.ViewModels
{
    public class StudentGroupViewModel
    {
        [Required]
        [Display(Name = "Student Groups")]
        public long StudentGroupId { get; set; }
        [Required]
        [Display(Name = "Group Name")]
        public string GroupName { get; set; }

        public IEnumerable<StudentGroup> StudentGroup { get; set; }


        public IEnumerable<StudentData> StudentData { get; set; }
        public Guid CourseGuid { get; set; }
       
       
    }
}