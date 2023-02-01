using Bruderer.Core.Application.DTO;
using Bruderer.Core.Domain.Models.ModelAggregate;
using Bruderer.Core.Domain.Test.TestComponents.Rockets;
using Bruderer.Core.Domain.Test.TestFixtures;
using System;
using System.Collections;
using System.Linq;
using System.Threading;
using Xunit;

namespace Bruderer.Core.Application.Test
{
    public class DTOTest : IClassFixture<CoreDomainTestFixture>
    {
        #region fields

        private CoreDomainTestFixture _CoreDomainTestFixture;

        #endregion
        #region ctor

        public DTOTest(CoreDomainTestFixture testFixture)
        {
            _CoreDomainTestFixture = testFixture;
        }

        #endregion

        [Fact]
        public async void ModelVariableDTOTest()
        {
            Assert.NotNull(_CoreDomainTestFixture);
            var rocketModel1 = _CoreDomainTestFixture.GetModel<RocketModel1>();

            // Consume model data
            var consumeModelDataResult = await rocketModel1.ConsumeModelData(new ModelUpdateOptions(), CancellationToken.None);
            Assert.True(consumeModelDataResult);

            // Setup DTOs
            var modelVariableDTOs = rocketModel1.Variables
                .Select(variable => new ModelVariableDTO(variable))
                .ToList();

            foreach (var variable in rocketModel1.Variables)
            {
                // Get DTO by Id
                var affectedDTOs = modelVariableDTOs
                    .Where(dto => Guid.Parse(dto.Id) == variable.Id)
                    .ToList();

                Assert.True(affectedDTOs.Count == 1);

                // Get DTO by model link
                affectedDTOs = modelVariableDTOs
                    .Where(dto => dto.ModelLink.Key == variable.ModelLink.Key)
                    .ToList();

                Assert.True(affectedDTOs.Count == 1);

                if (affectedDTOs.Count <= 0)
                    continue;

                var affectedDTO = affectedDTOs[0];
                var affectedDTOType = affectedDTO.GetType();
                var affectedDTOProperties = affectedDTOType.GetProperties();

                var variableType = variable.GetType();
                var variableProperties = variableType.GetProperties();

                foreach (var variableProp in variableProperties)
                {
                    var affectedDTOProp = affectedDTOProperties
                        .FirstOrDefault(dtoProp => dtoProp.Name == variableProp.Name);

                    if (affectedDTOProp == null)
                    {
                        continue;
                    }

                    var affectedDTOPropValue = affectedDTOProp.GetValue(affectedDTO);
                    var variablePropValue = variableProp.GetValue(variable);

                    if (affectedDTOPropValue == null ||
                        variablePropValue == null)
                    {
                        continue;
                    }

                    if (Nullable.GetUnderlyingType(variableProp.PropertyType) == null ||
                        Nullable.GetUnderlyingType(affectedDTOProp.PropertyType) == null)
                    {
                        continue;
                    }

                    if (variableProp.PropertyType == typeof(Guid))
                    {
                        Assert.True(variablePropValue.Equals(Guid.Parse(affectedDTOPropValue as string)));
                    }
                    else if (variableProp.PropertyType.IsGenericType &&
                        variableProp.PropertyType.GetInterfaces().Contains(typeof(IEnumerable)))
                    {
                        var enumerableVariablePropValue = Enumerable.Cast<object>(variablePropValue as IEnumerable);
                        var enumerableVDTOPropValue = Enumerable.Cast<object>(affectedDTOPropValue as IEnumerable);

                        Assert.True(enumerableVariablePropValue.SequenceEqual(enumerableVDTOPropValue));
                    }
                    else if (variableProp.PropertyType == typeof(Type))
                    {
                        Assert.True((variablePropValue as Type).FullName.Equals(affectedDTOPropValue));
                    }
                    else
                    {
                        Assert.True(variablePropValue.Equals(affectedDTOPropValue));
                    }
                }
            }
        }
    }
}
