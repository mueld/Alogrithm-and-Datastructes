using System;

namespace Bruderer.Core.Domain.Attributes
{
    [AttributeUsage(AttributeTargets.Property)]
    public class IsReadOnlyAttribute : Attribute
    {
        #region ctor

        public IsReadOnlyAttribute() { }

        public IsReadOnlyAttribute(bool isEnabled)
        {
            IsEnabled = isEnabled;
        }

        #endregion
        #region props

        public bool IsEnabled { get; } = true;

        #endregion
    }
}
