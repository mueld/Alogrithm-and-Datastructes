using Bruderer.Core.Domain.Models.ModelAggregate;
using Bruderer.Core.Domain.Models.ModelComponentAggregate;
using Bruderer.Core.Domain.Models.ModelRPCAggregate;
using Bruderer.Core.Domain.Models.ModelVariableAggregate;
using System;
using System.Collections.Generic;

namespace Bruderer.Core.Domain.Models.ModelComponentContainerAggregate
{
    public interface IModelComponentContainer : IModelComponent, IModelUserRoleAuthorizationComponent, IModelLocalizationComponent, IModelRepositoryComponent
    {
        bool IsEnumerable { get; set; }
        bool IsEnumerableContainer { get; set; }
        int EnumerationCount { get; set; }
        int EnumerationIndex { get; set; }
        List<IModelComponentContainerCollection> EnumerableModelContainers { get; set; }
        List<ModelComponentContainer> ModelContainers{ get; set; }
        List<ModelVariableBase> ModelVariables { get; set; }
        List<ModelRPCBase> ModelRPCs { get; set; }

        bool IsRepositoryRootContainer { get; }

        /// <summary>
        /// Performs a scan of model components in this container. 
        /// </summary>
        /// <remarks>
        /// Existing metadata will be overwritten. Use the <paramref name="scanningProps"/> to provide additional predefined parameters to the scan.
        /// </remarks>
        /// <param name="scanningProps">Predefinded scanning properties</param>
        /// <returns>
        /// True if the component scan of this model container was successful, otherwise False.
        /// </returns>
        bool Scan(ModelScanningProps scanningProps);

        object? Clone(ModelComponentContainerCloneOptions? cloneOptions = default);

        /// <summary>
        /// Finds recursively all model components in this container that implements <see cref="IModelComponent"/>.
        /// </summary>
        /// <typeparam name="T">Interface of <see cref="IModelComponent"/></typeparam>
        /// <returns>
        /// A list with the found <see cref="IModelComponent"/> types.
        /// </returns>
        List<T> FindComponents<T>()
            where T : IModelComponent;

        /// <summary>
        /// Finds recursively all model components in this container that implements <see cref="IModelComponent"/>. Performs a new scan of this container before the components are searched.
        /// </summary>
        /// <remarks>
        /// Uses <see cref="Scan"/> method for scanning.
        /// </remarks>
        /// <typeparam name="T">Interface of <see cref="IModelComponent"/></typeparam>
        /// <param name="scanningProps">Predefinded scanning properties</param>
        /// <returns>
        /// A list with the found <see cref="IModelComponent"/> types.
        /// </returns>
        List<T> FindComponents<T>(ModelScanningProps scanningProps)
            where T : IModelComponent;

        /// <summary>
        /// Finds all ModelComponentContainer objects that implement T. T must be a IModelComponent interface. 
        /// Since T has no class construct, it cannot be used as a return value and therefore returns a list of abstract ModelComponentContainers.
        /// You must perform the explicit cast to the explicit type yourself.
        /// </summary>
        /// <typeparam name="T">Interface of <see cref="IModelComponent" /></typeparam>
        /// <returns>
        /// A list with the found <see cref="ModelComponentContainer"/> types.
        /// </returns>
        List<ModelComponentContainer> FindExplicitContainers<T>();

        /// <summary>
        /// Consumes the passed model repository data on this container. 
        /// </summary>
        /// <param name="modelRepositoryData">Model repository data to consume</param>
        /// <param name="scanningProps">Optional predefinded scanning properties</param>
        /// <returns>
        /// True if the passed model repository data is consumed, otherwise False.
        /// </returns>
        /// <exception cref="InvalidOperationException">
        /// Thrown when the passed model repository data could not be consumed.
        /// </exception>
        bool ConsumeModelData(ModelRepositoryData modelRepositoryData);

        [Obsolete("This Prop is Deprecated. Use FindComponents instead.", false)]
        List<ModelVariableBase> CollectedVariables { get; set; }
        [Obsolete("This Method is Deprecated. Use FindComponents instead.", false)]
        bool CollectVariables();
        [Obsolete("This Method is Deprecated. Use FindComponents instead.", false)]
        bool CollectVariables(ModelScanningProps scannerProps);
        [Obsolete("This Method is Deprecated. You should remove all dependencies.", false)]
        bool DistributeVariables();
    }
}
