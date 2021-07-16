using System;
using OnlineExamPlatform.Models;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;



namespace OnlineExamPlatform.ViewModels
{
    public class ExamProcessViewModel
    {


        [Display(Name = "Exam Name")]
        [Required]
        public string ExamName { get; set; }



        [Required]
        [Display(Name = "Exam Description")]
        public string PaperDescription { get; set; }


        [Required]
        [Display(Name = "Start Date ")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy/MM/dd}", ApplyFormatInEditMode = true)]
        public DateTime? StartDate { get;set; }




        [Display(Name = "Start Time")]
        [Required]
        public TimeSpan? StartTime { get; set; }




        [Required]
        [Display(Name = "Duration (In Minutes)")]
        public int? Duration { get; set; }







        public bool IsExamActive { get; set; }




        



        [Display(Name = "Marks Per Question")]
        [Required]
        [Range(1, 100, ErrorMessage = "Please enter correct value between 1,100")]
        public int? MarksPerQuestion { get; set; }





   




        public ExamHeader ExamHeader { get; set; }
       


        public long ExaminerId{ get; set; }



        public long CourseId { get; set; }



        public List<StudentGroup> StudentGroup { get; set; }
      

        public List<QuestionData> QuestionData { get; set; }

       
    }
}