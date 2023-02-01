using Bruderer.Core.Domain.Models.ModelVariableAggregate;
using Bruderer.Core.Domain.Models.ValueObjects;
using System;
using System.ComponentModel;

namespace Bruderer.Core.Domain.Models.ModelComponentContainerAggregate
{
    public class AnalogProcessModelContainer<T> : ModelComponentContainer, IAnalogProcessModelContainer
    {
        #region ctor

        public AnalogProcessModelContainer(bool hasInstrumentRange = false)
        {
            HasInstrumentRange = hasInstrumentRange;
            EURangeMinimum = new ModelVariable<T>();
            EURangeMaximum = new ModelVariable<T>();
            CurrentValue = new ModelVariable<T>();
           
            if (HasInstrumentRange)
            {
                InstrumentRangeMinimum = new ModelVariable<T>();
                InstrumentRangeMaximum = new ModelVariable<T>();
            }
        }

        #endregion
        #region props

        public string ValueTypeString { get { return typeof(T).ToString(); } set {; } }

        public  Type ValueType { get { return typeof(T); } }

        public UnitValue EngineeringUnit { get; set; } = null;

        public bool HasInstrumentRange { get; set; } = false;

        [DisplayName("InstrumentRangeMinimum")]
        [Description("The InstrumentRangeMinimum of the AnalogItemType")]
        public IModelVariable InstrumentRangeMinimum { get; } = null;

        [DisplayName("InstrumentRangeMaximum")]
        [Description("The InstrumentRangeMaximum of the AnalogItemType")]
        public IModelVariable InstrumentRangeMaximum { get; } = null;

        [DisplayName("EURangeMinimum")]
        [Description("The EURangeMinimum of the AnalogItemType")]
        public IModelVariable EURangeMinimum { get; } = null;

        [DisplayName("EURangeMaximum")]
        [Description("The EURangeMaximum of the AnalogItemType")]
        public IModelVariable EURangeMaximum { get; } =null;

        [DisplayName("CurrentValue")]
        [Description("The CurrentValue of the AnalogItemType")]
        public IModelVariable CurrentValue { get; } = null;

        #endregion
    }
}
