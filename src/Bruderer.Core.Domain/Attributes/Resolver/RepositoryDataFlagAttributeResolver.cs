using Bruderer.Core.Domain.Models.ModelAggregate;
using System.Linq;
using System.Reflection;

namespace Bruderer.Core.Domain.Attributes.Resolver
{
    public static partial class AttributeResolver
    {
        public static ModelRepositoryDataFlags? GetRepositoryDataAttribute(PropertyInfo propertyInfo)
        {
            if (propertyInfo.IsDefined(typeof(RepositoryDataAttribute), false))
            {
                var attribute = propertyInfo.GetCustomAttributes(typeof(RepositoryDataAttribute), false).FirstOrDefault() as RepositoryDataAttribute;
                if (attribute != null)
                {
                    return attribute.RepositoryDataFlags;
                }
            }

            return null;
        }
    }
}
