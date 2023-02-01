using Bruderer.Core.Domain.Models.ModelVariableAggregate;

namespace Bruderer.Core.Domain.Models.ModelComponentContainerAggregate
{
    public interface IAnalogConfigurationModelContainer : IModelComponentContainer
    {
        ModelVariable<short> x0 { get; set; }
        ModelVariable<short> x1 { get; set; }
        ModelVariable<short> ValidMin { get; set; }
        ModelVariable<short> ValidMax { get; set; }
        ModelVariable<float> y0 { get; set; }
        ModelVariable<float> y1 { get; set; }
    }
}
