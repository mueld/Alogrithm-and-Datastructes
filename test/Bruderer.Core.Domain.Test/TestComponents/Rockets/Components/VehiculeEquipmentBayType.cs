using Bruderer.Core.Domain.Attributes;
using Bruderer.Core.Domain.Models.ModelComponentContainerAggregate;
using Bruderer.Core.Domain.Models.ModelVariableAggregate;
using System.ComponentModel;

namespace Bruderer.Core.Domain.Test.TestComponents.Rockets.Components
{
    public class VehiculeEquipmentBayType : ModelComponentContainer, IModelDependency
    {
        [DisplayName("PositionX")]
        [Description("PositionX")]
        [IsReadOnly]
        public ModelVariable<float> PositionX { get; set; } = new((float)0.0);

        [DisplayName("PositionY")]
        [Description("PositionY")]
        [IsReadOnly]
        public ModelVariable<float> PositionY { get; set; } = new((float)0.0);

        [DisplayName("PositionZ")]
        [Description("PositionZ")]
        [IsReadOnly]
        public ModelVariable<float> PositionZ { get; set; } = new((float)0.0);

   
        [Description("Nested Model Dependency")]
        public LowerCompositeService InnerModelDependecy { get; set; }  = new LowerCompositeService();
    }
}
