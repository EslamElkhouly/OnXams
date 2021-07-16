using System.Collections.Generic;

using OnlineExamPlatform.Models;

namespace OnlineExamPlatform.ViewModels
{
    public class GetAdminStudentExaminerProctorCount
    {
        
        public int StudentData { get; set; }
        public int AdminData { get; set; }
        public int ProctorData { get; set; }
        public int ExaminerData { get; set; }
        public IEnumerable<LogData> LogData{ get; set; }

       
    }
}