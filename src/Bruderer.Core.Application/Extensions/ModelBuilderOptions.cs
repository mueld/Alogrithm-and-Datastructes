using Bruderer.Core.Application.Interfaces;
using Microsoft.Extensions.DependencyInjection;

namespace Bruderer.Core.Application.Extensions
{
    internal class ModelBuilderOptions : IModelBuilderOptions
    {
        #region ctor

        public ModelBuilderOptions(IServiceCollection services)
        {
            ServiceCollection = services;
        }

        #endregion

        #region IModelBuilderOptions

        public IServiceCollection ServiceCollection { get; }

        #endregion
    }
}
