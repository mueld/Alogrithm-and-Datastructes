using Microsoft.Extensions.DependencyInjection;

namespace Bruderer.Core.Application.Interfaces
{
    public interface IModelBuilderOptions
    {
        IServiceCollection ServiceCollection { get; }
    }
}
