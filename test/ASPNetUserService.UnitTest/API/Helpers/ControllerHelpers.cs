using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Routing;

namespace ASPNetUserService.UnitTest.API.Helpers
{
    public class ControllerHelpers
    {
        public static ControllerContext GetControllerContext(HttpContext httpContext)
        {
            return new ControllerContext(new ActionContext(httpContext, new RouteData(), new ControllerActionDescriptor()));
        }
    }
}
