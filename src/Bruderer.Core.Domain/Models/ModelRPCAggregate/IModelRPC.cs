using Bruderer.Core.Domain.Interfaces;
using Bruderer.Core.Domain.Models.ModelComponentAggregate;
using Bruderer.Core.Domain.Models.ModelConditionAggregate;
using Bruderer.Core.Domain.Models.ModelRPCAggregate.Arguments;
using System;
using System.Collections.Generic;

namespace Bruderer.Core.Domain.Models.ModelRPCAggregate
{
    public interface IModelRPC : IModelComponent, IModelUserRoleAuthorizationComponent, IModelLocalizationComponent
    {
        Type InputArgumentType { get; }
        Type OutputArgumentType { get; }
        bool IsExecutable { get; }
        void AddModelCondition(IModelCondition modelCondition);

        /// <summary>
        /// invoke this method  to notify registered observers. Must be invoked after successfully RPC invocation.
        /// </summary>
        void Invoked();

        IList<object> GetInputArguments();
        IList<ModelRPCArgument> GetInputArguments<T>()
            where T : ModelRPCArgument;

        T GetInputArgument<T>()
            where T : ModelRPCInputArgumentContainer;
        bool SetInputArgument<T>(T inputArgumentContainer)
            where T : ModelRPCInputArgumentContainer;

        IList<object> GetOutputArguments();
        IList<ModelRPCArgument> GetOutputArguments<T>()
            where T : ModelRPCArgument;

        T GetOutputArgumentContainer<T>(object outputArgument)
            where T : ModelRPCOutputArgumentContainer;
        T GetOutputArgumentContainer<T>(IList<object> outputArguments)
            where T : ModelRPCOutputArgumentContainer;
    }
}
