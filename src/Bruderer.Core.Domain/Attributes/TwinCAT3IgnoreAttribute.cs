using System;

namespace Bruderer.Core.Domain.Attributes
{
    [AttributeUsage(AttributeTargets.Property)]
    public class TwinCAT3IgnoreAttribute : Attribute
    {
        #region ctor

        public TwinCAT3IgnoreAttribute(string modelUserName) 
        {
            ModelUserName = modelUserName;
        }

        public TwinCAT3IgnoreAttribute(string modelUserName, bool isEnabled)
        {
            ModelUserName = modelUserName;
            IsEnabled = isEnabled;
        }

        #endregion
        #region props

        public string ModelUserName { get; } = string.Empty;
        public bool IsEnabled { get; } = true;

        #endregion
    }
}
