using Bruderer.Core.Domain.Attributes;
using Bruderer.Core.Domain.Constants;
using Bruderer.Core.Domain.Models.ModelComponentContainerAggregate;
using Bruderer.Core.Domain.Models.ModelVariableAggregate;
using System.ComponentModel;

namespace Bruderer.Core.Domain.Test.TestComponents.Rockets.Components
{
    public class PayloadType : ModelComponentContainer
    {
        [DisplayName("CurrentWeight")]
        [Description("CurrentWeight")]
        [EngineeringUnit(UnitsEnumeration.Kilogram)]
        [IsReadOnly]
        public ModelVariable<float> CurrentWeight { get; set; } = new((float)0.0);
    }
}
