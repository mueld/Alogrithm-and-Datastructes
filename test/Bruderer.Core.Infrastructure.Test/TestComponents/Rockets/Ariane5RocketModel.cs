using Bruderer.Core.Domain.Models.ModelAggregate;
using Bruderer.Core.Domain.Models.ModelVariableAggregate;
using Bruderer.Core.Domain.Test.TestComponents.Rockets;
using Bruderer.Core.Domain.Test.TestComponents.Rockets.Components;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System.ComponentModel;

namespace Bruderer.Core.Infrastructure.Test.TestComponents.Rockets
{
    public class Ariane5RocketModel : RocketModel1, IAriane5RocketModel
    {
        #region ctor

        public Ariane5RocketModel(
            string name,
            IServiceScopeFactory serviceScopeFactory,
            IModelFactory modelFactory,
            ILoggerFactory loggerFactory)
            : base(name, serviceScopeFactory, loggerFactory)
        {
        }

        #endregion

        [DisplayName("RocketType")]
        [Description("RocketType")]
        public ModelVariable<ArianeRocketTypesEnumeration> RocketType { get; set; } = new(ArianeRocketTypesEnumeration.Ariane5);
    }
}
