using Bruderer.Core.Domain.Models.ValueObjects;

namespace Bruderer.Core.Domain.Models.ModelComponentAggregate
{
    public interface IModelLocalizationComponent
    {
        LocalizableValue Display { get; }
        LocalizableValue Description { get; }
    }
}
