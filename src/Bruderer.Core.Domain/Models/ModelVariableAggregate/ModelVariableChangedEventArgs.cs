using System;

namespace Bruderer.Core.Domain.Models.ModelVariableAggregate
{
    public class ModelVariableChangedEventArgs : EventArgs
    {
        #region ctor

        public ModelVariableChangedEventArgs(object oldValue, object newValue)
        {
            OldValue = oldValue;
            NewValue = newValue;
        }

        #endregion
        #region props

        public object OldValue { get; private set; }
        public object NewValue { get; private set; }

        #endregion
    }
}
