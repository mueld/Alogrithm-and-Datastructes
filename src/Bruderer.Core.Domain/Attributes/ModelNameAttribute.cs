using System;

namespace Bruderer.Core.Domain.Attributes
{
    [AttributeUsage(AttributeTargets.Property)]
    public class ModelNameAttribute : Attribute
    {
        #region ctor

        public ModelNameAttribute(string linkName)
        {
            LinkName = linkName;
        }

        #endregion
        #region props

        public string LinkName { get; } = string.Empty;

        #endregion
    }
}
