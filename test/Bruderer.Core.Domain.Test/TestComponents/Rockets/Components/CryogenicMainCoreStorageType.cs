using Bruderer.Core.Domain.Attributes;
using Bruderer.Core.Domain.Constants;
using Bruderer.Core.Domain.Models.ModelComponentContainerAggregate;
using System.ComponentModel;

namespace Bruderer.Core.Domain.Test.TestComponents.Rockets.Components
{
    public class CryogenicMainCoreStorageType : ModelComponentContainer
    {
        [DisplayName("CO2Pressure")]
        [Description("CO2Pressure")]
        [ValuePrecision(2)]
        [EngineeringUnit(UnitsEnumeration.Bar)]
        [IsReadOnly]
        public AnalogProcessModelContainer<float> CO2Pressure { get; set; } = new(true);

        [DisplayName("HPressure")]
        [Description("HPressure")]
        [ValuePrecision(2)]
        [EngineeringUnit(UnitsEnumeration.Bar)]
        [IsReadOnly]
        public AnalogProcessModelContainer<float> HPressure { get; set; } = new(true);
    }
}
