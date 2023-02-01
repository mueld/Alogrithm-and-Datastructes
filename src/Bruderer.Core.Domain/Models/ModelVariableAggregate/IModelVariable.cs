using Bruderer.Core.Domain.Models.ModelComponentAggregate;
using Bruderer.Core.Domain.Models.ModelConditionAggregate;
using Bruderer.Core.Domain.Models.ValueObjects;
using System;
using System.Collections.Generic;

#nullable enable

namespace Bruderer.Core.Domain.Models.ModelVariableAggregate
{
    public interface IModelVariable : IModelComponent, IModelUserRoleAuthorizationComponent, IModelLocalizationComponent, IModelRepositoryComponent
    {
        UnitValue? EngineeringUnit { get; }
        bool IsReadOnly { get; }
        bool IsVisible { get; }
        bool IsInvalid { get; set; }
        int ScanningIndex { get; }
        Type ValueType { get; }
        int? ValuePrecision { get; }
        string? ValueEnumerationKey { get; }

        bool IsValueEqual(object? value);
        object? GetConvertedValueType(object? value);

        object? GetValue();
        TValue? GetValue<TValue>();
        void SetValue(object? newValue);
        void SetValue<TValue>(TValue newValue);
        object? GetValueMaximum();
        TValue? GetValueMaximum<TValue>();
        void SetValueMaximum(object? newValue);

        object? GetValueMinimum();
        TValue? GetValueMinimum<TValue>();
        void SetValueMinimum(object? newValue);

        IModelVariableDEO GetDEO(bool includeReference = true);

        void AddVisibilityCondition(IModelCondition condition);
        void RemoveVisibilityCondition(IModelCondition condition);
        void AddEditabilityCondition(IModelCondition condition);
        void RemoveEditabilityCondition(IModelCondition condition);

        void Take(IModelVariable sourceVariable, bool takeMetadata = true, bool takeValue = true, bool onlyRespositoryData = false);
        void Take(IModelVariableDEO modelVariableDEO, bool takeId = false, bool takeModelLink = false);

        event EventHandler<ModelVariableChangedEventArgs> ValueChanged;
    }
}
