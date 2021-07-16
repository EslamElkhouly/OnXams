

using OnlineExamPlatform.Models;

namespace OnlineExamPlatform.ViewModels
{
    public class StudentShowHisResult
    {
        public ExamHeader ExamHeader { get; set; }
        public long NumberOfQuestion { get; set; }
        public ExaminationProcess ExaminationProcess { get; set; }
        public long FinaleMark { get; set; }
        public decimal? StudentResult { get; set; }

    }
}