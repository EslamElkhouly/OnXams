using System;
using System.Linq;
using System.Web.Mvc;
using OnlineExamPlatform.Authentication;
using OnlineExamPlatform.Models;
using OnlineExamPlatform.ViewModels;

namespace OnlineExamPlatform.Controllers
{
    [Authorize(Roles = "admin")]
    public class AdminProfileController : Controller
    {
        private OnXamsEntities _context;

        public AdminProfileController()
        {
            _context = new OnXamsEntities();
        }

        protected override void Dispose(bool disposing)
        {
            _context.Dispose();
            
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
            var adminAuthentication = _context.UserAuthentications
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
                , adminAuthentication.Password);


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
            adminAuthentication.Password = HashingPassword.HashPassword(model.NewPassword);
            _context.SaveChanges();
            return RedirectToAction("Index", "AdminProfile");

        }


        // GET: AdminProfile
        public ActionResult Index()
        {
           var adminAuthentication= _context.UserAuthentications.
               SingleOrDefault(a => a.Email.Equals(User.Identity.Name));
           var admin = _context.AdminDatas.SingleOrDefault(a => a.AdminID == adminAuthentication.AdminId);

          
           var adminViewModel = new AdminViewModel()
           {
               AdminData = admin,
               UserAuthentication = adminAuthentication,
              
               
           };
            return View(adminViewModel);
        }




        public ActionResult Edit(Guid guid)
        {
           var admin= _context.AdminDatas.SingleOrDefault(a => a.AdminGUID == guid);
           var adminAuthentication = _context.UserAuthentications.SingleOrDefault(a => a.AdminId == admin.AdminID);
           var gender = _context.Genders.ToList();
           var adminViewModel = new AdminViewModel()
           {
               AdminData = admin,
               UserAuthentication = adminAuthentication,
               Gender = gender
           };
            return View("Edit",adminViewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SaveAdmin(AdminViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View("Edit", model);
            }

            // Start of File upload




            //End Of File Upload






            var adminInDb= _context.AdminDatas.SingleOrDefault
               (a => a.AdminGUID == model.AdminData.AdminGUID);

           var userInDb = _context.UserAuthentications.SingleOrDefault
               (a => a.AdminId == adminInDb.AdminID);

           if (adminInDb == null || userInDb == null)
           {
               return HttpNotFound();
           }

           adminInDb.FirstName = model.AdminData.FirstName;
           adminInDb.MiddleName = model.AdminData.MiddleName;
           adminInDb.LastName = model.AdminData.LastName;
           //password here
           //userInDb.Password = HashingPassword.HashPassword(model.UserAuthentication.Password);


           adminInDb.BirthDate = model.AdminData.BirthDate;

           adminInDb.GenderID = model.AdminData.GenderID;

           adminInDb.PhoneNumber = model.AdminData.PhoneNumber;

           _context.SaveChanges();
           return RedirectToAction("Index", "AdminProfile");
        }


    }
}