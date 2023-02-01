using System;

namespace Bruderer.Core.Domain.Attributes
{
    [AttributeUsage(AttributeTargets.Property)]
    public class ModelPathAttribute : Attribute
    {
        #region ctor

        public ModelPathAttribute(string linkPath)
        {
            LinkPath = linkPath;
        }

        #endregion
        #region props

        public string LinkPath { get; } = string.Empty;

        #endregion
    }
}
