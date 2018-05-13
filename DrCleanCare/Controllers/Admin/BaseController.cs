using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using DrCleanCare.DAL.Security;

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
