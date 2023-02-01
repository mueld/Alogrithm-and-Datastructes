using Bruderer.Core.Domain.Models.ModelVariableAggregate;
using Bruderer.Core.Domain.Models.ValueObjects;

namespace Bruderer.Core.Domain.Models.ModelUserAggregate
{
    public class ModelTwinCAT3Link : IModelUserLink
    {
        #region ctor

        public ModelTwinCAT3Link()
        { }

        public ModelTwinCAT3Link(ModelTwinCAT3Link source)
        {
            Name = source.Name;
            Platform = source.Platform;
            IsConnected = source.IsConnected;
            Ignore = source.Ignore;
            SamplingRate = source.SamplingRate;

            SymbolKey.Path = source.SymbolKey.Path;
            SymbolKey.Name = source.SymbolKey.Name;
        }

        #endregion

        public string Name { get; set; } = string.Empty;
        public ModelUserPlatformEnumeration Platform { get; set; } = ModelUserPlatformEnumeration.TwinCAT3;
        public bool IsConnected { get; set; } = false;
        public bool Ignore { get; set; } = false;
        public ModelVariableSamplingRateEnumeration SamplingRate { get; set; } = ModelVariableSamplingRateEnumeration.Undefined;
        public KeyValue SymbolKey { get; set; } = new();
    }
}
