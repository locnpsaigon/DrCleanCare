using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Security;
using System.Web.Optimization;
using System.Web.Routing;
using DrCleanCare.DAL.Security;
using DrCleanCare.Models;
using Newtonsoft.Json;


namespace DrCleanCare
{
    // Note: For instructions on enabling IIS6 or IIS7 classic mode, 
    // visit http://go.microsoft.com/?LinkId=9394801

    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();

            WebApiConfig.Register(GlobalConfiguration.Configuration);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
            AppSettings.LoadConfigurations();

            System.Data.Entity.Database.SetInitializer<DAL.DataContext>(null);

            Application["OnlineVisitors"] = 0;
        }

        protected void Session_Start(object sender, EventArgs e)
        {
            Application.Lock();
            Application["OnlineVisitors"] = (int)Application["OnlineVisitors"] + 1;
            Application.UnLock();
        }

        protected void Session_End(object sender, EventArgs e)
        {
            Application.Lock();
            Application["OnlineVisitors"] = (int)Application["OnlineVisitors"] - 1;
            Application.UnLock();
        }

        protected void Application_PostAuthenticateRequest()
        {
            HttpCookie authCookie = Request.Cookies[FormsAuthentication.FormsCookieName];
            if (authCookie != null)
            {
                FormsAuthenticationTicket authTicket = FormsAuthentication.Decrypt(authCookie.Value);
                DrCleanCarePrincipalSerializeModel serializeModel = JsonConvert.DeserializeObject<DrCleanCarePrincipalSerializeModel>(authTicket.UserData);
                DrCleanCarePrinciple newUser = new DrCleanCarePrinciple(authTicket.Name);
                newUser.UserId = serializeModel.UserId;
                newUser.FirstName = serializeModel.FirstName;
                newUser.LastName = serializeModel.LastName;
                newUser.Roles = serializeModel.Roles;
                HttpContext.Current.User = newUser;
            }
        }
    }
}