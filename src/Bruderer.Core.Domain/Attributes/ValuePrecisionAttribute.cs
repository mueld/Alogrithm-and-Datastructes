using System;

namespace Bruderer.Core.Domain.Attributes
{
    [AttributeUsage(AttributeTargets.Property)]
    public class ValuePrecisionAttribute : Attribute
    {
        #region ctor

        public ValuePrecisionAttribute(int valuePrecision)
        {
            ValuePrecision = valuePrecision;

        }

        #endregion
        #region props

        public int ValuePrecision { get; } = 0;

        #endregion
    }
}
