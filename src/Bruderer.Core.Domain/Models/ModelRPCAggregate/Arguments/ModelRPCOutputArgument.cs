using System;

namespace Bruderer.Core.Domain.Models.ModelRPCAggregate.Arguments
{
    public class ModelRPCOutputArgument<T> : ModelRPCArgument
    {
        #region fields

        internal T _InternalArgument = default;

        #endregion
        #region props

        public static implicit operator T(ModelRPCOutputArgument<T> var)
        {
            return var._InternalArgument;
        }

        public static implicit operator string(ModelRPCOutputArgument<T> var)
        {
            return var.ToString();
        }

        public string ValueTypeString { get; } = typeof(T).ToString();

        public override bool IsArray { get; } = typeof(T).IsArray;

        public virtual T Argument
        {
            get { return _InternalArgument; }
            set
            {
                if (value != null && !_InternalArgument.Equals(value))
                {
                    _InternalArgument = value;
                }
            }
        }

        public override Type ArgumentType { get; } = typeof(T);

        #endregion
        #region methods

        public override void SetArgument(object argument)
        {
            _InternalArgument = (T)argument;
        }

        public override object GetArgument()
        {
            return _InternalArgument;
        }

        public override string ToString()
        {
            return Argument.ToString();
        }

        #endregion
    }
}
