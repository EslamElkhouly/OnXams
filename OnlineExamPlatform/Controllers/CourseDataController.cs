using System;
using System.Linq;
using System.Web.Mvc;
using OnlineExamPlatform.Models;
using OnlineExamPlatform.ViewModels;


namespace OnlineExamPlatform.Controllers
{
    [Authorize(Roles = "admin")]
    public class CourseDataController : Controller
    {
        private OnXamsEntities _context;

        public CourseDataController()
        {
            _context = new OnXamsEntities();
        }



        protected override void Dispose(bool disposing)
        {
            _context.Dispose();
        }



        // GET: CourseData
        //public ActionResult Index(string option, string search, int? pageNumber, string sort)
        //{
        //    ViewBag.SortByName = string.IsNullOrWhiteSpace(sort) ? "descending name" : "";
        //    var course=_context.CourseDatas.
        //        Where(c=>c.IsDeleted==null||c.IsDeleted==false).AsQueryable();
        //    //Department
            
        //    if (option == "Course Name")
        //    {
        //        course = _context.CourseDatas.Where(c =>
        //            c.IsDeleted == null || c.IsDeleted == false)
        //            .Where( c=>c.CourseName.Contains(search) || search == null);
        //    }

        //    if (option == "Department")
        //    {
        //       course = _context.CourseDatas.Where(c => c.IsDeleted == null || c.IsDeleted == false)
        //            .Where(c => c.Department.DepartmentName.Contains(search) || search == null);




        //    }


        //    switch (sort)
        //    {
        //        case "descending name":
        //            course = course.OrderByDescending(e => e.CourseName);
        //            break;
        //        default:
        //            course = course.OrderBy(e => e.CourseName);
        //            break;
        //    }

        //    return View(course.ToList().ToPagedList(pageNumber ?? 1, 20));
        //}
       
        
        public ActionResult Index()
        {
            var course = _context.CourseDatas.
                Where(c => c.IsDeleted == null || c.IsDeleted == false);
          




            return View(course);
        }







        public ActionResult NewCourse()
        {
            var semester = _context.Semesters.ToList();
            var department = _context.Departments.ToList();

            var gradeYear = _context.GradeYears.ToList();

            var model = new CourseDataViewModel()
            {
                Semester = semester,
                Department = department,
                GradeYear = gradeYear
            };
            return View("CourseForm",model);
        }



        public ActionResult Edit(Guid guid)
        {

            var course = _context.CourseDatas.SingleOrDefault(c => c.CourseDataGUID == guid);
            

            var department = _context.Departments.ToList();

            var gradeYear = _context.GradeYears.ToList();
            var semester = _context.Semesters.ToList();
            if (course == null)
            {
                return HttpNotFound();
            }

            
            var model = new CourseDataViewModel()
            {
                CourseData = course,
               
                
                Department = department,
                GradeYear = gradeYear,
               Semester = semester
                
            };
           
            return View("CourseForm",model);
        }



        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SaveCourse(CourseDataViewModel model)
        {
            var adminAuthentication = _context.UserAuthentications.SingleOrDefault
            (u =>
                u.Email.ToLower()
                    .Equals(User.Identity.Name));
            if (model.CourseData.CourseDataGUID == null) 
            {
                model.CourseData.CourseDataGUID=Guid.NewGuid();
                model.CourseData.CreatedDate=DateTime.Now;
                model.CourseData.CreatedBy = adminAuthentication.Email;
                
                

                _context.CourseDatas.Add(model.CourseData);
                _context.SaveChanges();
            }

            else
            {
                var courseInDb =
                    _context.CourseDatas.SingleOrDefault(c => c.CourseDataGUID == model.CourseData.CourseDataGUID);
                if (courseInDb==null)
                {
                    return HttpNotFound();
                }

                courseInDb.CourseName = model.CourseData.CourseName;
                courseInDb.CourseCode = model.CourseData.CourseCode;
                courseInDb.ModifiedBy = adminAuthentication.Email;
                courseInDb.ModifiedDate = DateTime.Now;
                courseInDb.DepartmentId = model.CourseData.DepartmentId;
                courseInDb.GradeYearID = model.CourseData.GradeYearID;
                courseInDb.SemesterId = model.CourseData.SemesterId;
                _context.SaveChanges();

            }

            
            return RedirectToAction("Index", "CourseData");
        }


        public ActionResult Delete(Guid guid)
        {
            var adminAuthentication = _context.UserAuthentications.SingleOrDefault
            (u =>
                u.Email.ToLower()
                    .Equals(User.Identity.Name));
            var course = _context.CourseDatas.SingleOrDefault(c => c.CourseDataGUID == guid);
            if (course!=null)
            {
                course.IsDeleted = true;
                course.ModifiedBy = adminAuthentication.Email;
                course.ModifiedDate=DateTime.Now;
                ;
            }
            else
            {
                 return HttpNotFound();
            }
            _context.SaveChanges();
            return RedirectToAction("Index","CourseData");
        }
    }
}