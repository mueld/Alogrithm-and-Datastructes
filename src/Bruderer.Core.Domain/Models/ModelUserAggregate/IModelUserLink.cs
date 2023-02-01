using Bruderer.Core.Domain.Models.ModelVariableAggregate;

namespace Bruderer.Core.Domain.Models.ModelUserAggregate
{
    public interface IModelUserLink
    {
        string Name { get; }
        ModelUserPlatformEnumeration Platform { get; }
        bool IsConnected { get; }
        bool Ignore { get; }
        ModelVariableSamplingRateEnumeration SamplingRate { get; }
    }
}
