using Bruderer.Core.Domain.Models.ModelComponentAggregate;

namespace Bruderer.Core.Domain.Models.ModelVariableAggregate
{
    public interface IModelVariableDEO
    {
        IModelContext ModelContext { get; set; }
        IModelRepositoryComponent RepositoryContext { get; set; }
        ModelVariablePayload Payload { get; set; }

        bool HasReference { get; }
        void SetReference(IModelVariable variable);
    }
}
