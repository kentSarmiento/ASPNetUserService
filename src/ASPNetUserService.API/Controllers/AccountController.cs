using System.Threading.Tasks;
using ASPNetUserService.API.DTOs;
using ASPNetUserService.Domain.Entities;
using ASPNetUserService.Domain.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ASPNetUserService.API.Controllers
{
    [Route("api/account")]
    [ApiController]
    [Authorize]
    public class AccountController : Controller
    {
        private readonly IApplicationUsersRepository _applicationUsersRepository;

        public AccountController(
            IApplicationUsersRepository applicationUsersRepository)
        {
            _applicationUsersRepository = applicationUsersRepository;
        }

        [HttpPost("register")]
        [AllowAnonymous]
        public async Task<IActionResult> Register([FromBody] UserRegistrationDTO userRegistration)
        {
            var user = await _applicationUsersRepository.FindByNameAsync(userRegistration.Email);
            if (user != null)
            {
                return StatusCode(StatusCodes.Status409Conflict);
            }

            user = new ApplicationUser
            {
                UserName = userRegistration.Email,
                Email = userRegistration.Email,
                Password = userRegistration.Password,
            };
            var result = await _applicationUsersRepository.CreateAsync(user);
            if (result == null)
            {
                return BadRequest();
            }

            return Ok();
        }
    }
}
