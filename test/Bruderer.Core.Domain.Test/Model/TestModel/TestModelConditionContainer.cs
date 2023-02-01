using Bruderer.Core.Domain.Models.ModelComponentAggregate;
using Bruderer.Core.Domain.Models.ModelConditionAggregate;
using Bruderer.Core.Domain.Models.ModelVariableAggregate;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bruderer.Core.Domain.Test.Model.TestModel
{
    public class TestCondition : ModelCondition
    {
        public TestCondition(IServiceScopeFactory serviceScopeFactory) : base("TestModelContainer.Service1.IsEnabled", nameof(TestCondition), serviceScopeFactory)
        {
          
        }

        public override void ModelComponentChanged(IModelComponent modelComponent)
        {
            IsFulfilled = (bool)(modelComponent as IModelVariable).GetValue();
            NotifyObserver();
        }

     }
}
