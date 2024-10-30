using DrCleanCare.DAL;
using DrCleanCare.DAL.Security;
using System;
using System.Web.Mvc;

namespace DrCleanCare.Controllers.Admin
{
    public class AdminController : BaseController
    {

        DataContext db = new DataContext();

        //
        // GET: /Admin/
        [Authorize]
        public ActionResult Index()
        {
            return View();
        }

        [Authorize]
        public ActionResult Credit()
        {
            return View();
        }

        [Authorize]
        [DrCleanCareAuthorize(Roles = "Administrators")]
        public ActionResult UpdateDB()
        {
            try
            {
                var scritps = System.IO.File.ReadAllText(Server.MapPath("~/App_Data/update_scripts.sql"));
                db.Database.ExecuteSqlCommand(scritps);
                ViewBag.Message = "Your web is update to date!";
            }
            catch (Exception ex)
            {
                ViewBag.Message = ex.Message;
            }

            return View();
        }

        [AllowAnonymous]
        public ActionResult ErrorMessage(string message)
        {
            ViewBag.ErrorMessage = message;
            return View();
        }
    }
}
