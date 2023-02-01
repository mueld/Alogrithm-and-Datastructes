using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;

namespace Bruderer.Core.Domain.Models.ModelVariableAggregate
{
    public class ModelVariable<T> : ModelVariableBase, IModelVariable
    {
        #region fields
        private T? _LastValue = default;
       

        #endregion
        #region ctor

        public ModelVariable() { ; }

        public ModelVariable(T value)
        {
            Value = value;
        }

        public ModelVariable( T value, T valueMaximum, T valueMinimum)
        {
            Value = value;
            ValueMaximum = valueMaximum;
            ValueMinimum = valueMinimum;
        }

        public ModelVariable(IModelVariable variableMaximum, IModelVariable variableMinimum)
        {
            SetValueMaximum(variableMaximum);
            SetValueMinimum(variableMinimum);
        }

        public ModelVariable(T value, IModelVariable variableMaximum, IModelVariable variableMinimum)
        {
            Value = value;
            SetValueMaximum(variableMaximum);
            SetValueMinimum(variableMinimum);
        }

        #endregion
        #region props

        [JsonProperty]
        public T? Value { get; private set; } = default;
        [JsonProperty]
        public T? ValueMaximum { get; set; }
        [JsonProperty]
        public T? ValueMinimum { get; set; }

        #endregion
        #region methods

        public void SetValue(T? newValue)
        {
            if (newValue == null)
                return;

            if (IsValueEqual(newValue))
                return;

            // Save old value
            _LastValue = Value;

            // Set the new value
            Value = newValue;

            // Increase version to indicate a major change on the variable
            Version++;

            // Raise domain event
            //AddDomainEvent(new VariableChangedEvent(Id, Link.Key, LastValue, Value));

            // Raise classic .NET event
            ValueChanged?.Invoke(this, new ModelVariableChangedEventArgs(_LastValue, Value));

            // Notify observers
            NotifyObserver();
        }

        private T? ConvertToValueType(object? newValue)
        {
            T? convertedValue = default;
            if (newValue == null)
                return convertedValue;

            // Get type for the new value
            var newValueType = newValue.GetType();

            try
            {
                if (ValueType.IsEnum)
                {
                    // Convert enum type to object
                    if (newValueType == typeof(string))
                    {
                        convertedValue = (T?)Enum.Parse(ValueType, newValue as string);
                    }
                    else
                    {
                        // Default behavior. Using the Convert.ChangeType() method.
                        convertedValue = (T?)Enum.ToObject(ValueType, newValue);
                    }
                }
                else if (ValueType.IsGenericType)
                {
                    // Convert to generic enumerables. Type must be an IEnumerable<>.
                    if (ValueType.IsInterface && (ValueType.GetGenericTypeDefinition() != typeof(IEnumerable<>)))
                        throw new Exception($"Generic type [{ValueType}] of variable [{ModelLink.Key}] is not supported. Supported generic types are: {typeof(IEnumerable<>)}");

                    if (!ValueType.IsInterface && !ValueType.GetInterfaces().Any(iface =>
                        iface.IsGenericType &&
                        iface.GetGenericTypeDefinition() == typeof(IEnumerable<>)))
                        throw new Exception($"Generic type [{ValueType}] of variable [{ModelLink.Key}] is not supported. Supported generic types are: {typeof(IEnumerable<>)}");

                    var genericArgumentTypes = ValueType.GetGenericArguments();
                    if (genericArgumentTypes.Length != 1)
                    {
                        throw new Exception($"Generic type [{ValueType}] of variable [{ModelLink.Key}] has an incorrect number of arguments. Supported generic types are: {typeof(IEnumerable<>)}");
                    }

                    var genericArgumentType = genericArgumentTypes[0];
                    if (genericArgumentType == null)
                    {
                        throw new Exception($"Generic argument of type [{ValueType}] in variable [{ModelLink.Key}] is null. Supported generic types are: {typeof(IEnumerable<>)}");
                    }

                    // Check the type of the new value
                    if (newValueType == typeof(string))
                    {
                        if (string.IsNullOrEmpty(newValue as string))
                        {
                            convertedValue = default;
                            return convertedValue;
                        }

                        Type genericListType = typeof(List<>).MakeGenericType(genericArgumentType);
                        var typedList =  (IList)Activator.CreateInstance(genericListType);

                        var splittedValue = (newValue as string)
                            .Split(",");

                        foreach (var valueString in splittedValue)
                        {
                            var value = Convert.ChangeType(valueString, genericArgumentType);
                            typedList.Add(value);
                        }

                        convertedValue = (T)typedList;
                    }
                    else if (newValueType == typeof(JArray))
                    {
                        if (newValue is JArray newValueJArray)
                            convertedValue = newValueJArray.ToObject<T>();
                    }
                    else if (newValueType.IsGenericType)
                    {
                        // Generic type must be an IEnumerable<>
                        if (newValueType.IsInterface && (newValueType.GetGenericTypeDefinition() != typeof(IEnumerable<>)))
                            throw new Exception($"Generic type [{newValueType}] of new value for variable [{ModelLink.Key}] is not supported. Supported generic types are: {typeof(IEnumerable<>)}");

                        if (!newValueType.IsInterface && !newValueType.GetInterfaces().Any(iface =>
                            iface.IsGenericType &&
                            iface.GetGenericTypeDefinition() == typeof(IEnumerable<>)))
                            throw new Exception($"Generic type [{newValueType}] of new value for variable [{ModelLink.Key}] is not supported. Supported generic types are: {typeof(IEnumerable<>)}");

                        var newValueEnumerable = newValue as IEnumerable;
                        convertedValue = (T?)newValueEnumerable;
                    }
                    else if (newValueType.IsArray)
                    {
                        // Convert a default Array to an IEnumerable<>
                        var newValueEnumerable = newValue as IEnumerable;
                        convertedValue = (T?)newValueEnumerable;
                    }
                    else
                    {
                        throw new Exception($"Type not supported. New value of type [{newValueType}] cannot be converted to type [{ValueType}] on variable [{ModelLink.Key}].");
                    }
                }
                else
                {
                    // Default behavior. Using the Convert.ChangeType() method.
                    convertedValue = (T)Convert.ChangeType(newValue, ValueType);
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"New value of type [{newValueType}] cannot be converted to type [{ValueType}] on variable [{ModelLink.Key}]. {ex.Message}");
            }

            return convertedValue;
        }

        private TValue? ConvertValueToType<TValue>(T? value)
        {
            TValue? convertedValue;
            if (value == null)
                return default;

            try
            {
                if (typeof(TValue) == typeof(string))
                {
                    if (value == null)
                        convertedValue = (TValue)Convert.ChangeType(string.Empty, typeof(TValue));

                    if (ValueType.IsArray)
                    {
                        var arrayString = string.Join(",", value);
                        convertedValue = (TValue)Convert.ChangeType(arrayString, typeof(TValue));
                    }
                    else if (ValueType.IsGenericType)
                    {
                        var enumerableValue = value as IEnumerable;
                        var arrayString = string.Join(",", enumerableValue.Cast<object>());
                        convertedValue = (TValue)Convert.ChangeType(arrayString, typeof(TValue));
                    }
                    else
                        convertedValue = (TValue)Convert.ChangeType(value, typeof(TValue));
                }
                else
                {
                    // Default behavior. Using the Convert.ChangeType() method.
                    convertedValue = (TValue)Convert.ChangeType(value, typeof(TValue));
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Value of variable [{ModelLink.Key}] cannot be converted to type [{typeof(TValue)}]. {ex.Message}");
            }

            return convertedValue;
        }

        private bool IsApproximatelyEqual(double value1, double value2, double epsilon)
        {
            // If they are equal anyway, just return True.
            if (value1.Equals(value2))
                return true;

            // Handle NaN, Infinity.
            if (double.IsInfinity(value1) | double.IsNaN(value1))
                return value1.Equals(value2);
            else if (double.IsInfinity(value2) | double.IsNaN(value2))
                return value1.Equals(value2);

            return Math.Abs(value1 - value2) < epsilon;
        }

        #endregion
        #region behaviours

        public static implicit operator T?(ModelVariable<T> variable)
        {
            return variable.Value;
        }

        public static implicit operator string(ModelVariable<T> variable)
        {
            var operatorType = typeof(T);

            if (variable == null)
                return string.Empty;

            if (operatorType.IsArray)
            {
                return string.Join(",", variable);
            }
            else if (operatorType.IsGenericType)
            {
                var enumerableValue = variable as IEnumerable;
                return string.Join(",", enumerableValue.Cast<object>());
            }
            else
                return variable.ToString();
        }

        #endregion

        #region IVariable

        [NotMapped]
        public override Type ValueType { get; protected set; } = typeof(T);

        [NotMapped]
        public override string? ValueEnumerationKey
        {
            get
            {
                if (ValueType.IsEnum)
                    return ValueType.Name.ToString();

                return null;
            }
        }


        public bool IsValueEqual(object? value)
        {
            var convertedValue = ConvertToValueType(value);
            if (convertedValue == null)
            {
                return (Value == null);
            }
            else if (Value == null)
            {
                return (convertedValue == null);
            }
            else if (ValueType == typeof(float) ||
                ValueType == typeof(double))
            {
                if (convertedValue == null)
                    return (Value == null);

                // Convert values to doubles for approximately equal test
                var convertedInternValue = ConvertValueToType<double>(Value);
                var convertedOtherValue = (double)Convert.ChangeType(convertedValue, typeof(double));

                // Define epsilon
                var valuePrecision = 4;
                if (ValuePrecision != null && ValuePrecision > 0)
                    valuePrecision = (int)ValuePrecision + 1;

                return IsApproximatelyEqual(convertedInternValue, convertedOtherValue, (1 / Math.Pow(10, valuePrecision)));
            }
            else if (ValueType.IsGenericType)
            {
                // Generic enumerables must use the IEnumerable.SequenceEqual() method
                if (ValueType.GetGenericTypeDefinition() != typeof(IEnumerable<>))
                {
                    Console.WriteLine($"Variable type [{ValueType.Name}] of variable [{ModelLink.Name}] cannot be checked for equality. Type must be [{typeof(IEnumerable<>).Name}].");
                    return false;
                }

                try
                {
                    var internalValueCasted = Enumerable.Cast<object>(Value as IEnumerable);
                    var newValueCasted = Enumerable.Cast<object>(convertedValue as IEnumerable);
                    return internalValueCasted.SequenceEqual(newValueCasted);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Variable type [{ValueType.Name}] of variable [{ModelLink.Name}] cannot be checked for equality. {ex.Message}");
                    return false;
                }
            }
            else
            {
                // Default behavior. Using the object.Equals() method
                return Value.Equals(convertedValue);
            }
        }

        public object? GetConvertedValueType(object? value)
        {
            return ConvertToValueType(value);
        }


        public override object? GetValue()
        {
            return Value;
        }

        public TValue? GetValue<TValue>()
        {
            return ConvertValueToType<TValue>(Value);
        }

        public override void SetValue(object? newValue)
        {
            if (newValue == null)
                return;

            var convertedValue = ConvertToValueType(newValue);
            SetValue(convertedValue);
        }

        public void SetValue<TValue>(TValue newValue)
        {
            if (newValue == null)
                return;

            var convertedValue = ConvertToValueType(newValue);
            SetValue(convertedValue);
            //NotifyObserver();
        }

        public override object? GetValueMaximum()
        {
            return ValueMaximum;
        }

        public TValue? GetValueMaximum<TValue>()
        {
            return ConvertValueToType<TValue>(ValueMaximum);
        }

        public override void SetValueMaximum(object? newValue)
        {
            if (newValue == null)
                return;

            ValueMaximum = ConvertToValueType(newValue);

            // Increase version to indicate a major change on the variable
            Version++;
        }

        public override object? GetValueMinimum()
        {
            return ValueMinimum;
        }

        public TValue? GetValueMinimum<TValue>()
        {
            return ConvertValueToType<TValue>(ValueMinimum);
        }

        public override void SetValueMinimum(object? newValue)
        {
            if (newValue == null)
                return;

            ValueMinimum = ConvertToValueType(newValue);

            // Increase version to indicate a major change on the variable
            Version++;
        }

        public event EventHandler<ModelVariableChangedEventArgs>? ValueChanged;

        #endregion
    }
}
