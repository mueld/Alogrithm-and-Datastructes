#nullable enable

namespace Bruderer.Core.Domain.Models.ModelVariableAggregate
{
    public class ModelVariablePayload
    {
        #region fields

        protected IModelVariable? _VariableReference = null;

        private object? _ValueLocal = null;
        private object? _ValueMaximumLocal = null;
        private object? _ValueMinimumLocal = null;

        #endregion
        #region ctor

        public ModelVariablePayload()
        { }

        public ModelVariablePayload(IModelVariable? variable, bool isReference = true)
        {
            if (variable == null)
                return;

            if (isReference)
            {
                _VariableReference = variable;
                return;
            }

            _ValueLocal = variable.GetValue();
            _ValueMaximumLocal = variable.GetValueMaximum();
            _ValueMinimumLocal = variable.GetValueMinimum();
        }

        #endregion
        #region props

        public object? Value
        {
            get
            {
                if (_VariableReference != null)
                    return _VariableReference.GetValue();


                return _ValueLocal;
            }
            set
            {
                if (_VariableReference != null)
                    _VariableReference.SetValue(value);

                _ValueLocal = value;
            }
        }

        public object? ValueMaximum
        {
            get
            {
                if (_VariableReference != null)
                    return _VariableReference.GetValueMaximum();


                return _ValueMaximumLocal;
            }
            set
            {
                if (_VariableReference != null)
                    _VariableReference.SetValueMaximum(value);

                _ValueMaximumLocal = value;
            }
        }

        public object? ValueMinimum
        {
            get
            {
                if (_VariableReference != null)
                    return _VariableReference.GetValueMinimum();


                return _ValueMinimumLocal;
            }
            set
            {
                if (_VariableReference != null)
                    _VariableReference.SetValueMinimum(value);

                _ValueMinimumLocal = value;
            }
        }

        #endregion
        #region methods

        public void SetReference(IModelVariable variable)
        {
            _VariableReference = variable;
        }

        #endregion
    }
}
