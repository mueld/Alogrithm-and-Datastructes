using Bruderer.Core.Domain.Models.ModelVariableAggregate;
using System.Collections.Generic;
using Xunit;

namespace Bruderer.Core.Domain.Test
{
    public class ModelVariableTest
    {
        [Fact]
        public void GenericValueTest()
        {
            ModelVariable<float> modelVariable = new((float)666.0, (float)777.0, (float)555.0);

            Assert.IsType<float>(modelVariable.GetValue());
            Assert.IsType<float>(modelVariable.GetValue<float>());
            Assert.IsType<float>(modelVariable.GetValueMaximum());
            Assert.IsType<float>(modelVariable.GetValueMaximum<float>());
            Assert.IsType<float>(modelVariable.GetValueMinimum());
            Assert.IsType<float>(modelVariable.GetValueMinimum<float>());

            object newObjectValue = 333;
            modelVariable.SetValue(newObjectValue);
            Assert.IsType<float>(modelVariable.GetValue());
            Assert.True(modelVariable.GetValue<float>() == (float)333.0);

            float newFloatValue = (float)600.666;
            modelVariable.SetValue<float>(newFloatValue);
            Assert.IsType<float>(modelVariable.GetValue());
            Assert.True(modelVariable.GetValue<float>() == (float)600.666);
        }

        [Fact]
        public void ValueEqualityTest()
        {
            // Check equality of float values
            ModelVariable<float> floatModelVariable = new((float)666.0);
            Assert.False(floatModelVariable.IsValueEqual(665.995));
            Assert.True(floatModelVariable.IsValueEqual(666.0));
            Assert.False(floatModelVariable.IsValueEqual(666.001));

            // Check equality of integer values
            ModelVariable<int> intModelVariable = new(666);
            Assert.False(intModelVariable.IsValueEqual(665));
            Assert.True(intModelVariable.IsValueEqual(666));
            Assert.False(intModelVariable.IsValueEqual(667));

            // Check equality of integer and float values
            Assert.False(intModelVariable.IsValueEqual(665.49));
            Assert.True(intModelVariable.IsValueEqual(665.5));
            Assert.True(intModelVariable.IsValueEqual(666.0));
            Assert.True(intModelVariable.IsValueEqual(666.5));
            Assert.False(intModelVariable.IsValueEqual(666.51));

            // Check equality of string values
            ModelVariable<string> stringModelVariable = new("ModelVariable");
            Assert.False(stringModelVariable.IsValueEqual("ModelVariable1"));
            Assert.True(stringModelVariable.IsValueEqual(nameof(ModelVariable<string>)));
            Assert.False(stringModelVariable.IsValueEqual(string.Empty));

            // Check equality of enumerable values
            ModelVariable<IEnumerable<double>> enumerableModelVariable = new(new List<double>() { 11.1, 12.2, 13.3, 14.4 });
            Assert.False(enumerableModelVariable.IsValueEqual(new List<double>() { 11.0, 12.2, 13.3, 14.4 }));
            Assert.True(enumerableModelVariable.IsValueEqual(new List<double>() { 11.1, 12.2, 13.3, 14.4 }));
            Assert.False(enumerableModelVariable.IsValueEqual(new List<double>() { 11.1, 12.2, 13.3, 14.4, 15.5 }));
        }
    }
}
