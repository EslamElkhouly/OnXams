using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using OnlineExamPlatform.Models;

namespace OnlineExamPlatform.ViewModels
{
    public class NewQuestionViewModel
    {
        [Required]
        [Display(Name = "Question Type")]
        public int QuestionTypeId { get; set; }

        [Required]
        [Display(Name = "Category")]
        public int CategoryId { get; set; }


        [Required]
        [Display(Name = "Sub Category")]
        public int SubCategoryId { get; set; }


       


        [Required]
        [Display(Name = "Answer Time")]
        [DisplayFormat(DataFormatString = "{0:mm\\:ss}", ApplyFormatInEditMode = true)]
        public TimeSpan AnswerTimeSpan { get; set; }



        [Required]
        [Display(Name = "Question Weight")]
        public int QuestionWeightId { get; set; }

        [Required(ErrorMessage = "Please select a Type")]
        public IEnumerable<QuestionType> QuestionType { get; set; }


        [Required(ErrorMessage = "Please select a Category")]
        public IEnumerable<Category> Category{ get; set; }
        public IEnumerable<SubCategory> SubCategory{ get; set; }
        public IEnumerable<QuestionWeight> QuestionWeight { get; set; }

        public QuestionData QuestionData { get; set; }
        //public List<string> OptionsList { get; set; }


        public List<OptionsOfMcqQuestion> OptionsOfMcqQuestion { get; set; }
        public Guid CourseGuid { get; set; }


    }
}