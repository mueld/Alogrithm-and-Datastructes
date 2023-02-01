using Bruderer.Core.Domain.Models.ModelVariableAggregate;
using Bruderer.Core.Domain.Models.ValueObjects;
using System;

namespace Bruderer.Core.Domain.Models.ModelComponentContainerAggregate
{
    public interface IAnalogProcessModelContainer : IModelComponentContainer
    {
        string ValueTypeString { get; }
        public Type ValueType { get; }
        UnitValue EngineeringUnit { get; }
        public bool HasInstrumentRange { get; }
        IModelVariable InstrumentRangeMinimum { get; }
        IModelVariable InstrumentRangeMaximum { get; }
        IModelVariable EURangeMinimum { get; }
        IModelVariable EURangeMaximum { get; }
        IModelVariable CurrentValue { get; }
    }
}
