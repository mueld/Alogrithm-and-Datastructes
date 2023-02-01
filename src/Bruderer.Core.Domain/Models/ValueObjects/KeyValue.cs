using Bruderer.Core.Domain.Constants;

namespace Bruderer.Core.Domain.Models.ValueObjects
{
    public class KeyValue : ValueObject<KeyValue>
    {
        #region ctor

        public KeyValue() { }

        public KeyValue(string key)
        {
            var splittedKey = key.Split(".");
 
            Path = string.Join(Seperator, splittedKey, 0, splittedKey.Length - 1);
            Name = splittedKey[splittedKey.Length - 1];
        }

        public KeyValue(string path, string name)
        {
            Path = path;
            Name = name;
        }

        #endregion
        #region props

        public string Key
        {
            get
            {
                if (string.IsNullOrEmpty(Name))
                    return Path;

                if (string.IsNullOrEmpty(Path))
                    return Name;

                return Path + Seperator + Name;
            }
        }
        public string Path { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Seperator { get; set; } = StringConstants.Separator;

        #endregion

        #region ValueObject

        protected override bool EqualsCore(KeyValue other)
        {
            if (other == null)
                return false;

            return GetHashCode() == other.GetHashCode();
        }

        protected override int GetHashCodeCore()
        {
            return Key.GetHashCode();
        }

        #endregion

        #region Methods

        public bool ContainsKey(KeyValue other)
        {
            return ContainsSubstring(Key, other.Key, Seperator);
        }

        public bool ContainsKey(string other)
        {
            return ContainsSubstring(Key, other, Seperator);
        }

        public bool ContainsPath(KeyValue other)
        {
            return ContainsSubstring(Path, other.Path, Seperator);
        }

        public bool ContainsPath(string other)
        {
            return ContainsSubstring(Path, other, Seperator);
        }

        private bool ContainsSubstring(string key, string substring, string seperator)
        {
            var splittedSubstring = substring.Split(seperator);
            var splittedKey = key.Split(seperator);

            var contains = true;
            if (splittedSubstring.Length > splittedKey.Length)
                return false;

            for(var index = 0; index < splittedSubstring.Length; index ++)
            {
                if (!splittedKey[index].Equals(splittedSubstring[index]))
                {
                    contains = false; break;
                }
            }
            return contains;
        }

        #endregion

    }
}
