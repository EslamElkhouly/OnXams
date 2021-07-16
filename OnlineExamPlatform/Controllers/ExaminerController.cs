using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using OnlineExamPlatform.Authentication;
using OnlineExamPlatform.Models;
using OnlineExamPlatform.ViewModels;
using System.Data.Entity;
using System.Data.Entity.Validation;


namespace OnlineExamPlatform.Controllers
{
    [Authorize(Roles = "examiner")]
    public class ExaminerController : Controller
    {
        private OnXamsEntities _context;

        public ExaminerController()
        {
            _context = new OnXamsEntities();
        }

        protected override void Dispose(bool disposing)
        {
            _context.Dispose();
        }




        // GET: Examiner
        public ActionResult Index()
        {

            var examinerAuthentication = _context.UserAuthentications.SingleOrDefault
            (u =>
                u.Email.ToLower()
                    .Equals(User.Identity.Name));


            var examinerData =
                _context.ExaminerDatas.SingleOrDefault(e => e.ExaminerID == examinerAuthentication.ExaminerId);


            var fullName = examinerData.FirstName + " " + examinerData.MiddleName + " " + examinerData.LastName;


            //المفروض اتاكد ان الامتحان دا تبع الدكتور دا !!
            var exam = _context.ExamHeaders.Where(c => c.ExaminerId == examinerData.ExaminerID
                                                     && c.IsActive == true)
                .OrderBy(c => c.StartDate)
                .ThenBy(c => c.StartTime)
                .FirstOrDefault();
            if (exam == null)
            {
                var model1 = new ExaminerAndExamInformation()
                {
                    Email = examinerAuthentication.Email,
                    Title = examinerData.ScientificDegree,
                    FullName = fullName,
                    BirthDate = (DateTime)examinerData.BirthDate,
                    PhoneNumber = examinerData.PhoneNumber,
                    Gender = examinerData.Gender.GenderName,
                };
                return View(model1);
            }

            var numberOfGroups = _context.ExamDetails.Where(c => c.ExamID == exam.ExamId && c.IsActive == true)
                .Select(c => c.StudentGroupID).Distinct();

            var model = new ExaminerAndExamInformation()
            {
                Email = examinerAuthentication.Email,
                Title = examinerData.ScientificDegree,
                FullName = fullName,
                BirthDate = (DateTime)examinerData.BirthDate,
                PhoneNumber = examinerData.PhoneNumber,
                Gender = examinerData.Gender.GenderName,
                CourseName = exam.CourseData.CourseName,
                StartDate = exam.StartDate,
                StartTime = exam.StartTime,
                Duration = exam.Duration,
                ExamName = exam.ExamName,
                NoOfGroups = numberOfGroups.Count()



            };

            return View(model);
        }












        [HttpGet]
        public ActionResult ExamSchedule()
        {

            var examDetail = _context.ExamDetails.Include(c => c.StudentGroup).Include(c => c.ExamHeader).ToList();
            return View(examDetail);
        }



        public ActionResult ExaminerProfile()
        {
            var examinerAuthentication = _context.UserAuthentications.SingleOrDefault
            (u =>
                u.Email.ToLower()
                    .Equals(User.Identity.Name));


            var examinerData =
                _context.ExaminerDatas.SingleOrDefault(e => e.ExaminerID == examinerAuthentication.ExaminerId);

            var model = new ExaminerProfileViewModel()
            {
                ExaminerData = examinerData,
                UserAuthentication = examinerAuthentication
            };
            return View(model);
        }









        public ActionResult ChangePassword()
        {
            var model = new ChangePasswordViewModel();
            return View("ChangePassword", model);
        }




        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SavePassword(ChangePasswordViewModel model)
        {
            var examinerAuthentication = _context.UserAuthentications
                .SingleOrDefault
                (u =>
                    u.Email.ToLower()
                        .Equals(User.Identity.Name));


            var examinerData =
                _context.ExaminerDatas.SingleOrDefault
                    (e => e.ExaminerID == examinerAuthentication.ExaminerId);

            if (!ModelState.IsValid)
            {
                ModelState.AddModelError(" ", "Some Information are Invalid");
                return View("ChangePassword", model);
            }

            var checkOldPassword = HashingPassword.ValidatePassword(model.OldPassword
                , examinerAuthentication.Password);


            if (!checkOldPassword)
            {
                ModelState.AddModelError(" ", "Old password does not match");
                return View("ChangePassword", model);

            }


            if (model.NewPassword != model.ConfirmNewPassword)
            {
                ModelState.AddModelError(" ", "Confirm Password must Match New password");
                return View("ChangePassword", model);


            }
            examinerAuthentication.Password = HashingPassword.HashPassword(model.NewPassword);
            _context.SaveChanges();
            return RedirectToAction("ExaminerProfile", "Examiner");

        }




















        /*#############################################################################*/

















        public ActionResult StudentGroupForExam(Guid courseGuid)
        {
            var examinerAuthentication = _context.UserAuthentications.SingleOrDefault
            (u =>
                u.Email.ToLower()
                    .Equals(User.Identity.Name));


            var examinerId =
                _context.ExaminerDatas.SingleOrDefault(e => e.ExaminerID == examinerAuthentication.ExaminerId).ExaminerID;


            var courseId = _context.CourseDatas.SingleOrDefault(c => c.CourseDataGUID
                                                                     == courseGuid
                                                                     &&
                                                                     c.IsDeleted != true).CourseDataID;



            var studentListId = _context.CourseTransactions
                  .Where(s => s.IsDeleted == null || s.IsDeleted == false)
                .Where(c => c.CourseDataID == courseId
                                                 &&
                                                 c.ExaminerID == examinerId).Select(c => c.StudentID).ToList();


            //for students in that course for that examiner
            var studentResult = _context.StudentDatas
                 .Where(s => s.IsDeleted == null || s.IsDeleted == false)
                .Where(s => studentListId.Contains(s.StudentID));


            //for dropdown list
            var studentGroup = _context.StudentGroups
                .Where(s => s.IsDeleted == null || s.IsDeleted == false);



            var model = new StudentGroupViewModel()
          {
              StudentData = studentResult,
              StudentGroup = studentGroup,
              CourseGuid = courseGuid
          };
            return View(model);
        }







        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SaveStudentGroup(StudentGroupViewModel model, string[] AreChecked, Guid courseGuid)
        {


            #region SaveNewEntry




            if (AreChecked == null)
            {
                return View("StudentGroupForExam", model);
            }

            if (!ModelState.IsValid)
            {
                return View("StudentGroupForExam", model);
            }
            var examinerAuthentication = _context.UserAuthentications.SingleOrDefault
            (u =>
                u.Email.ToLower()
                    .Equals(User.Identity.Name));


            var examinerId = _context.ExaminerDatas.SingleOrDefault
                (e => e.ExaminerID == examinerAuthentication.ExaminerId)
                .ExaminerID;


            var courseId = _context.CourseDatas
                .SingleOrDefault(c => c.CourseDataGUID == courseGuid).CourseDataID;





            var studentIds = Array.ConvertAll(AreChecked, a => long.Parse(a));

            //have list of ids for selected students bt examiner in course
            var studentResult = _context.StudentDatas
                .Where(s => studentIds.Contains(s.StudentID))
                .Select(s => s.StudentID).ToList();





            var studentGroup = new StudentGroup()
            {
                StudentGroupGuid = Guid.NewGuid(),
                GroupName = model.GroupName,
                CourseDataId = courseId,
                ExaminerId = examinerId

            };
            _context.StudentGroups.Add(studentGroup);
            _context.SaveChanges();
            var studentGroupId = _context.StudentGroups
                .Where(s => s.ExaminerId == examinerId
                            &&
                            s.CourseDataId == courseId)
                .OrderByDescending(c => c.StudentGroupID)
                .First().StudentGroupID;
            for (int i = 0; i < studentResult.Count; i++)
            {
                var group = new StudentMapToGroup();
                group.StudentGroupID = studentGroupId;
                group.StudentID = studentResult[i];

                _context.StudentMapToGroups.Add(group);
            }
            _context.SaveChanges();


            #endregion
            return RedirectToAction("StudentGroupForExam", new { courseGuid = courseGuid });

        }



        public ActionResult DeleteStudentGroup(long studentGroupId)
        {

            var studentGroup = _context.StudentGroups.SingleOrDefault
                (s => s.StudentGroupID == studentGroupId);
            if (studentGroup == null)
            {
                return HttpNotFound();
            }

            var courseGuid = _context.CourseDatas.SingleOrDefault
                    (c => c.CourseDataID == studentGroup.CourseDataId)
                .CourseDataGUID;
            studentGroup.IsDeleted = true;
            _context.SaveChanges();


          // var studentMapToGroup= _context.StudentMapToGroups.Where(c => c.StudentGroupID == studentGroup.StudentGroupID).ToList();


            return RedirectToAction("StudentGroupForExam", new { courseGuid = courseGuid });
        }
        //done




        [HttpGet]
        public ActionResult GetStudentsByGroupId(long studentGroupId)
        {
            _context.Configuration.ProxyCreationEnabled = false;
            var studentIds = _context.StudentMapToGroups.Where(c =>
                c.StudentGroupID == studentGroupId).Select(s => s.StudentID).ToList();


            var studentList = _context.StudentDatas
                 .Where(s => s.IsDeleted == null || s.IsDeleted == false)
                .Where(s => studentIds.Contains(s.StudentID)).ToList();

            return Json(studentList, JsonRequestBehavior.AllowGet);
        }



















        public ActionResult NewQuestion(Guid guid)
        {
            var questionWeight = _context.QuestionWeights.ToList();
            var questionType = _context.QuestionTypes.ToList();
            var category = _context.Categories.ToList();
            var subCategory = _context.SubCategories.ToList();


            var model = new NewQuestionViewModel()
            {
                QuestionType = questionType,
                QuestionWeight = questionWeight,
                Category = category,
                SubCategory = subCategory,
                CourseGuid = guid,

                OptionsOfMcqQuestion = new List<OptionsOfMcqQuestion>()



            };

            return View(model);
        }


        public ActionResult EditQuestion(Guid questionGuid, Guid courseGuid)
        {
            var questionDataInDb = _context.QuestionDatas.SingleOrDefault(q => q.QuestionDataGUID == questionGuid);
            if (questionDataInDb == null)
            {
                return HttpNotFound();
            }


            var optionsOfMcqQuestion = _context.OptionsOfMcqQuestions
                .Where(o => o.QuestionDataID == questionDataInDb.QuestionDataID)
                .Where(o => o.IsDeleted == null || o.IsDeleted == false)
                .ToList();


            var courseData = _context.CourseDatas.SingleOrDefault(c => c.CourseDataGUID == courseGuid);
            var questionWeightId = (int)questionDataInDb.QuestionWeightId;
            var questionTypeId = (int)questionDataInDb.QuestionTypeId;
            var categoryId = (int)questionDataInDb.CategoryId;
            var subCategoryId = (int)questionDataInDb.SubCategoryId;





            var questionWeight = _context.QuestionWeights.ToList();
            var questionType = _context.QuestionTypes.ToList();
            var category = _context.Categories.ToList();
            var subCategory = _context.SubCategories.ToList();




            var model = new NewQuestionViewModel()
            {
                AnswerTimeSpan = questionDataInDb.AnswerTime,
                QuestionData = questionDataInDb,
                QuestionTypeId = questionTypeId,
                QuestionWeightId = questionWeightId,
                CategoryId = categoryId,
                SubCategoryId = subCategoryId,
                QuestionType = questionType,
                QuestionWeight = questionWeight,
                Category = category,
                SubCategory = subCategory,
                CourseGuid = (Guid)courseData.CourseDataGUID,
                OptionsOfMcqQuestion = optionsOfMcqQuestion
            };

            return View("NewQuestion", model);
        }

        [ValidateInput(false)]
        [ValidateAntiForgeryToken]
        [HttpPost]


        public ActionResult SaveNewQuestion(NewQuestionViewModel model)
        {
            //Get ExaminerID
            var examinerAuthentication = _context.UserAuthentications
                .SingleOrDefault
                (u =>
                    u.Email.ToLower()
                        .Equals(User.Identity.Name));


            var examinerData =
                _context.ExaminerDatas.SingleOrDefault
                    (e => e.ExaminerID == examinerAuthentication.ExaminerId);

            //get CourseDataID
            var courseDataId = _context.CourseDatas.SingleOrDefault
                (c => c.CourseDataGUID == model.CourseGuid)
                .CourseDataID;
            //#####################################################################
            #region New Entity
            //New Entity
            if (model.QuestionData.QuestionDataGUID == null)
            {
                //question Without Options
                model.QuestionData.QuestionDataGUID = Guid.NewGuid();
                model.QuestionData.CategoryId = model.CategoryId;
                model.QuestionData.SubCategoryId = model.SubCategoryId;
                model.QuestionData.QuestionTypeId = model.QuestionTypeId;
                model.QuestionData.QuestionWeightId = model.QuestionWeightId;
                model.QuestionData.AnswerTime = model.AnswerTimeSpan;
                model.QuestionData.ExaminerID = examinerData.ExaminerID;
                model.QuestionData.CourseDataId = courseDataId;
                _context.QuestionDatas.Add(model.QuestionData);
                _context.SaveChanges();

                var questionDataId = model.QuestionData.QuestionDataID;

                if (model.OptionsOfMcqQuestion != null)
                {
                    //question With options
                    var options = new OptionsOfMcqQuestion();
                    for (int i = 0; i < model.OptionsOfMcqQuestion.Count; i++)
                    {
                        options.QuestionDataID = questionDataId;
                        options.OptionsOfMcqQuestionsGUID = Guid.NewGuid();
                        options.OptionNumber = model.OptionsOfMcqQuestion[i].OptionNumber;
                        options.OptionDescription = model.OptionsOfMcqQuestion[i].OptionDescription;
                        options.RightOrNot = false;
                        _context.OptionsOfMcqQuestions.Add(options);
                        _context.SaveChanges();

                    }
                }
            }
            #endregion
            //#################################################################################









            #region Modify old Entity

            //Modify old Entity
            else
            {

                var questionDataInDb = _context.QuestionDatas
                    .SingleOrDefault(q => q.QuestionDataGUID
                                          == model.QuestionData.QuestionDataGUID);


                //Get  Options for a question From DB
                var optionsOfMcqQuestionsInDb = _context.OptionsOfMcqQuestions.
                    Where(o => o.QuestionDataID == questionDataInDb.QuestionDataID)
                    .Where(o => o.IsDeleted == false || o.IsDeleted == null)
                    .ToList();



                //edit questionData in db
                questionDataInDb.QuestionDescription = model.QuestionData.QuestionDescription;
                questionDataInDb.CategoryId = model.CategoryId;
                questionDataInDb.SubCategoryId = model.SubCategoryId;
                questionDataInDb.QuestionTypeId = model.QuestionTypeId;
                questionDataInDb.QuestionWeightId = model.QuestionWeightId;
                questionDataInDb.QuestionAnswer = model.QuestionData.QuestionAnswer;
                questionDataInDb.AnswerTime = model.AnswerTimeSpan;
                questionDataInDb.QuestionSource = model.QuestionData.QuestionSource;
                _context.SaveChanges();

                //Check Question Type

                var getQuestionTypeInDb = _context.QuestionTypes
                    .SingleOrDefault(q => q.QuestionTypeId == model.QuestionTypeId).QuestionTypeName;


                //check type of question returned from view
                if (getQuestionTypeInDb == "MCQ" || getQuestionTypeInDb == "True and False")
                {
                    //If there Is Options(Do Not known Their Numbers)
                   
                        foreach (var option in optionsOfMcqQuestionsInDb)
                        {
                            _context.OptionsOfMcqQuestions.Remove(option);
                            _context.SaveChanges();

                        }

                        var options = new OptionsOfMcqQuestion();
                        for (int i = 0; i < model.OptionsOfMcqQuestion.Count; i++)
                        {
                            options.QuestionDataID = questionDataInDb.QuestionDataID;
                            options.OptionNumber = model.OptionsOfMcqQuestion[i].OptionNumber;
                            options.OptionDescription = model.OptionsOfMcqQuestion[i].OptionDescription;
                            options.OptionsOfMcqQuestionsGUID=Guid.NewGuid();
                            options.RightOrNot = false;
                            _context.OptionsOfMcqQuestions.Add(options);
                            _context.SaveChanges();

                        }
                }
                else
                {
                    foreach (var option in optionsOfMcqQuestionsInDb)
                    {
                        _context.OptionsOfMcqQuestions.Remove(option);
                        _context.SaveChanges();
                        
                    }
                }


            }
            #endregion

            return RedirectToAction("Subject", "Examiner", new { guid = model.CourseGuid });
        }

        //Render Courses For Each Examiner
        [AcceptVerbs(HttpVerbs.Post | HttpVerbs.Get)]
        public ActionResult SideBarExaminer()
        {
            var examinerAuthentication = _context.UserAuthentications.SingleOrDefault
            (u =>
                u.Email.ToLower()
                    .Equals(User.Identity.Name));

            var examinerData =
                _context.ExaminerDatas.SingleOrDefault
                    (e => e.ExaminerID == examinerAuthentication.ExaminerId);

            var examinerCourses =
                _context.CourseTransactions.Where(c =>
                        c.ExaminerID == examinerData.ExaminerID)
                    .Where(c => c.IsDeleted == false ||
                                c.IsDeleted == null).Select(c => c.CourseData).Distinct();



            return PartialView("sidebar/_SideBarExaminer", examinerCourses);
        }


        //Render All Questions
        public ActionResult Question(Guid courseGuid)
        {


            var examinerAuthentication = _context.UserAuthentications.SingleOrDefault
            (u =>
                u.Email.ToLower()
                    .Equals(User.Identity.Name));


            var examinerData =
                _context.ExaminerDatas.SingleOrDefault(e => e.ExaminerID == examinerAuthentication.ExaminerId);



            var courseData = _context.CourseDatas.SingleOrDefault(c => c.CourseDataGUID == courseGuid);

            var questionData = _context.QuestionDatas
                .Where(q => q.
                    CourseDataId == courseData.CourseDataID
                    &&
                    q.ExaminerID == examinerData.ExaminerID)
                .Where(q => q.IsDeleted == null || q.IsDeleted == false);




            return View(questionData.ToList());
        }










        //Delete a single question
        public ActionResult DeleteQuestion(Guid questionGuid, Guid courseGuid)
        {
            var questionData = _context.QuestionDatas.SingleOrDefault(q => q.QuestionDataGUID == questionGuid);
            if (questionData == null)
            {
                return HttpNotFound();
            }

            var courseData = _context.CourseDatas.SingleOrDefault(c => c.CourseDataID == questionData.CourseDataId);

            if (courseData == null)
            {
                return HttpNotFound();
            }

            questionData.IsDeleted = true;
            _context.SaveChanges();

            var optionsOfMcqQuestions = _context.OptionsOfMcqQuestions.Where(c => c.QuestionDataID == questionData.QuestionDataID).ToList();
            if (optionsOfMcqQuestions.Count!=0)
            {
                foreach (var option in optionsOfMcqQuestions)
                {
                    option.IsDeleted = true;
                    _context.SaveChanges();
                }
            }

            return RedirectToAction("Question", "Examiner", new { courseGuid });
        }



        // Render Subjects For Examiner
        public ActionResult Subject(Guid guid)
        {
            var courseData = _context.CourseDatas
                .SingleOrDefault(c => c.CourseDataGUID == guid);



            var examinerAuthentication = _context.UserAuthentications.SingleOrDefault
            (u =>
                u.Email.ToLower()
                    .Equals(User.Identity.Name));


            var examinerData =
                _context.ExaminerDatas.SingleOrDefault
                    (e => e.ExaminerID ==
                          examinerAuthentication.ExaminerId);


            var examinerCourses = _context.CourseTransactions.Where(c =>
                    c.ExaminerID == examinerData.ExaminerID &&
                    c.CourseDataID == courseData.CourseDataID)
                .Where(e =>
                    e.IsDeleted == null || e.IsDeleted == false);


            var questionData = _context.QuestionDatas.Where(q =>
                 q.ExaminerID == examinerData.ExaminerID &&
                     q.CourseDataId == courseData.CourseDataID)
                 .Where(q =>
                     q.IsDeleted == null || q.IsDeleted == false);


            var examData = _context.ExamHeaders.Where(e =>
                    e.ExaminerId == examinerData.ExaminerID &&
                    e.CourseDataID == courseData.CourseDataID)
                ;

            var studentCount = examinerCourses.ToList().Count();
            var questionCount = questionData.ToList().Count();
            var examCount = examData.ToList().Count();

            var model = new SubjectViewModel()
            {
                StudentCount = studentCount,
                QuestionsCount = questionCount,
                ExamCount = examCount,
                CourseData = courseData

            };

            return View(model);
        }





    }
}