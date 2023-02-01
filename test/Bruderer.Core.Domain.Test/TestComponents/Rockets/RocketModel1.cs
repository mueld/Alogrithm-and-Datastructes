using Bruderer.Core.Domain.Models.ModelAggregate;
using Bruderer.Core.Domain.Test.TestComponents.Rockets.Components;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System.ComponentModel;
using System.Threading;
using System.Threading.Tasks;

namespace Bruderer.Core.Domain.Test.TestComponents.Rockets
{
    public class RocketModel1 : Core.Domain.Models.ModelAggregate.Model, IRocketModel1
    {
        #region ctor

        public RocketModel1(
            string name,
            IServiceScopeFactory servcieScopeFactory,
            ILoggerFactory loggerFactory)
            : base(name, servcieScopeFactory, loggerFactory)
        {
        }

        #endregion

        #region Model

        public override async Task StartAsync(CancellationToken cancellationToken)
        {
            await SetState(ModelStateEnumeration.Started, cancellationToken);
        }

        public override async Task StopAsync(CancellationToken cancellationToken)
        {
            await SetState(ModelStateEnumeration.Stopped, cancellationToken);
        }

        protected override async Task SetState(ModelStateEnumeration newState, CancellationToken cancellationToken)
        {
            await base.SetState(newState, cancellationToken);
        }

        #endregion

        #region IRocketModel1

        [DisplayName("Oberer Verbund")]
        [Description("Check this -> https://www.esa.int/Enabling_Support/Space_Transportation/Launch_vehicles/Ariane_5_ES.")]
        public UpperCompositeService UpperCompositeService { get; set; } = new();

        [DisplayName("Unterer Verbund")]
        [Description("Check this -> https://www.esa.int/Enabling_Support/Space_Transportation/Launch_vehicles/Ariane_5_ES.")]
        public LowerCompositeService LowerCompositeService { get; set; } = new();

        #endregion
    }
}
