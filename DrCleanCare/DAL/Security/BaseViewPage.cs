using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace DrCleanCare.DAL.Security
{
    public abstract class BaseViewPage : WebViewPage
    {
        public virtual new DrCleanCarePrinciple User
        {
            get { return base.User as DrCleanCarePrinciple; }
        }
    }
    public abstract class BaseViewPage<TModel> : WebViewPage<TModel>
    {
        public virtual new DrCleanCarePrinciple User
        {
            get { return base.User as DrCleanCarePrinciple; }
        }
    }
}