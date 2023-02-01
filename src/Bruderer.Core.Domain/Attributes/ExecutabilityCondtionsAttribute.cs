using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bruderer.Core.Domain.Attributes
{
    [AttributeUsage(AttributeTargets.Property)]
    public class ExecutabilityCondtionsAttribute: Attribute
    {
        #region ctor
        public ExecutabilityCondtionsAttribute(string condtion)
        {
            Conditions.Add(condtion);
        }
        public ExecutabilityCondtionsAttribute(string[] condtions)
        {
            Conditions.AddRange(condtions);
        }
        #endregion

        #region props
        public List<string> Conditions { get; set; } = new();
        #endregion
    }
}
