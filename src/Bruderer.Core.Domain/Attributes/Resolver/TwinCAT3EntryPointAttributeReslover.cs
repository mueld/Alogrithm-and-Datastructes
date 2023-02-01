using System;
using System.Linq;
using System.Reflection;

namespace Bruderer.Core.Domain.Attributes.Resolver
{
    public static partial class AttributeResolver
    {
        public static TwinCAT3EntryPointAttribute GetTwinCAT3EntryPointAttribute(PropertyInfo propertyInfo)
        {

            if (propertyInfo.IsDefined(typeof(TwinCAT3SymbolNameAttribute), false))
            {
                return propertyInfo.GetCustomAttributes(typeof(TwinCAT3EntryPointAttribute), false).FirstOrDefault() as TwinCAT3EntryPointAttribute;
            }

            return null;
        }

        public static TwinCAT3EntryPointAttribute GetTwinCAT3EntryPointAttribute(Type type)
        {

            if (type.IsDefined(typeof(TwinCAT3SymbolNameAttribute), false))
            {
                return type.GetCustomAttributes(typeof(TwinCAT3EntryPointAttribute), false).FirstOrDefault() as TwinCAT3EntryPointAttribute;
            }

            return null;
        }
    }
}
