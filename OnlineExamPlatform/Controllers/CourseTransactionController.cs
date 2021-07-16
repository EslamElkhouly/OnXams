using System;
using System.Linq;
using System.Web.Mvc;
using OnlineExamPlatform.Models;
using OnlineExamPlatform.ViewModels;


namespace OnlineExamPlatform.Controllers
{
    public class CourseTransactionController : Controller
    {
        private OnXamsEntities _context;

        public CourseTransactionController()
        {
            _context = new OnXamsEntities();

        }

        protected override void Dispose(bool disposing)
        {
            _context.Dispose();
        }

        // GET: CourseTransaction
        //public ActionResult Index(string option, string search, int? pageNumber, string sort)
        //{
        //    ViewBag.SortByName = string.IsNullOrWhiteSpace(sort) ? "descending name" : "";


        //    var courseTransaction = _context.CourseTransactions
        //        .Where(c => c.IsDeleted == null || c.IsDeleted == false)
        //        .GroupBy(p =>
        //               new { p.CourseData.CourseName, p.AcademicYear })
        //        .Select(g => g.FirstOrDefault())
        //        .AsQueryable();

        //    if (option == "Course Name")
        //    {
        //        courseTransaction = courseTransaction
        //            .Where(c =>
        //                c.CourseData.CourseName.Contains(search) || search == null);
        //    }
        //    else if (option == "Examiner Name")
        //    {
        //        courseTransaction = courseTransaction
        //            .Where(c =>
        //                c.ExaminerData.FirstName.Contains(search) || search == null);

        //    }

        //    else
        //    {
        //        courseTransaction = courseTransaction
        //            .Where(c =>
        //                c.CourseData.CourseName.Contains(search) || search == null);

        //    }

        //    switch (sort)
        //    {
        //        case "descending name":
        //            courseTransaction = courseTransaction.OrderByDescending(c => c.CourseData.CourseName);
        //            break;
        //        default:
        //            courseTransaction = courseTransaction.OrderBy(c => c.CourseData.CourseName);
        //            break;

        //    }

        //    return View(courseTransaction.ToList().ToPagedList(pageNumber ?? 1, 20));
        //}

        public ActionResult Index()
        {
            var courseTransaction = _context.CourseTransactions
                .Where(c => c.IsDeleted == null || c.IsDeleted == false)
                .GroupBy(p =>
                    new { p.CourseData.CourseName, p.AcademicYear })
                .Select(g => g.FirstOrDefault())
                ;

            return View(courseTransaction);
        }






        [HttpGet]
        public ActionResult GetCourseByIds(int departmentId,int semesterId,int gradeId)
        {
          var course=  _context.CourseDatas.Where(c => c.DepartmentId == departmentId
                                            && c.SemesterId == semesterId
                                            && c.GradeYearID == gradeId).ToList();
          SelectList courseObj = new SelectList(course, "CourseDataID", "CourseName", 0);
            return Json(courseObj,JsonRequestBehavior.AllowGet);
        }


        [HttpGet]
        public ActionResult GetStudentByIds(int departmentId, int gradeId)
        {
            _context.Configuration.ProxyCreationEnabled = false;
            var student = _context.StudentDatas.Where(c => c.DepartmentId == departmentId
                                                           && c.GradeYearID == gradeId);
            student = student.Where(s => s.IsDeleted == false || s.IsDeleted == null);
            return Json(student, JsonRequestBehavior.AllowGet);
        }




        public ActionResult NewCourseTransaction()
        {
            var department = _context.Departments.ToList();
            var semester = _context.Semesters.ToList();
            var gradeYear = _context.GradeYears.ToList();
            var courseData = _context.CourseDatas
                .Where(c=>c.IsDeleted==false||c.IsDeleted==null)
                .ToList();

            var examiner = _context.ExaminerDatas.
                Where(e => e.IsDeleted == false || e.IsDeleted == null)
                .ToList();


            //***********************************
            //another solution

            //var examinersInDb = _context.ExaminerDatas.Where(e => e.IsDeleted == false || e.IsDeleted == null).Select(
            //    s => new
            //    {
            //        Text = s.FirstName + " " + s.MiddleName + " " + s.LastName,
            //        Value = s.ExaminerID
            //    }).ToList();

            //ViewBag.ExaminerList = new SelectList(examinersInDb, "Value", "Text");
            ////***********************************

            var model = new CourseTransactionViewModel()
            {
                Department = department,
                Semester = semester,
                GradeYear = gradeYear,
                CourseData = courseData,
                ExaminerData = examiner
            };

            return View("CourseTransactionForm", model);


          
        }



        
     




        /*################### Edit ###################*/
        public ActionResult Edit(Guid guid)
        {
            var courseTransactionInDb = _context.CourseTransactions
                .SingleOrDefault(c => c.CourseTransactionGUID == guid);
            if (courseTransactionInDb == null)
            {
                return HttpNotFound();
            }

            var courseInDb =
                _context.CourseDatas.SingleOrDefault(
                    c => c.CourseDataID == 
                         courseTransactionInDb.CourseDataID);
            if (courseInDb==null)
            {
                return HttpNotFound();
            }
            var departmentId = (int)courseInDb.DepartmentId;
            var semesterId = (int)courseInDb.SemesterId;
            var gradeYearId = (int)courseInDb.GradeYearID;
            var courseDataId = courseInDb.CourseDataID; 
            //var course = _context.CourseDatas.ToList();
            var department = _context.Departments.ToList();
            var semester = _context.Semesters.ToList();
            var gradeYear = _context.GradeYears.ToList();
            var examiner = _context.ExaminerDatas.Where(e => e.IsDeleted == false || e.IsDeleted == null).ToList();



            var model = new CourseTransactionViewModel()
            {
                DepartmentId = departmentId,
                SemesterId = semesterId,
                GradeYearId = gradeYearId,
                CourseDataId = courseDataId,
                CourseTransaction = courseTransactionInDb,
               
                ExaminerData = examiner,
                GradeYear = gradeYear,
                Semester = semester,
                Department = department,
                AcademicYear = courseTransactionInDb.AcademicYear



            };
            return View("CourseTransactionForm", model);
        }
















        /*################### Save ###################*/
        [ValidateAntiForgeryToken]
        [HttpPost]
        public ActionResult SaveCourseTransaction(CourseTransactionViewModel model)
        {
            var adminAuthentication = _context.UserAuthentications.SingleOrDefault
            (u =>
                u.Email.ToLower()
                    .Equals(User.Identity.Name));
            var flag = ModelState.IsValid;
            if (!flag)
            {
                return View("CourseTransactionForm", model);
            }
            //New Entity
            if (model.CourseTransaction.CourseTransactionGUID == null)
            {

                var studentInDb = _context.StudentDatas.Where(s => s.IsDeleted == null || s.IsDeleted == false)
                    .Where(s => s.DepartmentId == model.DepartmentId && s.GradeYearID == model.GradeYearId);
                var totalStudentCount = studentInDb.ToList().Count();
                foreach (var studentData in studentInDb.ToList())
                {
                    model.CourseTransaction.CourseTransactionGUID =Guid.NewGuid();
                    model.CourseTransaction.CourseDataID = model.CourseDataId;
                    model.CourseTransaction.AcademicYear = (DateTime)model.AcademicYear;
                    model.CourseTransaction.CreatedBy = adminAuthentication.Email;
                    model.CourseTransaction.CreatedDate = DateTime.Now;
                    model.CourseTransaction.StudentID = studentData.StudentID;
                    model.CourseTransaction.NumberOfStudents = totalStudentCount;
                    model.CourseTransaction.ExaminerID = model.ExaminerId;
                    if ((bool)studentData.IsPassed)
                    {
                        model.CourseTransaction.StudentStatus = "passed";
                    }
                    else
                    {
                        model.CourseTransaction.StudentStatus = "Failed";
                    }
                    _context.CourseTransactions.Add(model.CourseTransaction);
                    _context.SaveChanges();

                }
            }
            else
            {
                //Modify old Entity
                var courseTransactionInDb =
                    _context.CourseTransactions.SingleOrDefault(c =>
                        c.CourseTransactionGUID == model.CourseTransaction.CourseTransactionGUID);
                if (courseTransactionInDb == null)
                {
                    return HttpNotFound();
                }

                var id= courseTransactionInDb.CourseDataID;
               var courseTransactions= _context.CourseTransactions.Where(c => c.CourseDataID == id);
               foreach (var courseTransaction in courseTransactions.ToList())
               {
                   courseTransaction.ExaminerID = model.CourseTransaction.ExaminerID;
                   courseTransaction.CourseDataID = model.CourseDataId;
                   courseTransaction.ModifiedBy = adminAuthentication.Email;
                   courseTransaction.ModifiedDate=DateTime.Now;
                   courseTransaction.AcademicYear = (DateTime)model.AcademicYear;
                   courseTransaction.ExaminerID = model.ExaminerId;
                   _context.SaveChanges();
               }
               

            }


            return RedirectToAction("Index", "CourseTransaction");
        }


        /*#############################################################################*/















        public ActionResult Delete(Guid guid)
        {
            //var courseTransaction = _context.CourseTransactions.SingleOrDefault(c => c.CourseTransactionGUID == guid);
            var courseTransactionInDb = _context.CourseTransactions.SingleOrDefault(c => c.CourseTransactionGUID == guid);

            if (courseTransactionInDb == null)
            {
                return HttpNotFound();
            }

            var courseDataId = courseTransactionInDb.CourseDataID;

            foreach (var c in _context.CourseTransactions)
            {
                if (c.CourseDataID == courseDataId)
                {
                    c.IsDeleted = true;
                }
            }


            _context.SaveChanges();

            return RedirectToAction("Index", "CourseTransaction");

        }

    }
}