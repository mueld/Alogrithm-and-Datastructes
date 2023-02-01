using System;
using System.Linq;
using System.Reflection;

namespace Bruderer.Core.Domain.Attributes.Resolver
{
    public static partial class AttributeResolver
    {
        public static TwinCAT3SymbolNameAttribute GetTwinCAT3SymbolNameAttribute(PropertyInfo propertyInfo)
        {

            if (propertyInfo.IsDefined(typeof(TwinCAT3SymbolNameAttribute), false))
            {
                return propertyInfo.GetCustomAttributes(typeof(TwinCAT3SymbolNameAttribute), false).FirstOrDefault() as TwinCAT3SymbolNameAttribute;
            }

            return null;
        }

        public static TwinCAT3SymbolNameAttribute GetTwinCAT3SymbolNameAttribute(Type type)
        {

            if (type.IsDefined(typeof(TwinCAT3SymbolNameAttribute), false))
            {
                return type.GetCustomAttributes(typeof(TwinCAT3SymbolNameAttribute), false).FirstOrDefault() as TwinCAT3SymbolNameAttribute;
            }

            return null;
        }
    }
}
