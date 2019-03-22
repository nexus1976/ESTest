using System.Security.Principal;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace ESTest
{
    public class SessionExpireFilterAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            HttpContext ctx = HttpContext.Current;

            var currentUser = ctx.Session["current_user"];
            if (currentUser == null)
            {
                if (HttpContext.Current.User.Identity.IsAuthenticated)
                {
                    FormsAuthentication.SignOut();
                    ctx.Session.Clear();
                    HttpContext.Current.User = new GenericPrincipal(new GenericIdentity(string.Empty), null);
                }
                // check if a new session id was generated
                filterContext.Result = new RedirectResult("~/Home/Login");
                return;
            }

            base.OnActionExecuting(filterContext);
        }
    }
}