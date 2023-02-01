using Bruderer.Core.Domain.Commands;
using Bruderer.Core.Domain.Interfaces;
using Bruderer.Core.Domain.Messaging.Response;
using Bruderer.Core.Domain.Models.ModelAggregate;
using Bruderer.Core.Domain.Models.ModelRPCAggregate.Response;
using Bruderer.Core.Infrastructure.Bus;
using Bruderer.Core.Infrastructure.CommandHandlers;
using Bruderer.Core.Infrastructure.Data.EventSourcing;
using Bruderer.Core.Infrastructure.ModelContext;
using MediatR;
using Microsoft.Extensions.DependencyInjection;

namespace Bruderer.Core.Infrastructure.IoC
{
    public static class CoreInfrastructureBootstrapper
    {
        public static void RegisterServices(IServiceCollection services)
        {
            // Event sourcing
            services.AddSingleton<IEventStore, MemoryEventStore>();

            // Message Bus (Mediator)
            services.AddScoped<IMessageBus, MessageBus>();

            // Model factory
            services.AddSingleton<IModelFactory, ModelFactory>();

            // Commands
            services.AddScoped<IRequestHandler<ModelVariablesValueChangeCommand, IResponse<bool>>, ModelContextCommandHandler>();
            services.AddScoped<IRequestHandler<ModelVariablesMetadataChangeCommand, IResponse<bool>>, ModelContextCommandHandler>();
            services.AddScoped<IRequestHandler<ModelStructureChangeCommand, IResponse<bool>>, ModelContextCommandHandler>();
            services.AddScoped<IRequestHandler<ModelRpcCommand, IResponse<ModelRPCResponse>>, ModelContextCommandHandler>();
        }
    }
}
