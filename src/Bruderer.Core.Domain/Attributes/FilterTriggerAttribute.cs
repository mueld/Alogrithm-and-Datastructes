using System;
using System.Collections.Generic;

namespace Bruderer.Core.Domain.Attributes
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = true)]
    public class FilterTriggerAttribute : Attribute
    {
        #region ctor

        public FilterTriggerAttribute(string filterName)
        {
            Trigger.Filters.Add(filterName);
        }

        public FilterTriggerAttribute(string[] modelKeys)
        {
            Trigger.Filters.AddRange(modelKeys);
        }

        public FilterTriggerAttribute(IEnumerable<string> modelKeys)
        {
            Trigger.Filters.AddRange(modelKeys);
        }

        #endregion
        #region props

        public FilterTrigger Trigger { get; } = new FilterTrigger();

        #endregion
    }

    public enum FilterTriggerActionEnumeration
    {
        Undefined = 0,
        Sampling
    }

    public enum FilterTriggerConditionCompareTypeEnumeration
    {
        Undefined = 0,
        Equal,
        NotEqual,
        Less,
        Greater
    }

    public class FilterTrigger
    {
        public List<string> Filters { get; set; } = new List<string>();
        public FilterTriggerActionEnumeration Action { get; set; } = FilterTriggerActionEnumeration.Sampling;
        public bool HasCondtion
        {
            get { return TriggerCondition != null; }
        }
        public object TriggerCondition { get; set; } = null;
        public FilterTriggerConditionCompareTypeEnumeration TriggerConditionCompareType { get; set; } = FilterTriggerConditionCompareTypeEnumeration.Undefined;
    }
}
