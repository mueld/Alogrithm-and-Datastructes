using System;

namespace Bruderer.Core.Domain.Attributes
{
    [AttributeUsage(AttributeTargets.Property)]
    public class TwinCAT3SymbolPathAttribute: Attribute
    {
        #region ctor

        public TwinCAT3SymbolPathAttribute(string modelUserName, string symbolLink)
        {
            ModelUserName = modelUserName;
            SymbolLink = symbolLink;
        }

        #endregion
        #region props

        public string ModelUserName { get; } = string.Empty;
        public string SymbolLink { get; } = string.Empty;

        #endregion
    }
}
