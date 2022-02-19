using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OpenIddict.Validation.AspNetCore;

namespace ASPNetUserService.API.Controllers
{
    [Route("api")]
    [Authorize(AuthenticationSchemes = OpenIddictValidationAspNetCoreDefaults.AuthenticationScheme)]
    public class ResourceController : Controller
    {
        [HttpGet("message")]
        public IActionResult GetMessage()
        {
            return Content($"{User.Identity.Name} has been successfully authenticated.");
        }
    }
}