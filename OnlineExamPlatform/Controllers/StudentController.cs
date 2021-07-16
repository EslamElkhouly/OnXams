using System;
using System.Web.Mvc;
using OnlineExamPlatform.Models;
using System.Linq;
using OnlineExamPlatform.ViewModels;



namespace OnlineExamPlatform.Controllers
{
    [Authorize(Roles = "student")]
    public class StudentController : Controller
    {
        private OnXamsEntities _context;

        public StudentController()
        {
            _context = new OnXamsEntities();
        }

        protected override void Dispose(bool disposing)
        {
            _context.Dispose();
        }




        public ActionResult Index()
        {

            var studentAuthentication = _context.UserAuthentications.SingleOrDefault
            (u =>
                u.Email.ToLower()
                    .Equals(User.Identity.Name));


            var studentData =
                _context.StudentDatas.SingleOrDefault(e => e.StudentID == studentAuthentication.StudentID);


            var fullName = studentData.FirstName + " " + studentData.MiddleName + " " + studentData.LastName;


            var studentGroupsListId = _context.StudentMapToGroups.Where(s => s.StudentID == studentData.StudentID).Select(c=>c.StudentGroupID).ToList();


            //get list of student courses ids
            var studentCourses = _context.CourseTransactions
                .Where(c => c.StudentID == studentData.StudentID)
                .Where(c => c.IsDeleted == false || c.IsDeleted == null)
                .Select(c => c.CourseDataID).ToList();

         
            
            //get early exam;

            var exam = _context.ExamHeaders.Where(c => c.IsActive == true
                                                       &&studentCourses.Contains(c.CourseDataID))
                .OrderBy(c => c.StartDate)
                .ThenBy(c => c.StartTime)
                .FirstOrDefault();

            if (exam==null)
            {
                var model1 = new StudentExamInformationViewModel()
                {
                    ExamHeader= exam,
                    Email = studentAuthentication.Email,
                    FullName = fullName,
                    PhoneNumber = studentData.PhoneNumber,
                    Department = studentData.Department.DepartmentName,
                    GradeYear = studentData.GradeYear.GradeYearName,
                    Gender = studentData.Gender.GenderName,
                    BirthDate = studentData.BirthDate,
                 

                };
                return View(model1);
            }

            var examDetails = _context.ExamDetails.First(c => c.ExamID == exam.ExamId && studentGroupsListId.Contains(c.StudentGroupID));


            TimeSpan duration = new TimeSpan(0, 0, (int)exam.Duration, 0);


            
            var model = new StudentExamInformationViewModel()
            {
                Email = studentAuthentication.Email,
                FullName = fullName,
                PhoneNumber = studentData.PhoneNumber,
                Department = studentData.Department.DepartmentName,
                GradeYear = studentData.GradeYear.GradeYearName,
                Gender = studentData.Gender.GenderName,
                BirthDate = studentData.BirthDate,
                ExamName = examDetails.ExamHeader.ExamName,
                StartDate = examDetails.ExamHeader.StartDate,
                StartTime = examDetails.ExamHeader.StartTime,
                Duration = examDetails.ExamHeader.Duration,
                ExaminerName = examDetails.ExamHeader.ExaminerData.FirstName + " " + examDetails.ExamHeader.ExaminerData.LastName,
                CourseName = examDetails.ExamHeader.CourseData.CourseName,
                GroupName = examDetails.StudentGroup.GroupName,
                ExamHeaderGuid=(Guid)examDetails.ExamHeader.ExamGUID,
                StudentGroupId = (long)examDetails.StudentGroupID,
                EndTime = exam.StartTime.Add(duration)


            };
      
            return View(model);

        }


        [AcceptVerbs(HttpVerbs.Post | HttpVerbs.Get)]
        public ActionResult SideBarStudent()
        {
            var studentAuthentication = _context.UserAuthentications.SingleOrDefault
            (u =>
                u.Email.ToLower()
                    .Equals(User.Identity.Name));


            var studentData =
                _context.StudentDatas.SingleOrDefault(e => e.StudentID == studentAuthentication.StudentID);

            var studentCourses = _context.CourseTransactions.Where(
                    c => c.StudentID == studentData.StudentID).Where(c => c.IsDeleted == false ||
                                                                          c.IsDeleted == null)
                .Select(c => c.CourseData).Distinct();

            return PartialView("sidebar/_SideBarStudent", studentCourses);
        }





        public ActionResult Subject(Guid courseGuid)
        {
            var courseData = _context.CourseDatas
                .SingleOrDefault(c => c.CourseDataGUID == courseGuid);
           
            
            var studentAuthentication = _context.UserAuthentications.SingleOrDefault
            (u =>
                u.Email.ToLower()
                    .Equals(User.Identity.Name));


            var studentData =
                _context.StudentDatas.SingleOrDefault(e => e.StudentID == studentAuthentication.StudentID);

            var model = new StudentCourseViewModel()
            {
                CourseName = courseData.CourseName,
                CourseCode = courseData.CourseCode,
                Department = courseData.Department.DepartmentName,
                CourseGuid = (Guid)courseData.CourseDataGUID
            };
            
            
            return View(model);
        }



        public ActionResult StudentExamInCourse(Guid courseGuid)
        {
            
            var studentAuthentication = _context.UserAuthentications.SingleOrDefault
            (u =>
                u.Email.ToLower()
                    .Equals(User.Identity.Name));


            var studentData =
                _context.StudentDatas.SingleOrDefault(e => e.StudentID == studentAuthentication.StudentID);



            
            
            var courseDate = _context.CourseDatas.SingleOrDefault(c => c.CourseDataGUID == courseGuid);


            

            var studentMapToGroupListId = _context.StudentMapToGroups.Where(c => c.StudentID == studentData.StudentID).Select(c=>c.StudentGroupID);

          var groups=  _context.StudentGroups.Where(c => c.IsDeleted != true && c.CourseDataId == courseDate.CourseDataID
                                                                  && studentMapToGroupListId
                                                                      .Contains(c.StudentGroupID));



          var groupsListId = groups.Select(c => c.StudentGroupID).ToList();


            var examHeader =
                _context.ExamHeaders.Where(c => c.IsActive == true
                                                && c.CourseDataID == courseDate.CourseDataID).ToList();
            var examHeaderListIds = examHeader.Select(c => c.ExamId).ToList();

            var examDetails = _context.ExamDetails.Where(c => examHeaderListIds.Contains((long) c.ExamID)
            && groupsListId.Contains((long)c.StudentGroupID))
                .OrderBy(c => c.ExamHeader.StartDate)
                .ThenBy(c => c.ExamHeader.StartTime);

            
            return View(examDetails);
        }




        public ActionResult StudentTakeExam(Guid examHeaderGuid,long studentGroupId)
        {
            var studentAuthentication = _context.UserAuthentications.SingleOrDefault
            (u =>
                u.Email.ToLower()
                    .Equals(User.Identity.Name));


            var studentData =
                _context.StudentDatas.SingleOrDefault(e => e.StudentID == studentAuthentication.StudentID);



            var examHeader = _context.ExamHeaders.SingleOrDefault(c => c.ExamGUID == examHeaderGuid);
            if (examHeader==null)
            {
                return HttpNotFound();
            }

            //Validation on time and date in back-End

            TimeSpan duration = new TimeSpan(0, 0, (int) examHeader.Duration, 0);
            DateTime endExamTime = examHeader.StartDate + examHeader.StartTime.Add(duration);
            DateTime startExamTime = examHeader.StartDate + examHeader.StartTime;
            
            int result = DateTime.Compare( DateTime.Now, startExamTime);
            int value = DateTime.Compare(DateTime.Now, endExamTime);
            
            //Exam time has not come yet
            //if (result<0 && value<0) 
            //{
            //    return HttpNotFound();
            //}

            ////time finished
            //if (result>0 && value>0)
            //{
            //    return HttpNotFound();
            //}

           

            var examDetail = _context.ExamDetails.Where(c => c.ExamID == examHeader.ExamId
                                                             &&c.StudentGroupID==studentGroupId);
            var examDetailListQuestionId = examDetail.Select(c => c.QuestionID).ToList();




            var questionData = _context.QuestionDatas.Where(c => examDetailListQuestionId.Contains(c.QuestionDataID)&&c.IsDeleted!=true);
            var questionDateListId = questionData.Select(c => c.QuestionDataID);




            var optionOfMcq = _context.OptionsOfMcqQuestions.Where(c =>
                questionDateListId.Contains(c.QuestionDataID) && c.IsDeleted != true).ToList();

            var model = new StudentTakeExamViewModel()
            {
                ExamDetail = examDetail,
                QuestionData = questionData.ToList(),
               // OptionsOfMcqQuestions = optionOfMcq,
                CourseName = examHeader.CourseData.CourseName,
                CourseCode = examHeader.CourseData.CourseCode,
                StartDate = examHeader.StartDate,
                Duration = examHeader.Duration,
                ExamName = examHeader.ExamName,
                ExamDescription = examHeader.ExamDescription,
                AcademicYear = examHeader.AcademicYear,
                ExamHeaderId = examHeader.ExamId,
                ExaminerName = examHeader.ExaminerData.FirstName + " " + examHeader.ExaminerData.LastName,

                DepartmentName = studentData.Department.DepartmentName,
                GradeYearName = studentData.GradeYear.GradeYearName,
                Semester = examHeader.CourseData.Semester.SemesterName,
                StartTime = examHeader.StartTime,
                EndTime = examHeader.StartTime.Add(duration)

            };

            return View(model);
        }






        public ActionResult SaveStudentAnswers(StudentTakeExamViewModel model)
        {
            var studentAuthentication = _context.UserAuthentications.SingleOrDefault
            (u =>
                u.Email.ToLower()
                    .Equals(User.Identity.Name));


            var studentData =
                _context.StudentDatas.SingleOrDefault(e => e.StudentID == studentAuthentication.StudentID);


            //get submitted Exam header

            var examHeader = _context.ExamHeaders.SingleOrDefault(c => c.ExamId == model.ExamHeaderId);

            if (examHeader == null)
            {
                return HttpNotFound();
            }

            string result = null;
            foreach (var q in model.QuestionData)
            {
                if (q.QuestionTypeId==1 ||q.QuestionTypeId==2)
                {
                    if (q.SelectedAnswer != null)
                    {
                        result = _context.OptionsOfMcqQuestions
                            .SingleOrDefault(c => c.OptionsOfMcqQuestionsID == q.SelectedAnswer).OptionDescription;

                    }
                    var studentAnswerModel = new StudentAnswersQuestionInExam()
                    {
                        StudentAnswersQuestionInExamGUID = Guid.NewGuid(),
                        StudentID = studentData.StudentID,
                        ExamID = examHeader.ExamId,
                        QuestionID = q.QuestionDataID,
                        StudentAnswer = result

                    };
                    _context.StudentAnswersQuestionInExams.Add(studentAnswerModel);
                }
                else
                {
                    var studentAnswerModel = new StudentAnswersQuestionInExam()
                    {
                        StudentAnswersQuestionInExamGUID = Guid.NewGuid(),
                        StudentID = studentData.StudentID,
                        ExamID = examHeader.ExamId,
                        QuestionID = q.QuestionDataID,
                        StudentAnswer = q.StudentAnswerWrite,
                        

                    };
                    _context.StudentAnswersQuestionInExams.Add(studentAnswerModel);

                }
                
            }
            _context.SaveChanges();


            //calculate student result!!

            var studentAnswerInExam = _context.StudentAnswersQuestionInExams.Where(c =>
                c.StudentID == studentData.StudentID && c.ExamID == model.ExamHeaderId).ToList();

            var questionsInDb = _context.QuestionDatas.Where(c =>
                c.ExaminerID == examHeader.ExaminerId && c.CourseDataId == examHeader.CourseDataID).ToList();


            decimal studentResult = 0;
            foreach (var studentAnswer in studentAnswerInExam)
            {
                foreach (var question in questionsInDb)
                {
                    if (studentAnswer.QuestionID==question.QuestionDataID)
                    {
                        if (string.Equals(question.QuestionAnswer.ToLower(),studentAnswer.StudentAnswer.ToLower()))
                        {
                            studentResult++;
                        }
                        
                    }
                }
                
            }

            var examProcessModel = new ExaminationProcess()
            {
                ExaminationProcessGUID = Guid.NewGuid(),
                StudentID = studentData.StudentID,
                ExamID = examHeader.ExamId,
                StudentResult = studentResult*examHeader.MarksPerQuestion,
                CourseDataID = examHeader.CourseDataID,
                ExaminerID = examHeader.ExaminerId
            };

            _context.ExaminationProcesses.Add(examProcessModel);
            _context.SaveChanges();

            return RedirectToAction("Index", "Student");
        }


      


        public ActionResult ShowStudentResult(Guid examHeaderGuid, long examNumberOfQuestions)
        {
     
            var studentAuthentication = _context.UserAuthentications.SingleOrDefault
            (u =>
                u.Email.ToLower()
                    .Equals(User.Identity.Name));


            var studentData =
                _context.StudentDatas.SingleOrDefault(e => e.StudentID == studentAuthentication.StudentID);

           var examHeader= _context.ExamHeaders.SingleOrDefault(c => c.ExamGUID == examHeaderGuid);

           var examProcess = _context.ExaminationProcesses.SingleOrDefault(c =>
               c.ExamID == examHeader.ExamId && c.StudentID == studentData.StudentID && c.IsPublished==true);

           decimal? result = null;
           if (examProcess==null)
           {
               result = null;
           }
           else
           {
               result = examProcess.StudentResult;
           }
           
           var model = new StudentShowHisResult()
           {
               ExamHeader=examHeader,
               ExaminationProcess = examProcess,
               NumberOfQuestion =examNumberOfQuestions ,
               FinaleMark = examNumberOfQuestions*examHeader.MarksPerQuestion,
               StudentResult = result
           };
           return View(model);
        }




    }
}