using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using ThinkTecture.Api.Identity;
using ThinkTecture.Common.Identity;

namespace ThinkTecture.Api.Controller
{
    [Route("api/values")]
    public class ValuesController : BaseApiController
    {
        [AuthorizeClaim(ApplicationClaimTypes.Values, ApplicationClaimValues.Read)]
        public IEnumerable<string> Get()
        {
            var claims = CurrentUser.Claims.Select(c => c.Type + ":" + c.Value);
            return claims;
        }
    }
}
