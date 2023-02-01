using Bruderer.Core.Domain.Exceptions;
using Bruderer.Core.Domain.Models.ModelComponentContainerAggregate;
using Bruderer.Core.Domain.Models.ModelConditionAggregate;
using Bruderer.Core.Domain.Models.ModelRPCAggregate;
using Bruderer.Core.Domain.Models.ModelVariableAggregate;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Bruderer.Core.Domain.Models.ModelAggregate
{
    public interface IModel
    {
        Guid Id { get; }
        string Name { get; }
        ModelStateEnumeration State { get; }

        List<IServiceModelContainer> ServiceModelContainers { get; }
        List<IRepositoryModelContainer> RepositoryModelContainers { get; }
        List<IModelComponentContainerCollection> EnumerableModelContainers { get; }

        ModelComponentContainer ComponentTree { get; }

        List<IModelVariable> Variables { get; }
        List<IModelRPC> RPCs { get; }

        List<ModelCondition> Conditions { get; }

        Task StartAsync(CancellationToken cancellationToken);
        Task StopAsync(CancellationToken cancellationToken);

        /// <summary>
        /// Consumes the passed model update data on this model. 
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
        /// <exception cref="ModelComponentScanningException">
        /// Thrown when the scanning of the affected model data failed.
        /// </exception>
        Task<bool> ConsumeModelData(ModelUpdateOptions modelUpdateOptions, CancellationToken cancellationToken);

        /// <summary>
        /// Consumes the passed model repository data on this model. 
        /// </summary>
        /// <remarks>
        /// For proper data consistency this this method uses the method <see cref="ConsumeModelData"/>.
        /// </remarks>
        /// <param name="modelRepositoryData">Model repository data to consume.</param>
        /// <returns>
        /// True if the passed model repository data is consumed, otherwise False.
        /// </returns>
        /// <exception cref="InvalidOperationException">
        /// Thrown when the passed model repository data could not be consumed.
        /// </exception>
        Task<bool> ConsumeModelData(ModelRepositoryData modelRepositoryData, CancellationToken cancellationToken);

        /// <summary>
        /// Releases the passed model update data on this model. 
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
        /// Pushes the passed variable data from <paramref name="modelVariableDEOs"/> in this model. 
        /// </summary>
        /// <remarks>
        /// The model variables in this model are searched by the ID of the passed variables.
        /// </remarks>
        /// <param name="modelVariableDEOs">Contains the new variable data to push.</param>
        /// <returns>
        /// True if the passed variable data is pushed in this model, otherwise False.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// Thrown when a passed argument is null or a corresponding model variable in this model could not be found.
        /// </exception>
        /// <exception cref="PushModelVariableDataException">
        /// Thrown when the new variable data could not be pushed.
        /// </exception>
        Task<bool> PushVariableData(IEnumerable<IModelVariableDEO> modelVariableDEOs, CancellationToken cancellationToken);

        List<IModelVariable> FindModelVariablesByID(List<Guid> IDs);
    }
}
