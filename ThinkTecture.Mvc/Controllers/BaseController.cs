using System.Security.Claims;
using System.Web.Mvc;
using ThinkTecture.Common.Identity;

namespace ThinkTecture.Mvc.Controllers
{
    public class BaseController : Controller
    {
        public AppUser CurrentUser
        {
            get { return new AppUser(User as ClaimsPrincipal); }
        }
	}
}