using Bruderer.Core.Domain.Attributes;
using Bruderer.Core.Domain.Constants;
using Bruderer.Core.Domain.Models.ModelComponentContainerAggregate;
using Bruderer.Core.Domain.Models.ModelVariableAggregate;
using System.ComponentModel;

namespace Bruderer.Core.Domain.Test.TestComponents.Rockets.Components
{
    public class VulcainEngineType : ModelComponentContainer
    {
        [DisplayName("MaxTemperature")]
        [Description("MaxTemperature")]
        [EngineeringUnit(UnitsEnumeration.DegreeCelsius)]
        public ModelVariable<float> MaxTemperature { get; set; } = new((float)0.0);

        [DisplayName("CurrentBoost")]
        [Description("CurrentBoost")]
        [EngineeringUnit(UnitsEnumeration.KiloNewton)]
        public ModelVariable<double> CurrentBoost { get; set; } = new(0.0);
    }
}
