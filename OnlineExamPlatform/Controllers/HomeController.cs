using System.Linq;
using System.Security.Principal;
using System.Threading;
using System.Web.Mvc;
using System.Web.Security;
using OnlineExamPlatform.Authentication;
using OnlineExamPlatform.Models;
using OnlineExamPlatform.ViewModels;


namespace OnlineExamPlatform.Controllers
{
    
    public class HomeController : Controller
    {
        private readonly OnXamsEntities _context;

        public HomeController()
        {
            _context = new OnXamsEntities();
        }

        protected override void Dispose(bool disposing)
        {
            _context.Dispose();
        }

        public ActionResult Login()
        {
            return View(new UserLogedInViewModel());
        }



        [HttpPost]
        [ValidateAntiForgeryToken] 
        public ActionResult Login(UserLogedInViewModel userLogedIn)
        {
            if (!ModelState.IsValid)
            {
                ModelState.AddModelError(string.Empty, "invalid Email or Password");
                return View("Login", userLogedIn);
            }

            if (userLogedIn.EmailAddress == null || userLogedIn.Password == null)
            {
                ModelState.AddModelError(string.Empty, "invalid Email or Password");
                return View("Login", userLogedIn);
            }


            var myUser = _context.UserAuthentications.SingleOrDefault(c =>
                c.Email.ToLower().Equals(userLogedIn.EmailAddress.ToLower())&&c.IsDeleted!=true);

            if (myUser == null)
            {
                ModelState.AddModelError(string.Empty, "invalid Email or Password");
                return View("Login", userLogedIn);

            }
            if (!HashingPassword.ValidatePassword(userLogedIn.Password, myUser.Password))
            {
                ModelState.AddModelError(string.Empty, "invalid Email or Password");
                return View("Login", userLogedIn);
            }
           

            var custom = new MyCustomRoles();
            var identity = new GenericIdentity(userLogedIn.EmailAddress);
            var principal = new GenericPrincipal(identity, custom.GetRolesForUser(userLogedIn.EmailAddress));
            HttpContext.User = principal;
            Thread.CurrentPrincipal = principal;
           
            FormsAuthentication.SetAuthCookie(userLogedIn.EmailAddress, true);

            if (HttpContext.User.Identity.IsAuthenticated)
            {
                if (User.IsInRole("admin"))
                {
                    return RedirectToAction("index", "Admin");
                }

                if (User.IsInRole("examiner"))
                {
                    return RedirectToAction("index", "Examiner");
                }

                if (User.IsInRole("proctor"))
                {
                    return RedirectToAction("index", "Proctor");
                }

                if (User.IsInRole("student"))
                {
                    return RedirectToAction("index", "Student");
                }
                if (User.IsInRole("super admin"))
                {
                    return RedirectToAction("index", "SuperAdmin");
                }


                ModelState.AddModelError(string.Empty, "invalid Email or Password");
                return View("Login", userLogedIn);
            }
            else
            {
                ModelState.AddModelError(string.Empty, "invalid Email or Password");
                return View("Login", userLogedIn);
            }

            
        }












        public ActionResult Logout()
        {
           // Response.Cookies[0].Expires.AddDays(-1);
           
            FormsAuthentication.SignOut();

            return RedirectToAction("Login", "Home");
        }
    }
}