using System.Linq;
using System.Web.Mvc;
using OnlineExamPlatform.Models;
using OnlineExamPlatform.ViewModels;
using System.Data.Entity;


namespace OnlineExamPlatform.Controllers
{
    
    [Authorize(Roles = "admin")]
    public class AdminController : Controller
    {
        private OnXamsEntities _context;

        public AdminController()
        {
            _context = new OnXamsEntities();
        }

        protected override void Dispose(bool disposing)
        {
            _context.Dispose();
        }
       
        // GET: Admin

        public ActionResult Index()
        {
            var studentCount = _context.StudentDatas.Where(s=>s.IsDeleted!=true).ToList().Count;
            var proctorCount = _context.ProctorDatas.Where(s => s.IsDeleted != true).ToList().Count;
            var adminCount= _context.AdminDatas.Where(s => s.IsDeleted != true&&s.AdminRank!=0).ToList().Count; ;
            var examinerCount = _context.ExaminerDatas.Where(s => s.IsDeleted != true).ToList().Count; ;
            var log = _context.LogDatas;
            
            var getCount = new GetAdminStudentExaminerProctorCount()
            {
                StudentData = studentCount,
                ProctorData = proctorCount,
                AdminData = adminCount,
                ExaminerData = examinerCount,
                LogData = log

            };


            return View(getCount);
        }



        public ActionResult TotalExamSchedulesForAdmin()
        {
          var examDetails=  _context.ExamDetails.Include(c => c.ExamHeader).Include(c => c.StudentGroup).Where(c => c.IsActive == true);
            return View(examDetails);
        }



    }
}