using System;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace DrCleanCare.DAL.Security
{
    public class DrCleanCareAuthorizeAttribute : AuthorizeAttribute
    {
        protected virtual DrCleanCarePrinciple CurrentUser
        {
            get { return HttpContext.Current.User as DrCleanCarePrinciple; }
        }

        public override void OnAuthorization(AuthorizationContext filterContext)
        {
            if (filterContext == null) { throw new ArgumentNullException("filterContext"); }

            // validate current logon user
            var currentUser = HttpContext.Current.User as DrCleanCarePrinciple;
            if (currentUser == null || currentUser.Identity.IsAuthenticated == false)
            {

                // auth fail, redirect to login page
                filterContext.Result = new HttpUnauthorizedResult();

            }
            else if (currentUser.IsInRole(Roles))
            {

                // user is authenticated and is in roles
                SetCachePolicy(filterContext);

            }
            else if (Users.Contains(currentUser.Identity.Name))
            {

                // user is in allowed users list
                SetCachePolicy(filterContext);

            }
            else
            {

                // user do not have any permissions
                filterContext.Result = new RedirectToRouteResult(
                    new RouteValueDictionary(
                        new
                        {
                            controller = "Admin",
                            action = "ErrorMessage",
                            message = "Chức năng đang bị giới hạn quyền truy cập!"
                        }));
            }

            // base.OnAuthorization(filterContext);
        }

        public void CacheValidationHandler(HttpContext context, object data, ref HttpValidationStatus validationStatus)
        {
            validationStatus = OnCacheAuthorization(new HttpContextWrapper(context));
        }

        protected void SetCachePolicy(AuthorizationContext filterContext)
        {
            // ** IMPORTANT **
            // Since we're performing authorization at the action level, the authorization code runs
            // after the output caching module. In the worst case this could allow an authorized user
            // to cause the page to be cached, then an unauthorized user would later be served the
            // cached page. We work around this by telling proxies not to cache the sensitive page,
            // then we hook our custom authorization code into the caching mechanism so that we have
            // the final say on whether a page should be served from the cache.
            HttpCachePolicyBase cachePolicy = filterContext.HttpContext.Response.Cache;
            cachePolicy.SetProxyMaxAge(new TimeSpan(0));
            cachePolicy.AddValidationCallback(CacheValidationHandler, null /* data */);
        }
    }
}