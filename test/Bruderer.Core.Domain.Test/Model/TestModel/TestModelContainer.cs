using Bruderer.Core.Domain.Attributes;
using Bruderer.Core.Domain.Models.ModelAggregate;
using Bruderer.Core.Domain.Models.ModelComponentContainerAggregate;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Bruderer.Core.Domain.Test.Model.TestModel
{

    [DisplayName(nameof(TestModelContainer))]
    [Description("Testmodel specification.")]
    public class TestModelContainer : ModelComponentContainer
    {
        public TestModelContainer()
        {
            Service2.VisibilityConditions.Add("TestCondition");
        }
        [VisibilityCondtionsAttribute("TestCondition")]
        public TestModelChildreen Service1 { get; set; } = new TestModelChildreen();
        public TestModelChildreen Service2 { get; set; } = new TestModelChildreen();
        public TestModelChildreen Service3 { get; set; } = new TestModelChildreen();
        public TestModelChildreen Service4 { get; set; } = new TestModelChildreen();
    }
}
