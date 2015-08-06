using System.Security.Claims;
using System.Web.Http;
using ThinkTecture.Common.Identity;

namespace ThinkTecture.Api.Controller
{
    public class BaseApiController : ApiController
    {
        public AppUser CurrentUser
        {
            get { return new AppUser(User as ClaimsPrincipal); }
        }
    }
}
