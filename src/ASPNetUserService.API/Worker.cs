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
        private const string ClientId = "tasklist";
        private const string ClientSecret = "846B62D0-DEF9-4215-A99D-86E6B8DAB342";
        private const string ScopeName = "tasklist";
        private const string ResourceName = "tasklist";

        private readonly IServiceProvider _serviceProvider;

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
                if (await manager.FindByClientIdAsync(ClientId) == null)
                {
                    var descriptor = new OpenIddictApplicationDescriptor
                    {
                        ClientId = ClientId,
                        ClientSecret = ClientSecret,
                        Permissions =
                        {
                            Permissions.Endpoints.Introspection,
                        },
                    };

                    await manager.CreateAsync(descriptor);
                }
            }

            async Task CreateScopesAsync()
            {
                var manager = scope.ServiceProvider.GetRequiredService<OpenIddictScopeManager<OpenIddictEntityFrameworkCoreScope>>();

                if (await manager.FindByNameAsync(ScopeName) == null)
                {
                    var descriptor = new OpenIddictScopeDescriptor
                    {
                        Name = ScopeName,
                        Resources =
                        {
                            ResourceName,
                        },
                    };

                    await manager.CreateAsync(descriptor);
                }
            }
        }

        public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
    }
}
