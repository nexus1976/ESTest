using System.Web.Mvc;
using ESTest.Services;

namespace ESTest.Controllers
{
    [Authorize]
    public class CommentBoardController : Controller
    {
        // GET: CommentBoard
        public ActionResult Index()
        {
            var userToken = SessionHelper.UserTokenGet(Session);
            if (userToken != null)
            {
                ViewBag.UserOAuthToken = userToken.OAuthToken;
                ViewBag.UserId = userToken.UserId;
            }
            ViewBag.apiURL = RESTServiceHelper.Instance.GetSystemAPIUrl();
            return View();
        }
    }
}