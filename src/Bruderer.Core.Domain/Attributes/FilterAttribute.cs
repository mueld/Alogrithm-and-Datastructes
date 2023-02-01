using System;
using System.Collections.Generic;

namespace Bruderer.Core.Domain.Attributes
{
    [AttributeUsage(AttributeTargets.Property)]
    public class FilterAttribute : Attribute
    {
        #region ctor

        public FilterAttribute(string filterName)
        {
            Filters = new List<string>
            {
                filterName
            };
        }

        public FilterAttribute(string[] filterNames)
        {
            Filters = new List<string>();
            Filters.AddRange(filterNames);
        }

        public FilterAttribute(IEnumerable<string> filterNames)
        {
            Filters = new List<string>();
            Filters.AddRange(filterNames);
        }

        #endregion
        #region props

        public List<string> Filters { get; } = new List<string>();

        #endregion
    }
}
