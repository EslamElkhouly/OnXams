using System;
using System.Linq;
using System.Web.Mvc;
using OnlineExamPlatform.Models;

using System.Data.Entity;
using System.Data.Entity.Validation;
using OnlineExamPlatform.Authentication;
using OnlineExamPlatform.Enums;

namespace OnlineExamPlatform.ViewModels
{
    [Authorize(Roles = "admin")]
    public class AdminAddProctorController : Controller
    {
        private OnXamsEntities _context;

        public AdminAddProctorController()
        {
            _context = new OnXamsEntities();
        }

        protected override void Dispose(bool disposing)
        {
            _context.Dispose();
        }
       
        public ActionResult Index()
        {
            var proctor = _context.ProctorDatas
                .Include(e => e.UserAuthentications).Where(e => e.IsDeleted != true)
                .ToList();



            return View(proctor);
        }











        public ActionResult NewProctor()
        {
            var gender = _context.Genders.ToList();
            var model = new NewProctorViewModel()
            {
                Gender = gender
            };
            return View("ProctorForm", model);
        }





        public ActionResult Edit(Guid proctorGuid)
        {
            var proctorInDb = _context.ProctorDatas
                .SingleOrDefault(e => e.ProctorGUID == proctorGuid);

            var gender = _context.Genders.ToList();
            var user = _context.UserAuthentications.SingleOrDefault(u => u.ProctorId == proctorInDb.ProctorID);
            if (proctorInDb == null || user == null)
                return HttpNotFound();
            var model = new NewProctorViewModel()
            {
                ProctorData = proctorInDb,
                Gender = gender,
                EmailAddress = user.Email,
                Password = user.Password




            };
            return View("ProctorForm", model);
        }




        [ValidateAntiForgeryToken]
        [HttpPost]
        public ActionResult SaveProctor(NewProctorViewModel model)
        {
            var adminAuthentication = _context.UserAuthentications.SingleOrDefault
            (u =>
                u.Email.ToLower()
                    .Equals(User.Identity.Name));
            //Saving New Proctor In Db
            if (model.ProctorData.ProctorID == 0)
            {

                model.ProctorData.ProctorGUID = Guid.NewGuid();
                model.ProctorData.CreatedBy = adminAuthentication.Email;
                model.ProctorData.CreatedDate=DateTime.Now;
                _context.ProctorDatas.Add(model.ProctorData);
                _context.SaveChanges();

                //Get  Last ProctorId 
                //var proctorId = _context.ProctorDatas.OrderByDescending(e => e.ProctorID).First()
                //    .ProctorID;

                var proctorId = model.ProctorData.ProctorID;
                var userInfo = new UserAuthentication()
                {
                    ProctorId = proctorId,
                    Email = model.EmailAddress,
                    Password = HashingPassword.HashPassword(model.Password)
                };

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
                    RolesId = (int)RulesName.Proctor,
                    UserAuthenticationId = userAuthenticationId
                };
                _context.RolesMappingUsers.Add(role);
                try
                {
                    _context.SaveChanges();
                }
                catch (DbEntityValidationException e)
                {
                    foreach (var eve in e.EntityValidationErrors)
                    {
                        Console.WriteLine("Entity of type \"{0}\" in state \"{1}\" has the following validation errors:",
                            eve.Entry.Entity.GetType().Name, eve.Entry.State);
                        foreach (var ve in eve.ValidationErrors)
                        {
                            Console.WriteLine("- Property: \"{0}\", Error: \"{1}\"",
                                ve.PropertyName, ve.ErrorMessage);
                        }
                    }
                    throw;
                }



            }


            //Editing Existing Proctor in Db
            else
            {

                var proctorInDb = _context.ProctorDatas
                    .SingleOrDefault(e => e.ProctorID == model.ProctorData.ProctorID);
                var userInDb = _context.UserAuthentications
                    .SingleOrDefault(u => u.ProctorId == proctorInDb.ProctorID);

                if (proctorInDb == null || userInDb == null)
                {
                    return HttpNotFound();
                }
                proctorInDb.FirstName = model.ProctorData.FirstName;
                proctorInDb.MiddleName = model.ProctorData.MiddleName;
                proctorInDb.LastName = model.ProctorData.LastName;
                proctorInDb.ModifiedBy = adminAuthentication.Email;
                proctorInDb.ModifiedDate=DateTime.Now;
                userInDb.Email = model.EmailAddress;
                userInDb.Password = HashingPassword.HashPassword(model.Password);
              //  proctorInDb.ExaminationProcesses = model.ProctorData.ExaminationProcesses;
                proctorInDb.BirthDate = model.ProctorData.BirthDate;
                proctorInDb.Job = model.ProctorData.Job;
                proctorInDb.PhoneNumber = model.ProctorData.PhoneNumber;
                _context.SaveChanges();
            }

            return RedirectToAction("Index", "AdminAddProctor");
        }

        public ActionResult Delete(Guid proctorGuid)
        {
            var proctor = _context.ProctorDatas.SingleOrDefault(e => e.ProctorGUID == proctorGuid);

            if (proctor != null)
            {
                proctor.IsDeleted = true;
            }
            else
            {
                return HttpNotFound();
            }
            _context.SaveChanges();
            var user = _context.UserAuthentications.SingleOrDefault(e => e.ProctorId == proctor.ProctorID);
            if (user != null)
            {
                user.IsDeleted = true;
            }
            _context.SaveChanges();
            return RedirectToAction("Index", "AdminAddProctor");
        }







        public ActionResult CheckIfEmailAddressExistsOrNot(string emailAddress, string  userEmailAddress)
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

    }
}