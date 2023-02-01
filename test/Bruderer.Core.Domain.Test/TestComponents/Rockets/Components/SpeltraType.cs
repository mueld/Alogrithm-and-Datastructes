using Bruderer.Core.Domain.Models.ModelComponentContainerAggregate;
using System.ComponentModel;

namespace Bruderer.Core.Domain.Test.TestComponents.Rockets.Components
{
    public class SpeltraType : ModelComponentContainer
    {
        [DisplayName("DryAirSensorConfiguration")]
        [Description("DryAirSensorConfiguration")]
        public virtual AnalogConfigurationModelContainer DryAirSensorConfiguration { get; set; } = new();
    }
}
