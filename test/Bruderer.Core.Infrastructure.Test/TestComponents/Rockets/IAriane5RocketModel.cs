using Bruderer.Core.Domain.Models.ModelVariableAggregate;
using Bruderer.Core.Domain.Test.TestComponents.Rockets;
using Bruderer.Core.Domain.Test.TestComponents.Rockets.Components;

namespace Bruderer.Core.Infrastructure.Test.TestComponents.Rockets
{
    public interface IAriane5RocketModel : IRocketModel1
    {
        ModelVariable<ArianeRocketTypesEnumeration> RocketType { get; set; }
    }
}
