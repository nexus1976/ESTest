using ESTest.Models;
using ESTest.Services;
using Microsoft.AspNet.Identity;
using Microsoft.Owin.Security;
using System.Security.Principal;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace ESTest.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        private IAuthenticationManager AuthenticationManager { get { return HttpContext.GetOwinContext().Authentication; } }

        [SessionExpireFilter]
        public ActionResult Index()
        {
            UserToken userToken = Session["current_user"] as UserToken;
            if (userToken != null)
            {
                ViewBag.DisplayName = userToken.DisplayName;
            }
            else
            {
                return LogOff();
            }
            return View();
        }

        [AllowAnonymous]
        public ActionResult Login(string returnUrl)
        {
            if (HttpContext.User.Identity.IsAuthenticated)
            {
                if (string.IsNullOrWhiteSpace(returnUrl))
                    return Redirect("~/Home");
                else
                    return RedirectToLocal(returnUrl);
            }
            return View(new LoginModel() { ReturnUrl = returnUrl });
        }

        [AllowAnonymous]
        [HttpPost]
        public async Task<ActionResult> Login(LoginModel model, string returnUrl)
        {
            if (!ModelState.IsValid)
            {
                model.Message = "Please provide a valid user name and password";
                return View(model);
            }
            var userToken = await UserService.Instance.GetUserToken(model.Username, model.Password);
            if (userToken != null && !string.IsNullOrWhiteSpace(userToken.OAuthToken))
            {
                SessionHelper.UserTokenSet(Session, userToken);
                SessionHelper.UserDisplayNameSet(Session, userToken.DisplayName);
                FormsAuthentication.SetAuthCookie(model.Username, false);

                returnUrl = string.IsNullOrWhiteSpace(returnUrl) ? model.ReturnUrl : returnUrl;
                if (string.IsNullOrWhiteSpace(returnUrl))
                {
                    return Redirect("~/Home");
                }
                else
                {
                    return RedirectToLocal(returnUrl);
                }
            }
            else
            {
                model.Message = "Invalid user name or password.";
                return View(model);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult LogOff()
        {
            FormsAuthentication.SignOut();
            Session.Clear();
            HttpContext.User = new GenericPrincipal(new GenericIdentity(string.Empty), null);
            //AuthenticationManager.SignOut(DefaultAuthenticationTypes.ApplicationCookie);
            return RedirectToAction("Login", "Home");
        }

        private void AddErrors(IdentityResult result)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error);
            }
        }

        private ActionResult RedirectToLocal(string returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            return RedirectToAction("Login", "Home");
        }
    }
}