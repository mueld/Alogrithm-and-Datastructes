namespace Bruderer.Core.Domain.Models.ValueObjects
{
    public class UnitValue : ValueObject<UnitValue>
    {
        #region ctor

        public UnitValue() {}

        public UnitValue(string uneceCode, int unitID, string displayName, string description)
        {
            UNECECode = uneceCode;
            UnitID = unitID;
            DisplayName = displayName;
            Description = description;
        }

        #endregion
        #region props

        public string UNECECode { get; set; } = string.Empty;

        public int UnitID { get; set; } = -1;

        public string DisplayName { get; set; } = string.Empty;

        public string Description { get; set; } = string.Empty;

        #endregion

        #region ValueObject

        protected override bool EqualsCore(UnitValue other)
        {
            if (other == null)
                return false;

            return GetHashCode() == other.GetHashCode();
        }

        protected override int GetHashCodeCore()
        {
            return UNECECode.GetHashCode() +
                UnitID.GetHashCode() +
                DisplayName.GetHashCode() +
                Description.GetHashCode();
        }

        #endregion
    }
}
