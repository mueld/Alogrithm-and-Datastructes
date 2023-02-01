using Bruderer.Core.Domain.Models.ModelVariableAggregate;

namespace Bruderer.Core.Domain.Models.ModelUserAggregate
{
    public class ModelOPCUALink : IModelUserLink
    {
        #region ctor

        public ModelOPCUALink()
        { }

        public ModelOPCUALink(ModelOPCUALink source)
        {
            Name = source.Name;
            Platform = source.Platform;
            IsConnected = source.IsConnected;
            Ignore = source.Ignore;
            SamplingRate = source.SamplingRate;
        }

        #endregion

        public string Name { get; set; } = string.Empty;
        public ModelUserPlatformEnumeration Platform { get; set; } = ModelUserPlatformEnumeration.OPCUA;
        public bool IsConnected { get; set; } = false;
        public bool Ignore { get; set; } = false;
        public ModelVariableSamplingRateEnumeration SamplingRate { get; set; } = ModelVariableSamplingRateEnumeration.Undefined;
    }
}
