using DrCleanCare.DAL.Security;
using System.Web.Mvc;

namespace DrCleanCare.Controllers.Admin
{
    public class BaseController : Controller
    {
        protected virtual new DrCleanCarePrinciple User
        {
            get { return HttpContext.User as DrCleanCarePrinciple; }
        }

    }
}
