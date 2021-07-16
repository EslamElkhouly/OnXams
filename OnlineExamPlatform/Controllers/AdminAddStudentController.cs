using System;
using System.Web.Mvc;
using OnlineExamPlatform.Models;
using System.Data.Entity;
using System.Data.Entity.Validation;
using System.Linq;
using OnlineExamPlatform.Authentication;
using OnlineExamPlatform.Enums;
using OnlineExamPlatform.ViewModels;


namespace OnlineExamPlatform.Controllers
{
    [Authorize(Roles = "admin")]
    public class AdminAddStudentController : Controller
    {
        private OnXamsEntities _context;

        public AdminAddStudentController()
        {
            _context = new OnXamsEntities();
        }




        protected override void Dispose(bool disposing)
        {
            _context.Dispose();
        }



        public ActionResult Index( )
        {
            var student = _context.StudentDatas
                .Include(e => e.UserAuthentications).Where(e => e.IsDeleted != true)
                .ToList();

           

            return View(student);

        }









        public ActionResult NewStudent()
        {
            var gender = _context.Genders.ToList();
            var department = _context.Departments.ToList();
            var gradeYear = _context.GradeYears.ToList();
            var model = new NewStudentViewModel()
            {
                Gender = gender,
                Department = department,
                GradeYear = gradeYear
            };
            return View("StudentForm", model);
        }


        public ActionResult Edit(Guid studentGuid)
        {
            var studentInDb = _context.StudentDatas
                .SingleOrDefault(e => e.StudentGUID == studentGuid);

            var gender = _context.Genders.ToList();
            var department = _context.Departments.ToList();
            var gradeYear = _context.GradeYears.ToList();
           

            var user = _context.UserAuthentications
                .SingleOrDefault(u => u.StudentID == studentInDb.StudentID);
            
            if (studentInDb == null || user == null)
            {
                return HttpNotFound();
            }
            
            var model = new NewStudentViewModel()
            {
                StudentData = studentInDb,
                EmailAddress = user.Email,
                Password = user.Password,
                Gender=gender,
                Department = department,
                GradeYear = gradeYear,
                IsStudentPassed = (bool)studentInDb.IsPassed
                

            };
            return View("StudentForm", model);
        }

        [ValidateAntiForgeryToken]
        [HttpPost]
        public ActionResult SaveStudent(NewStudentViewModel model)
        {
            var adminAuthentication = _context.UserAuthentications.SingleOrDefault
            (u =>
                u.Email.ToLower()
                    .Equals(User.Identity.Name));

            //Saving New Student In Db
            if (model.StudentData.StudentID == 0)
            {
                model.StudentData.StudentGUID = Guid.NewGuid();
                model.StudentData.IsPassed = model.IsStudentPassed;
                model.StudentData.CreatedBy = adminAuthentication.Email;
                model.StudentData.CreatedDate=DateTime.Now;
                _context.StudentDatas.Add(model.StudentData);
                try
                {
                    _context.SaveChanges();
                }
                catch (DbEntityValidationException e)
                {

                    foreach (var error in e.EntityValidationErrors)
                    {
                        Console.WriteLine("Entity of type \"{0}\" in state \"{1}\" has the following validation errors:",
                            error.Entry.Entity.GetType().Name, error.Entry.State);
                        foreach (var ve in error.ValidationErrors)
                        {
                            Console.WriteLine("- Property: \"{0}\", Error: \"{1}\"",
                                ve.PropertyName, ve.ErrorMessage);
                        }
                    }
                }
               

                //Get  Last StudentId 
                //var StudentId = _context.StudentDatas.OrderByDescending(e => e.StudentID).First()
                //    .StudentID;

                var StudentId = model.StudentData.StudentID;
                var userInfo = new UserAuthentication()
                {
                    StudentID = StudentId,
                    Email = model.EmailAddress,
                    Password = HashingPassword.HashPassword(model.Password)
                };
                //Assign FK Relation
               
                //Add userAuthentication to Database
                _context.UserAuthentications.Add(userInfo);
                _context.SaveChanges();


                //Get Last User AuthenticationID 
                //var userAuthenticationId = _context.UserAuthentications.OrderByDescending(u => u.UserAuthenticationId).First()
                //    .UserAuthenticationId;

                var userAuthenticationId = userInfo.UserAuthenticationId;
                //Create And Add New Role Mapping User
                var role = new RolesMappingUser()
                {
                    RolesId = (int)RulesName.Student,
                    UserAuthenticationId = userAuthenticationId
                };
                _context.RolesMappingUsers.Add(role);
                _context.SaveChanges();


            }


            //Editing Existing Student in Db
            else
            {

                var StudentInDb = _context.StudentDatas
                    .SingleOrDefault(e => e.StudentID == model.StudentData.StudentID);
                var userInDb = _context.UserAuthentications
                    .SingleOrDefault(u => u.StudentID == StudentInDb.StudentID);

                if (StudentInDb == null || userInDb == null)
                {
                    return HttpNotFound();
                }
                StudentInDb.FirstName = model.StudentData.FirstName;
                StudentInDb.MiddleName = model.StudentData.MiddleName;
                StudentInDb.LastName = model.StudentData.LastName;
                StudentInDb.ModifiedBy = adminAuthentication.Email;
                StudentInDb.ModifiedDate = DateTime.Now;
                StudentInDb.IsPassed = model.IsStudentPassed;
                userInDb.Email = model.EmailAddress;
                userInDb.Password = HashingPassword.HashPassword(model.Password);
                StudentInDb.StudentID = model.StudentData.StudentID;
                StudentInDb.BirthDate = model.StudentData.BirthDate;
                StudentInDb.GenderID = model.StudentData.GenderID;
              //  StudentInDb.SuccessStatus = model.StudentData.SuccessStatus;
                StudentInDb.DepartmentId = model.StudentData.DepartmentId;
                StudentInDb.GradeYearID = model.StudentData.GradeYearID;
                StudentInDb.PhoneNumber = model.StudentData.PhoneNumber;
                _context.SaveChanges();
            }

            return RedirectToAction("Index", "AdminAddStudent");
        }


        public ActionResult CheckIfEmailAddressExistsOrNot(string emailAddress, string userEmailAddress)
        {
          
            if (emailAddress == userEmailAddress)
            {
                return Json(true, JsonRequestBehavior.AllowGet);
            }
            



            var checkEmail = _context.UserAuthentications.FirstOrDefault(u => u.Email.ToLower().Equals(emailAddress.ToLower()));
            bool status;
            if (checkEmail == null)
            {
                //Email does not Exist
                status = true;
            }
            else
            {
                //Email Exists
                status = false;
            }
            return Json(status, JsonRequestBehavior.AllowGet);
        }
        
        
        
        public ActionResult Delete(Guid studentGuid)
        {
            var student = _context.StudentDatas.SingleOrDefault(e => e.StudentGUID == studentGuid);

            if (student != null)
            {
                student.IsDeleted = true;
            }
            else
            {
                return HttpNotFound();
            }
            _context.SaveChanges();
            var user = _context.UserAuthentications.SingleOrDefault(e => e.StudentID == student.StudentID);
            if (user != null)
            {
                user.IsDeleted = true;
            }
            _context.SaveChanges();
            return RedirectToAction("Index", "AdminAddStudent");
        }
    }
}