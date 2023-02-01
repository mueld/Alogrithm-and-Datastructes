using Bruderer.Core.Domain.Commands;
using Bruderer.Core.Domain.Exceptions;
using Bruderer.Core.Domain.Messaging.Response;
using Bruderer.Core.Domain.Models.ComponentAggregate;
using Bruderer.Core.Domain.Models.ModelAggregate;
using Bruderer.Core.Domain.Models.ModelRPCAggregate.Arguments;
using Bruderer.Core.Domain.Models.ModelRPCAggregate.Response;
using Bruderer.Core.Domain.Models.ModelVariableAggregate;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Bruderer.Core.Domain.Models.ModelUserAggregate
{
    public interface IModelUser
    {
        Guid Id { get; }
        string Name { get; }
        ModelUserPlatformEnumeration Platform { get; }
        ModelUserStateEnumeration State { get; }
        IComponent Component { get; }

        Task StartAsync(CancellationToken cancellationToken);
        Task StopAsync(CancellationToken cancellationToken);

        /// <summary>
        /// Consumes the passed model update data on this model user. 
        /// </summary>
        /// <remarks>
        /// For proper data consistency this this method uses the method <see cref="ReleaseModelData"/>.
        /// </remarks>
        /// <param name="modelUpdateOptions">Contains the model update data to consume.</param>
        /// <returns>
        /// True if the passed model update data is consumed, otherwise False.
        /// </returns>
        /// <exception cref="InvalidOperationException">
        /// Thrown when the passed model update data could not be consumed.
        /// </exception>
        Task<bool> ConsumeModelData(ModelUpdateOptions modelUpdateOptions, CancellationToken cancellationToken);

        /// <summary>
        /// Releases the passed model update data on this model user. 
        /// </summary>
        /// <remarks>
        /// For proper data consistency this method should only used in combination with the method <see cref="ConsumeModelData"/>.
        /// </remarks>
        /// <param name="modelUpdateOptions">Contains the model update data to release.</param>
        /// <returns>
        /// True if the passed model update data is released, otherwise False.
        /// </returns>
        Task<bool> ReleaseModelData(ModelUpdateOptions modelUpdateOptions, CancellationToken cancellationToken);

        /// <summary>
        /// Pushes the passed variable data from <paramref name="modelVariableDEOs"/> in this model user. 
        /// </summary>
        /// <remarks>
        /// The model variables in this model user are searched by the ID of the passed variables.
        /// </remarks>
        /// <param name="modelVariableDEOs">Contains the new variable data to push.</param>
        /// <returns>
        /// True if the passed variable data is pushed in this model user, otherwise False.
        /// </returns>
        /// <exception cref="PushModelVariableDataException">
        /// Thrown when the new variable data could not be pushed.
        /// </exception>
        Task<bool> PushVariableData(IEnumerable<IModelVariableDEO> modelVariableDEOs, CancellationToken cancellationToken);

        /// <summary>
        /// Pulls all model data from this model user.
        /// </summary>
        /// <remarks>
        /// After a successful pull of the data from this model user the changed model variable values will be dispatched as <see cref="ModelVariablesValueChangeCommand"/> command. So the changed values can be catched via the regular flow in a corresponding event handler.
        /// </remarks>
        /// <param name="cancellationToken">CancellationToken for this task.</param>
        /// <returns>
        /// True if the data is pulled and dispatched, otherwise False.
        /// </returns>
        /// <exception cref="InvalidOperationException">
        /// Thrown when the data coult not be pulled from this model user.
        /// </exception>
        Task<Dictionary<Guid,List<IModelVariableDEO>>> PullVariableData(CancellationToken cancellationToken);

        /// <summary>
        /// Pulls model data from this model user by the passed Informations of <paramref name="variableDEOs"/>.
        /// </summary>
        /// <remarks>
        /// After a successful pull of the data from this model user the changed model variable values will be dispatched as <see cref="ModelVariablesValueChangeCommand"/> command. So the changed values can be catched via the regular flow in a corresponding event handler.
        /// </remarks>
        /// <param name="cancellationToken">CancellationToken for this task.</param>
        /// <returns>
        /// True if the data is pulled and dispatched, otherwise False.
        /// </returns>
        /// <exception cref="InvalidOperationException">
        /// Thrown when the data coult not be pulled from this model user.
        /// </exception>
        Task<Dictionary<Guid, List<IModelVariableDEO>>> PullVariableData(List<Guid> variableIDs, CancellationToken cancellationToken);

        Task<ModelRPCResponse> InvokeRpcMethod(Guid rpcId, IList<object> inputArguments, CancellationToken cancellationToken);
        Task<RPCResponse<T>> InvokeRpcMethod<T>(Guid rpcId, IList<object> inputArguments, CancellationToken cancellationToken)
            where T : ModelRPCOutputArgumentContainer;
    }
}
