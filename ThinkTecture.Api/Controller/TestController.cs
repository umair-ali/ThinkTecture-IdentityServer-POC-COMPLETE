using System.Web.Http;
using ThinkTecture.Api.Identity;
using ThinkTecture.Common.Identity;


namespace ThinkTecture.Api.Controller
{
    [Route("api/test")]
    public class TestController : BaseApiController
    {
        [AuthorizeClaim(ApplicationClaimTypes.CallApi, ApplicationClaimValues.Read)]
        public string Get()
        {
            return string.Format("Api Says: Hello {0}!!!", CurrentUser.Name);
        }
    }
}
