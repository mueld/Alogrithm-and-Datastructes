using System;
using System.Linq;
using System.Reflection;

namespace Bruderer.Core.Domain.Attributes.Resolver
{
    public static partial class AttributeResolver
    {
        public static TwinCAT3SymbolPathAttribute GetTwinCAT3SymbolLinkAttribute(PropertyInfo propertyInfo)
        {

            if (propertyInfo.IsDefined(typeof(TwinCAT3SymbolPathAttribute), false))
            {
                return propertyInfo.GetCustomAttributes(typeof(TwinCAT3SymbolPathAttribute), false).FirstOrDefault() as TwinCAT3SymbolPathAttribute;
            }


            return null;
        }

        public static TwinCAT3SymbolPathAttribute GetTwinCAT3SymbolLinkAttribute(Type type)
        {

            if (type.IsDefined(typeof(TwinCAT3SymbolPathAttribute), false))
            {
                return type.GetCustomAttributes(typeof(TwinCAT3SymbolPathAttribute), false).FirstOrDefault() as TwinCAT3SymbolPathAttribute;
            }

            return null;
        }
    }
}
