using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading.Tasks;

namespace Bruderer.Core.Domain.Models.ModelTrigger
{
    public interface IModelTriggerHandler
    {
        void InjectDependencies(Guid ModelId, IServiceScopeFactory serviceScopeFactory);
        Task HandleTrigger(ModelTrigger trigger);
    }
}
