
using System.Web.Mvc;

namespace OnlineExamPlatform.Controllers
{
    [Authorize(Roles = "proctor")]
    public class ProctorController : Controller
    {
        // GET: Proctor
        public ActionResult Index()
        {
            return Content("Hello From Proctor");
        }
    }
}