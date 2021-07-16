using OnlineExamPlatform.Models;
namespace OnlineExamPlatform.ViewModels
{
    public class SubjectViewModel
    {
        public CourseData CourseData { get; set; }

        public long StudentCount { get; set; }

        public long ExamCount { get; set; }

        public long QuestionsCount { get; set; }

    }
}