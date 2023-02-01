using System;

namespace Bruderer.Core.Domain.Attributes
{
    [AttributeUsage(AttributeTargets.Property)]
    public class TwinCAT3SymbolNameAttribute : Attribute
    {
        #region ctor

        public TwinCAT3SymbolNameAttribute(string modelUserName, string symbolName)
        {
            ModelUserName = modelUserName;
            SymbolName = symbolName;
        }

        #endregion
        #region props

        public string ModelUserName { get; } = string.Empty;
        public string SymbolName { get; } = string.Empty;

        #endregion
    }
}
