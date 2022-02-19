using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using OpenIddict.Abstractions;
using OpenIddict.Core;
using OpenIddict.EntityFrameworkCore.Models;
using static OpenIddict.Abstractions.OpenIddictConstants;

namespace ASPNetUserService.API
{
    public class Worker : IHostedService
    {
        private readonly IServiceProvider _serviceProvider;

        private readonly string CLIENT_ID = "tasklist";
        private readonly string CLIENT_SECRET = "846B62D0-DEF9-4215-A99D-86E6B8DAB342";

        private readonly string SCOPE_NAME = "tasklist";
        private readonly string RESOURCE_NAME = "tasklist";

        public Worker(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            using var scope = _serviceProvider.CreateScope();

            await CreateApplicationsAsync();
            await CreateScopesAsync();

            async Task CreateApplicationsAsync()
            {
                var manager = scope.ServiceProvider.GetRequiredService<OpenIddictApplicationManager<OpenIddictEntityFrameworkCoreApplication>>();
                if (await manager.FindByClientIdAsync(CLIENT_ID) == null)
                {
                    var descriptor = new OpenIddictApplicationDescriptor
                    {
                        ClientId = CLIENT_ID,
                        ClientSecret = CLIENT_SECRET,
                        Permissions =
                        {
                            Permissions.Endpoints.Introspection
                        }
                    };

                    await manager.CreateAsync(descriptor);
                }
            }

            async Task CreateScopesAsync()
            {
                var manager = scope.ServiceProvider.GetRequiredService<OpenIddictScopeManager<OpenIddictEntityFrameworkCoreScope>>();

                if (await manager.FindByNameAsync(SCOPE_NAME) == null)
                {
                    var descriptor = new OpenIddictScopeDescriptor
                    {
                        Name = SCOPE_NAME,
                        Resources =
                        {
                            RESOURCE_NAME
                        }
                    };

                    await manager.CreateAsync(descriptor);
                }
            }
        }

        public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
    }
}
