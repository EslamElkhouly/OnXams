
using System;
using System.Collections.Generic;
using OnlineExamPlatform.Models;

namespace OnlineExamPlatform.ViewModels
{
    public class ExamInformationViewModel
    {
        public Guid CourseGuid { get; set; }
        public List<QuestionData> QuestionData { get; set; }
        public IEnumerable<ExamHeader> ExamHeader { get; set; }
        public IEnumerable<ExamDetail> ExamDetail { get; set; }
    }
}