using Bruderer.Core.Domain.Models.ValueObjects;
using System;
using System.Linq;
using System.Reflection;

#nullable enable

namespace Bruderer.Core.Domain.Attributes.Resolver
{
    public static partial class AttributeResolver
    {
        public static UnitValue? GetEngineeringUnit(PropertyInfo propertyInfo)
        {

            if (propertyInfo.IsDefined(typeof(EngineeringUnitAttribute), false))
            {
                var attribute = propertyInfo.GetCustomAttributes(typeof(EngineeringUnitAttribute), false).FirstOrDefault() as EngineeringUnitAttribute;
                if (attribute == null)
                    return null;
                return attribute.Value;
            }

            return null;
        }

        public static UnitValue? GetEngineeringUnit(Type type)
        {

            if (type.IsDefined(typeof(EngineeringUnitAttribute), false))
            {
                var attribute = type.GetCustomAttributes(typeof(EngineeringUnitAttribute), false).FirstOrDefault() as EngineeringUnitAttribute;
                if (attribute == null)
                    return null;
                return attribute.Value;
            }

            return null;
        }
    }
}
