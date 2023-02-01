using Bruderer.Core.Domain.Models.ModelVariableAggregate;
using System;

namespace Bruderer.Core.Domain.Attributes
{
    [AttributeUsage(AttributeTargets.Property)]
    public class TwinCAT3SamplingRateAttribute : Attribute
    {
        #region ctor

        public TwinCAT3SamplingRateAttribute(string modelUserName, ModelVariableSamplingRateEnumeration samplingRate)
        {
            ModelUserName = modelUserName;
            SamplingRate = samplingRate;
        }

        #endregion
        #region props

        public string ModelUserName { get; } = string.Empty;
        public ModelVariableSamplingRateEnumeration SamplingRate { get; } = ModelVariableSamplingRateEnumeration.Undefined;

        #endregion
    }
}
