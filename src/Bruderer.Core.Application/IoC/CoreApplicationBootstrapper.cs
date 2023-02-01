using Bruderer.Core.Application.Commands;
using Bruderer.Core.Application.Interfaces;
using Bruderer.Core.Application.Services;
using Bruderer.Core.Domain.Commands;
using Bruderer.Core.Domain.Messaging.Response;
using MediatR;
using Microsoft.Extensions.DependencyInjection;

namespace Bruderer.Core.Application.IoC
{
    public static class CoreApplicationBootstrapper
    {
        public static void RegisterServices(IServiceCollection services)
        {
            // Rgister core system service first
            services.AddSingleton<ICoreSystemService, CoreSystemService>();

            // Commands
            services.AddScoped<IRequestHandler<ComponentChangeCommand, IResponse<bool>>, CoreSystemCommandHandler>();
        }
    }
}
