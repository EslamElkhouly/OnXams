using System;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using OnlineExamPlatform.Authentication;
using OnlineExamPlatform.Enums;
using OnlineExamPlatform.Models;
using OnlineExamPlatform.ViewModels;



namespace OnlineExamPlatform.Controllers
{
    [Authorize(Roles = "admin")]
    public class AdminAddExaminerController : Controller
    {
        private OnXamsEntities _context;

        public AdminAddExaminerController()
        {
            _context = new OnXamsEntities();
        }

        protected override void Dispose(bool disposing)
        {
            _context.Dispose();
        }

        // GET: AdminAddExaminer

        

        public ActionResult Index()
        {
            var examiner = _context.ExaminerDatas
                .Include(e => e.UserAuthentications)
                .Where(e => e.IsDeleted != true)
                .ToList();

            return View(examiner);
        }


















        public ActionResult NewExaminer()
        {
            var gender = _context.Genders.ToList();
            var model = new NewExaminerViewModel()
            {
                Gender = gender
            };

            return View("ExaminerForm", model);
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
            { //Email does not Exist
                status = true;
            }
            else
            { //Email Exists
                status = false;
            }
            return Json(status, JsonRequestBehavior.AllowGet);
        }





        [ValidateAntiForgeryToken]
        [HttpPost]
        public ActionResult SaveExaminer(NewExaminerViewModel model)
        {
            var adminAuthentication = _context.UserAuthentications.SingleOrDefault
            (u =>
                u.Email.ToLower()
                    .Equals(User.Identity.Name));

            //Saving New Examiner In Db
            if (model.ExaminerData.ExaminerID == 0)
            {

                model.ExaminerData.ExaminerGUID = Guid.NewGuid();



                model.ExaminerData.CreatedBy = adminAuthentication.Email;
                model.ExaminerData.CreatedDate=DateTime.Now;
                //Add Examiner Object to dataBase
                _context.ExaminerDatas.Add(model.ExaminerData);
                _context.SaveChanges();

                //Get  Last ExaminerId 
                //var examinerId = _context.ExaminerDatas.OrderByDescending(e => e.ExaminerID).First()
                //    .ExaminerID;

                var examinerId = model.ExaminerData.ExaminerID;

                //Assign FK Relation
                var userInfo = new UserAuthentication()
                {
                    ExaminerId = examinerId,
                    Email = model.EmailAddress,
                    Password = HashingPassword.HashPassword(model.Password)
                };



                //Hashing the password 

                //Add userAuthentication to Database
                _context.UserAuthentications.Add(userInfo);
                _context.SaveChanges();


                //Get Last User AuthenticationID 
                //var userAuthenticationId=_context.UserAuthentications.OrderByDescending(u => u.UserAuthenticationId).First()
                //    .UserAuthenticationId;


                var userAuthenticationId = userInfo.UserAuthenticationId;
                //Create And Add New Role Mapping User
                var role = new RolesMappingUser()
                {
                    RolesId = (int)RulesName.Examiner,
                    UserAuthenticationId = userAuthenticationId
                };
                _context.RolesMappingUsers.Add(role);
                _context.SaveChanges();


            }


            //Editing Existing Examiner in Db
            else
            {

                var examinerInDb = _context.ExaminerDatas
                    .SingleOrDefault(e => e.ExaminerID == model.ExaminerData.ExaminerID);
                var userInDb = _context.UserAuthentications
                    .SingleOrDefault(u => u.ExaminerId == examinerInDb.ExaminerID);

                if (examinerInDb == null || userInDb == null)
                {
                    return HttpNotFound();
                }
                examinerInDb.FirstName = model.ExaminerData.FirstName;
                examinerInDb.MiddleName = model.ExaminerData.MiddleName;
                examinerInDb.LastName = model.ExaminerData.LastName;
                examinerInDb.ModifiedBy = adminAuthentication.Email;
                examinerInDb.ModifiedDate=DateTime.Now;

                examinerInDb.GenderId = model.ExaminerData.GenderId;


                userInDb.Email = model.EmailAddress;
                userInDb.Password = HashingPassword.HashPassword(model.Password);
                examinerInDb.ScientificDegree = model.ExaminerData.ScientificDegree;
                examinerInDb.BirthDate = model.ExaminerData.BirthDate;
                examinerInDb.TeachingStartDate = model.ExaminerData.TeachingStartDate;
                examinerInDb.PhoneNumber = model.ExaminerData.PhoneNumber;
                _context.SaveChanges();
            }

            return RedirectToAction("Index", "AdminAddExaminer");
        }

        public ActionResult Edit(Guid examinerGuid)
        {
            var examinerInDb = _context.ExaminerDatas
                .SingleOrDefault(e => e.ExaminerGUID == examinerGuid);
            var gender = _context.Genders.ToList();
            var user = _context.UserAuthentications.SingleOrDefault(u => u.ExaminerId == examinerInDb.ExaminerID);
            if (examinerInDb == null || user == null)
                return HttpNotFound();
            var model = new NewExaminerViewModel()
            {
                ExaminerData = examinerInDb,
                Gender = gender,
                EmailAddress = user.Email,
                Password = user.Password


            };
            return View("ExaminerForm", model);
        }

        public ActionResult Delete(Guid examinerGuid)
        {
            var examiner = _context.ExaminerDatas.SingleOrDefault(e => e.ExaminerGUID == examinerGuid);

            if (examiner != null)
            {
                examiner.IsDeleted = true;
            }
            else
            {
                return HttpNotFound();
            }
            _context.SaveChanges();
            var user=_context.UserAuthentications.SingleOrDefault(e => e.ExaminerId == examiner.ExaminerID);
            if (user!=null)
            {
                user.IsDeleted = true;
            }
            _context.SaveChanges();
            return RedirectToAction("Index", "AdminAddExaminer");
        }
    }
}