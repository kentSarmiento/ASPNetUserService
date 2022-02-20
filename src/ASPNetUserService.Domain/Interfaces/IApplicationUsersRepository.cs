using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using ASPNetUserService.Domain.Entities;

namespace ASPNetUserService.Domain.Interfaces
{
    public interface IApplicationUsersRepository
    {
        public Task<ApplicationUser> FindByNameAsync(string name);

        public Task<ApplicationUser> CreateAsync(ApplicationUser user);

        public Task<bool> CheckPasswordSignInAsync(ApplicationUser user, string password);

        public Task<bool> CanSignInAsync(ApplicationUser user);

        public Task<ApplicationUser> GetUserAsync(ClaimsPrincipal principal);

        public Task<ClaimsPrincipal> CreateUserPrincipalAsync(ApplicationUser user);
    }
}
