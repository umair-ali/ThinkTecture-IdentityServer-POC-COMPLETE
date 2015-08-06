using System.Security.Claims;
using System.Web.Mvc;
using ThinkTecture.Common.Identity;

namespace ThinkTecture.Mvc.Identity
{
    public abstract class AppViewPage<TModel> : WebViewPage<TModel>
    {
        protected AppUser CurrentUser
        {
            get
            {
                return new AppUser(User as ClaimsPrincipal);
            }
        }
    }

    public abstract class AppViewPage : AppViewPage<dynamic>
    {
    }
}