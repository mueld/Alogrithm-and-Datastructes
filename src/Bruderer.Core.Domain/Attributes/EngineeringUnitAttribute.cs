using Bruderer.Core.Domain.Constants;
using Bruderer.Core.Domain.Models.ValueObjects;
using System;

#nullable enable

namespace Bruderer.Core.Domain.Attributes
{
    [AttributeUsage(AttributeTargets.Property)]
    public class EngineeringUnitAttribute : Attribute
    {
        #region ctor

        public EngineeringUnitAttribute(UnitsEnumeration unit)
        {
            Value = UnitConstants.GetUnitValue(unit);
        }

        #endregion
        #region props

        public UnitValue? Value { get; } = null;

        #endregion
    }
}
