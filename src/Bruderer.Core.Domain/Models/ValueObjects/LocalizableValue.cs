using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace Bruderer.Core.Domain.Models.ValueObjects
{
    public class LocalizableValue : ValueObject<LocalizableValue>
    {
        #region ctor

        public LocalizableValue() {}

        public LocalizableValue(string linkPath, string linkName, string value)
        {
            Link.Path = linkPath;
            Link.Name = linkName;
            Value = value;
        }

        public LocalizableValue(string linkPath, string linkName, string keyNamespace, string value)
        {
            Link.Path = linkPath;
            Link.Name = linkName;
            KeyNamespace = keyNamespace;
            Value = value;
        }

        #endregion
        #region props

        public KeyValue Link { get; set; } = new KeyValue();
        public string KeyNamespace { get; set; } = "system";
        public string Value { get; set; } = string.Empty;
        [NotMapped]
        public Dictionary<string, string> DynamicValueDictionary { get; set; } = new Dictionary<string, string>();

        #endregion

        #region ValueObject

        protected override bool EqualsCore(LocalizableValue other)
        {
            if (other == null)
                return false;

            return GetHashCode() == other.GetHashCode();
        }

        protected override int GetHashCodeCore()
        {
            return Link.Key.GetHashCode();
        }

        #endregion
    }
}
