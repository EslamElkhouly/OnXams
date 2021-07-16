using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net.Mail;
using System.Web.Mvc;
using OnlineExamPlatform.Authentication;
using OnlineExamPlatform.Models;
using OnlineExamPlatform.ViewModels;



namespace OnlineExamPlatform.Controllers
{
    [Authorize(Roles = "examiner")]
    public class ExamController : Controller
    {
        private OnXamsEntities _context;

        public ExamController()
        {
            _context = new OnXamsEntities();
        }

        protected override void Dispose(bool disposing)
        {
            _context.Dispose();

        }



        // GET: Exam




        public ActionResult Index(Guid courseGuid)
        {

            var examinerAuthentication = _context.UserAuthentications.SingleOrDefault
            (u =>
                u.Email.ToLower()
                    .Equals(User.Identity.Name));


            var examiner =
                _context.ExaminerDatas.SingleOrDefault(e => e.ExaminerID == examinerAuthentication.ExaminerId);



            var courseData = _context.CourseDatas.SingleOrDefault(c => c.CourseDataGUID == courseGuid);


            var examHeader = _context.ExamHeaders.Where(e => e.ExaminerId == examiner.ExaminerID
                          
                                                             
                                                             && e.CourseDataID == courseData.CourseDataID).ToList();

            List<long> examHeaderIds = new List<long>();
            foreach (var header in examHeader)
            {
                examHeaderIds.Add(header.ExamId);
            }
            var examDetails = _context.ExamDetails.Where(c=>examHeaderIds.Contains((long)c.ExamID)).Include(c => c.StudentGroup).Include(c => c.QuestionData).ToList();
           
            var model = new ExamInformationViewModel()
            {
                CourseGuid = (Guid)courseData.CourseDataGUID,
                ExamHeader = examHeader,
                ExamDetail = examDetails

            };
            return View(model);


        }


        public ActionResult ActiveDeActiveExamForGroup(long examHeaderId, long studentGroupId, Guid courseGuid)
        {
            var examDetails = _context.ExamDetails.Where(c => c.ExamID == examHeaderId && c.StudentGroupID == studentGroupId);
            var examHeader = _context.ExamHeaders.SingleOrDefault(c => c.ExamId == examHeaderId);

            foreach (var eDetail in examDetails)
            {
                if ((bool)eDetail.IsActive)
                {
                    eDetail.IsActive = false;
                }
                else
                {
                    eDetail.IsActive = true;
                }
            }

            if ((bool)examHeader.IsActive)
            {
                examHeader.IsActive = false;
            }
            else
            {
                examHeader.IsActive = true;
            }
            _context.SaveChanges();

            return RedirectToAction("Index", "Exam", new { courseGuid });
        }





        public ActionResult NewExam(Guid courseGuid)
        {

            var examinerAuthentication = _context.UserAuthentications.SingleOrDefault
            (u =>
                u.Email.ToLower()
                    .Equals(User.Identity.Name));


            var examiner =
                _context.ExaminerDatas.SingleOrDefault(e => e.ExaminerID == examinerAuthentication.ExaminerId);



            var course = _context.CourseDatas.SingleOrDefault
                (c => c.CourseDataGUID == courseGuid);




            var studentGroup = _context.StudentGroups
                .Where(s => s.IsDeleted != true)
                .Where(c => c.CourseDataId == course.CourseDataID
                            && c.ExaminerId == examiner.ExaminerID).ToList();




            var questionList = _context.QuestionDatas.Where(q =>
                q.IsDeleted != true
                &&
                q.CourseDataId == course.CourseDataID
                                    &&
                q.ExaminerID == examiner.ExaminerID).ToList()

                ;


            var model = new ExamProcessViewModel()
            {
                ExaminerId = examiner.ExaminerID,
                CourseId = course.CourseDataID,
                StudentGroup = studentGroup,
                QuestionData = questionList,


            };

            return View(model);
        }




        public ActionResult EditExamForGroup(long examHeaderId, long studentGroupId, Guid courseGuid)
        {
            var examHeader = _context.ExamHeaders.SingleOrDefault(e => e.ExamId == examHeaderId);
            var examDetails = _context.ExamDetails.Where(e => e.ExamID == examHeaderId);


            var studentGroup = _context.StudentGroups
                .Where(s => s.IsDeleted != true)
                .Where(c => c.CourseDataId == examHeader.CourseDataID
                            && c.ExaminerId == examHeader.ExaminerId).ToList();


            foreach (var eDetail in examDetails)
            {
                var group = eDetail.StudentGroupID;
                foreach (var s in studentGroup)
                {
                    if (s.StudentGroupID == group)
                    {
                        s.IsChecked = true;
                    }
                }
            }


            var questionList = _context.QuestionDatas.Where(q =>
                q.IsDeleted != true
                &&
                q.CourseDataId == examHeader.CourseDataID
                &&
                q.ExaminerID == examHeader.ExaminerId).ToList();

            foreach (var eDetail in examDetails)
            {
                var questionId = (long)eDetail.QuestionID;
                foreach (var q in questionList)
                {
                    if (q.QuestionDataID == questionId)
                    {
                        q.IsChecked = true;
                    }
                }

            }
            var model = new ExamProcessViewModel()
            {
                ExaminerId = (long)examHeader.ExaminerId,
                CourseId = (long)examHeader.CourseDataID,
                StudentGroup = studentGroup,
                QuestionData = questionList,
                ExamName = examHeader.ExamName,
                PaperDescription = examHeader.ExamDescription,
                StartDate = examHeader.StartDate,
                StartTime = examHeader.StartTime,
                Duration = (int)examHeader.Duration,
                IsExamActive = (bool)examHeader.IsActive,
                MarksPerQuestion = examHeader.MarksPerQuestion,
                ExamHeader = examHeader



            };
            return View("NewExam", model);
        }



        public ActionResult ExamSummary(long examHeaderId)
        {
            var examHeaderInDb = _context.ExamHeaders.Include(c => c.CourseData).Include(c => c.ExamDetails).SingleOrDefault(c => c.ExamId == examHeaderId);
            return View(examHeaderInDb);
        }





        [HttpPost]
        public ActionResult SaveExam(ExamProcessViewModel model)
        {
            var courseGuidInDb = _context.CourseDatas.SingleOrDefault(m => m.CourseDataID == model.CourseId).CourseDataGUID;
           



            if (model.ExamHeader.ExamGUID==null)
            {
                var examHeader = new ExamHeader()
                {
                    ExamGUID = Guid.NewGuid(),
                    ExamName = model.ExamName,
                    ExamDescription = model.PaperDescription,
                    MarksPerQuestion = (int)model.MarksPerQuestion,
                    StartDate = (DateTime)model.StartDate,
                    StartTime = (TimeSpan)model.StartTime,
                    Duration = (decimal)model.Duration,
                    CourseDataID = model.CourseId,
                    ExaminerId = model.ExaminerId,
                    AcademicYear = DateTime.Now.Year.ToString() + "/" + (DateTime.Now.Year + 1).ToString(),
                    IsActive = model.IsExamActive

                };
                _context.ExamHeaders.Add(examHeader);
               
                _context.SaveChanges();
                
                var examHeaderId = examHeader.ExamId;
                if (examHeaderId == 0)
                {
                    return HttpNotFound();
                }

                //retrieve the id of previously saved examHeader

                var studentGroupsIdList = new List<long>();

                for (int i = 0; i < model.StudentGroup.Count; i++)
                {
                    if (model.StudentGroup[i].IsChecked)
                    {
                        studentGroupsIdList.Add(model.StudentGroup[i].StudentGroupID);
                    }
                }

                var questionIdList = new List<long>();
                for (int i = 0; i < model.QuestionData.Count; i++)
                {
                    if (model.QuestionData[i].IsChecked)
                    {
                        questionIdList.Add(model.QuestionData[i].QuestionDataID);
                    }
                }

                for (int i = 0; i < studentGroupsIdList.Count; i++)
                {
                    for (int j = 0; j < questionIdList.Count; j++)
                    {
                        var examDetails = new ExamDetail()
                        {
                            ExamID = examHeaderId,
                            ExamDetailsGUID = Guid.NewGuid(),
                            IsActive = model.IsExamActive,
                            StudentGroupID = studentGroupsIdList[i],
                            QuestionID = questionIdList[j],
                            NumberOfQuestions = questionIdList.Count


                        };
                        _context.ExamDetails.Add(examDetails);
                    }

                    _context.SaveChanges();
                }
                return RedirectToAction("Index", "Exam", new { courseGuid = courseGuidInDb });

            }





            // old Entity
            else
            {
                var examHeaderInDb = _context.ExamHeaders.SingleOrDefault(c => c.ExamGUID == model.ExamHeader.ExamGUID);
                if (examHeaderInDb == null)
                {
                    return HttpNotFound();
                }
                examHeaderInDb.StartDate = (DateTime)model.StartDate;
                examHeaderInDb.StartTime = (TimeSpan)model.StartTime;
                examHeaderInDb.Duration = (decimal)model.Duration;
                examHeaderInDb.ExamName = model.ExamName;
                examHeaderInDb.ExamDescription = model.PaperDescription;
                examHeaderInDb.MarksPerQuestion = (int)model.MarksPerQuestion;
                examHeaderInDb.IsActive = model.IsExamActive;

                var examDetailsInDb = _context.ExamDetails
                    .Where(c => c.ExamID == examHeaderInDb.ExamId)
                    .ToList();

                List<long> groupCheckedListId = new List<long>();
                foreach (var group in model.StudentGroup)
                {
                    if (group.IsChecked)
                    {
                        groupCheckedListId.Add(group.StudentGroupID);
                    }


                }


                List<long> questionCheckedListId = new List<long>();
                foreach (var question in model.QuestionData)
                {
                    if (question.IsChecked)
                    {
                        questionCheckedListId.Add(question.QuestionDataID);
                    }

                }

                //gets unchecked groups and questions to remove from database
                //var examDetailsToDelete = new List<long>();
                //foreach (var eDetail in examDetailsInDb)
                //{
                //    if (!questionCheckedListId.Contains((long)eDetail.QuestionID)||
                //        !groupCheckedListId.Contains((long)eDetail.StudentGroupID))
                //    {
                //        examDetailsToDelete.Add(eDetail.ExamDetailsID);
                //    }
                //}




                //delete element  from examDetails
               // var selectElementFromExamDetails = _context.ExamDetails.Where(c => examDetailsToDelete.Contains(c.ExamDetailsID));
                var selectElementFromExamDetails = _context.ExamDetails.Where(c=>c.ExamID==examHeaderInDb.ExamId);
                foreach (var examDetail in selectElementFromExamDetails)
                {
                    _context.ExamDetails.Remove(examDetail);
                }


                _context.SaveChanges();


                //add new groups and questions

                

                for (int i = 0; i < groupCheckedListId.Count; i++)
                {
                    for (int j = 0; j < questionCheckedListId.Count; j++)
                    {
                        var examDetails = new ExamDetail()
                        {
                            ExamID = examHeaderInDb.ExamId,
                            ExamDetailsGUID = Guid.NewGuid(),
                            IsActive = model.IsExamActive,
                            StudentGroupID = groupCheckedListId[i],
                            QuestionID = questionCheckedListId[j],
                            NumberOfQuestions = questionCheckedListId.Count


                        };
                        _context.ExamDetails.Add(examDetails);
                    }

                    _context.SaveChanges();
                }
               

            } 
            return RedirectToAction("Index", "Exam", new { courseGuid = courseGuidInDb });





        }





        public ActionResult StudentResult(Guid courseGuid)
        {
            var examinerAuthentication = _context.UserAuthentications.SingleOrDefault
            (u =>
                u.Email.ToLower()
                    .Equals(User.Identity.Name));


            var examiner =
                _context.ExaminerDatas.SingleOrDefault(e => e.ExaminerID == examinerAuthentication.ExaminerId);

            var courseData = _context.CourseDatas.SingleOrDefault(c => c.CourseDataGUID == courseGuid);

            var examHeader = _context.ExamHeaders.Where(c =>
                c.ExaminerId == examiner.ExaminerID 
                && c.CourseDataID == courseData.CourseDataID && c.IsActive == true).ToList();
            
            var model = new ExamResultForEaminerViewModel()
            {
                CourseName = courseData.CourseName,
                CourseCode = courseData.CourseCode,
                ExamHeader = examHeader,
                
            };
            return View(model);
        }




        


        [HttpGet]
        public ActionResult GetStudentResult(long examId)
        {
          var examProcess=  _context.ExaminationProcesses.Where(c => c.ExamID == examId).ToList();


           var studentResultInExam = examProcess.Join(_context.StudentDatas, c => c.StudentID, a => a.StudentID,
               (result,studentName)=>new
               {
                   sName=studentName.FirstName +" "+studentName.MiddleName+" "+studentName.LastName,
                   sResult=result.StudentResult
               }).ToList();

           return Json(studentResultInExam, JsonRequestBehavior.AllowGet);
           
        }

        [HttpGet]
        public ActionResult GetExamInfo(long examId)
        {
            _context.Configuration.ProxyCreationEnabled = false;
            var examHeader = _context.ExamHeaders.SingleOrDefault(c => c.ExamId == examId);
            var examDetails = _context.ExamDetails.FirstOrDefault(c => c.ExamID == examHeader.ExamId);

            return Json(new
            {
               NumberOfQuestion= examDetails.NumberOfQuestions,
               MarksPerQuestion=examDetails.ExamHeader.MarksPerQuestion,
               FinaleMark = (int)examDetails.NumberOfQuestions * (int)examDetails.ExamHeader.MarksPerQuestion
                
            }, JsonRequestBehavior.AllowGet);
        }


        [HttpPost]
        public ActionResult PushResultToStudents(long examId)
        {
           var examProcess= _context.ExaminationProcesses.Where(c => c.ExamID == examId).Include(c=>c.StudentData);
           var studentInExam = examProcess.Join(_context.UserAuthentications, c => c.StudentID, a => a.StudentID,
               (exam, student) => new
               {
                   studentResult = exam.StudentResult,
                   studentEmail = student.Email
               }).ToList();
           
            foreach (var eProcess in examProcess)
           {
               eProcess.IsPublished = true;

           }

           _context.SaveChanges();

          // **************************************
          var result = new SendEmail();
          foreach (var email in studentInExam)
          {
              result.Send(email.studentEmail,"Exam Result","Dear student ,you can now show your result through the OnXams platform ." +
                                                           "Plz,check your account");
          }
           // **************************************


           return Json(string.Format("Sucess",true));
        }

       
    }
}


