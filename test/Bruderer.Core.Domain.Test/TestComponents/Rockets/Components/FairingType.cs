using Bruderer.Core.Domain.Attributes;
using Bruderer.Core.Domain.Constants;
using Bruderer.Core.Domain.Models.ModelComponentContainerAggregate;
using Bruderer.Core.Domain.Models.ModelVariableAggregate;
using System.ComponentModel;

namespace Bruderer.Core.Domain.Test.TestComponents.Rockets.Components
{
    public class FairingType : ModelComponentContainer
    {
        [DisplayName("CurrentFrontTemperature")]
        [Description("CurrentFrontTemperature")]
        [EngineeringUnit(UnitsEnumeration.DegreeCelsius)]
        public ModelVariable<float> CurrentFrontTemperature { get; set; } = new((float)0.0);
    }
}
