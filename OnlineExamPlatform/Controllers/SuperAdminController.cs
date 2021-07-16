using System;
using System.Data.Entity;
using System.Web.Mvc;
using OnlineExamPlatform.Authentication;
using OnlineExamPlatform.Models;
using OnlineExamPlatform.ViewModels;
using  System.Linq;
using OnlineExamPlatform.Enums;

namespace OnlineExamPlatform.Controllers
{
    [Authorize(Roles = "super admin")]
    public class SuperAdminController : Controller
    {
        private OnXamsEntities _context;

        public SuperAdminController()
        {
            _context = new OnXamsEntities();
        }

        protected override void Dispose(bool disposing)
        {
            _context.Dispose();
            
        }

        // GET: SuperAdmin
        public ActionResult Index()
        {
            var superAdminAuthentication = _context.UserAuthentications.
                SingleOrDefault(a => a.Email.Equals(User.Identity.Name));
            var superAdmin = _context.AdminDatas.SingleOrDefault(a => a.AdminID == superAdminAuthentication.AdminId);


            var adminViewModel = new AdminViewModel()
            {
                AdminData = superAdmin,
                UserAuthentication = superAdminAuthentication,


            };
            return View(adminViewModel);
        }

        public ActionResult ChangePassword()
        {
            var model = new ChangePasswordViewModel();
            return View(model);
        }

        [HttpPost]
 
        public ActionResult SavePassword(ChangePasswordViewModel model)
        {
            var superAdminAuthentication = _context.UserAuthentications
                .SingleOrDefault
                (u =>
                    u.Email.ToLower()
                        .Equals(User.Identity.Name));


            if (!ModelState.IsValid)
            {
                ModelState.AddModelError(" ", "Some Information are Invalid");
                return View("ChangePassword", model);
            }

            var checkOldPassword = HashingPassword.ValidatePassword(model.OldPassword
                , superAdminAuthentication.Password);


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
            superAdminAuthentication.Password = HashingPassword.HashPassword(model.NewPassword);
            _context.SaveChanges();
            return RedirectToAction("Index", "SuperAdmin");

        }

        public ActionResult Admin()
        {
            var admin = _context.AdminDatas.Include(e => e.UserAuthentications)
                .Where(e => e.IsDeleted != true && e.AdminRank != 0).ToList();
            return View(admin);
        }



        public ActionResult NewAdmin()
        {
            var gender = _context.Genders.ToList();
            var model = new NewAdminViewModel()
            {
                Gender = gender
            };
            return View("AdminForm", model);
        }
        public ActionResult EditAdmin(Guid adminGuid)
        {
            var adminInDb = _context.AdminDatas.SingleOrDefault(c => c.AdminGUID == adminGuid);
            var gender = _context.Genders.ToList();
            var user = _context.UserAuthentications.SingleOrDefault(u => u.AdminId == adminInDb.AdminID);
            if (adminInDb == null || user == null)
                return HttpNotFound();

            var model = new NewAdminViewModel()
            {
                AdminData = adminInDb,
                Gender = gender,
                EmailAddress = user.Email,
                Password = user.Password

            };
            return View("AdminForm",model);
        }
        public ActionResult DeleteAdmin(Guid adminGuid)
        {
            var adminInDb = _context.AdminDatas.SingleOrDefault(c => c.AdminGUID == adminGuid);
            if (adminInDb != null)
            {
                adminInDb.IsDeleted = true;
            }
            else
            {
                return HttpNotFound();
            }
            _context.SaveChanges();
            return RedirectToAction("Admin", "SuperAdmin");
        }

        public ActionResult SaveAdmin(NewAdminViewModel model )
        {

            var superAdminAuthentication = _context.UserAuthentications.SingleOrDefault
            (u =>
                u.Email.ToLower()
                    .Equals(User.Identity.Name));
            //Saving New Admin In Db

            if (model.AdminData.AdminID==0)
            {
                model.AdminData.AdminGUID = Guid.NewGuid();
                model.AdminData.AdminRank = 1;
                model.AdminData.Job = "Doctor";
                model.AdminData.CreatedDate=DateTime.Now;
                model.AdminData.CreatedBy = superAdminAuthentication.Email;
                _context.AdminDatas.Add(model.AdminData);
                _context.SaveChanges();
                var adminId = model.AdminData.AdminID;
                var userInfo = new UserAuthentication()
                {
                    AdminId = adminId,
                    Email = model.EmailAddress,
                    Password = HashingPassword.HashPassword(model.Password)
                };

                //Add userAuthentication to Database
                _context.UserAuthentications.Add(userInfo);
                _context.SaveChanges();
                var userAuthenticationId = userInfo.UserAuthenticationId;
                //Create And Add New Role Mapping User
                var role = new RolesMappingUser()
                {
                    RolesId = (int)RulesName.Admin,
                    UserAuthenticationId = userAuthenticationId
                };
                _context.RolesMappingUsers.Add(role);
                _context.SaveChanges();

            }

            //Editing Existing admin in Db
            else
            {
                var adminInDb = _context.AdminDatas
                    .SingleOrDefault(e => e.AdminID == model.AdminData.AdminID);
                var userInDb = _context.UserAuthentications
                    .SingleOrDefault(u => u.AdminId == adminInDb.AdminID);

                if (adminInDb == null || userInDb == null)
                {
                    return HttpNotFound();
                }

                adminInDb.FirstName = model.AdminData.FirstName;
                adminInDb.MiddleName = model.AdminData.MiddleName;
                adminInDb.LastName = model.AdminData.LastName;
                adminInDb.ModifiedDate=DateTime.Now;
                adminInDb.ModifiedBy = superAdminAuthentication.Email;
                userInDb.Email = model.EmailAddress;
                userInDb.Password = HashingPassword.HashPassword(model.Password);
                adminInDb.BirthDate = model.AdminData.BirthDate;
                adminInDb.GenderID = model.AdminData.GenderID;
               
                adminInDb.PhoneNumber = model.AdminData.PhoneNumber;
                _context.SaveChanges();
            }

            return RedirectToAction("Admin", "SuperAdmin");
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

    }
}