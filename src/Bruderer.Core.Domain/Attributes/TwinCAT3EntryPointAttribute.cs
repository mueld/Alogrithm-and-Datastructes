using System;

namespace Bruderer.Core.Domain.Attributes
{
    [AttributeUsage(AttributeTargets.Property)]
    public class TwinCAT3EntryPointAttribute : Attribute
    {
        #region ctor

        public TwinCAT3EntryPointAttribute(string modelUserName)
        {
            ModelUserName = modelUserName;
        }

        #endregion
        #region props

        public string ModelUserName { get; } = string.Empty;

        #endregion
    }
}
