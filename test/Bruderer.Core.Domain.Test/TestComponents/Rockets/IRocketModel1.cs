using Bruderer.Core.Domain.Models.ModelAggregate;
using Bruderer.Core.Domain.Models.ModelVariableAggregate;
using Bruderer.Core.Domain.Test.TestComponents.Rockets.Components;

namespace Bruderer.Core.Domain.Test.TestComponents.Rockets
{
    public interface IRocketModel1 : IModel
    {
        UpperCompositeService UpperCompositeService { get; set; }
        LowerCompositeService LowerCompositeService { get; set; }
    }
}
