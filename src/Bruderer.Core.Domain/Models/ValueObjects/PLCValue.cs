namespace Bruderer.Core.Domain.Models.ValueObjects
{
    public class PLCValue : ValueObject<PLCValue>
    {
        #region ctor

        public PLCValue() {}

        #endregion
        #region props

        public KeyValue Symbol = new KeyValue();
        public bool IsConnected { get; set; } = false;
        public bool Ignore { get; set; } = false;

        #endregion

        #region ValueObject

        protected override bool EqualsCore(PLCValue other)
        {
            if (other == null)
                return false;

            return GetHashCode() == other.GetHashCode();
        }

        protected override int GetHashCodeCore()
        {
            return Symbol.Key.GetHashCode();
        }

        #endregion
    }
}
