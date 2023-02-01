using System;

namespace Bruderer.Core.Domain.Attributes
{
    [AttributeUsage(AttributeTargets.Property)]
    public class LocalizationNamespaceAttribute : Attribute
    {
        #region ctor

        public LocalizationNamespaceAttribute(string namespaceName)
        {
            NamespaceName = namespaceName;
        }

        #endregion
        #region props

        public string NamespaceName { get; } = string.Empty;

        #endregion
    }
}
