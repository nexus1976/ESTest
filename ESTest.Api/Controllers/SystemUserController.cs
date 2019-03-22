using ESTest.DAL;
using ESTest.Domain;
using Microsoft.AspNet.Identity;
using System.Web.Http;
using System.Web.Http.Cors;

namespace ESTest.Api.Controllers
{
    [Authorize]
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    public class SystemUserController : ApiController
    {
        private IQueryContext _context = null;
        public SystemUserController(IQueryContext context)
        {
            this._context = context;
        }

        public IHttpActionResult Get()
        {
            if (User == null || User.Identity == null)
                return Unauthorized();
            long currentUserId = User.Identity.GetUserId<long>();
            if (currentUserId <= 0)
                return Unauthorized();
            var systemUser = SystemUser.Get(currentUserId, this._context);
            if (systemUser == null)
                return NotFound();
            else
                return Ok(systemUser);
        }
    }
}
