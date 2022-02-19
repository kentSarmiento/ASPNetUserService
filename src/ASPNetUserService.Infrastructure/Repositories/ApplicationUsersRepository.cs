using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using ASPNetUserService.Domain.Interfaces;
using ASPNetUserService.Domain.Entities;
using Microsoft.AspNetCore.Identity;

namespace ASPNetUserService.Infrastructure.Repositories
{
    public class ApplicationUsersRepository : IApplicationUsersRepository
    {
        private readonly UserManager<AppUserIdentity> _userManager;
        private readonly SignInManager<AppUserIdentity> _signInManager;

        public ApplicationUsersRepository(
            UserManager<AppUserIdentity> userManager,
            SignInManager<AppUserIdentity> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }

        public async Task<ApplicationUser> FindByNameAsync(string name)
        {
            var userIdentity = await _userManager.FindByNameAsync(name);
            if (userIdentity != null)
            {
                var user = new ApplicationUser
                {
                    UserName = userIdentity.UserName,
                    Email = userIdentity.Email
                };

                return user;
            }

            return null;
        }

        public async Task<ApplicationUser> CreateAsync(ApplicationUser user)
        {
            var userIdentity = new AppUserIdentity
            {
                UserName = user.UserName,
                Email = user.Email
            };

            var result = await _userManager.CreateAsync(userIdentity, user.Password);
            if (result.Succeeded)
            {
                return user;
            }

            return null;
        }

        public async Task<bool> CheckPasswordSignInAsync(ApplicationUser user, string password)
        {
            var userIdentity = await _userManager.FindByNameAsync(user.UserName);

            var result = await _signInManager.CheckPasswordSignInAsync(userIdentity, password, lockoutOnFailure: true);
            return result.Succeeded;
        }

        public async Task<ClaimsPrincipal> CreateUserPrincipalAsync(ApplicationUser user)
        {
            var userIdentity = await _userManager.FindByNameAsync(user.UserName);

            var principal = await _signInManager.CreateUserPrincipalAsync(userIdentity);
            return principal;
        }

    }
}
