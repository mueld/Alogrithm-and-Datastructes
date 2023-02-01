using Bruderer.Core.Domain.Interfaces;
using Bruderer.Core.Domain.Models.ModelComponentContainerAggregate;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Bruderer.Core.Domain.Models.ModelAggregate
{
    public interface IModelRepository : IRepository
    {
        /// <summary>
        /// Gets the repository infos for the passed type parameter <typeparamref name="T"/>. 
        /// </summary>
        /// <remarks>
        /// A repository info is a file with serialized entries from type <see cref="ModelRepositoryInfo"/>.
        /// </remarks>
        /// <typeparam name="T">Type of <see cref="ModelRepositoryInfo" /></typeparam>
        /// <returns>
        /// A List of all repository info objects from type <typeparamref name="T"/> .
        /// </returns>
        /// <exception cref="InvalidOperationException">
        /// Thrown when the repository infos could not be pulled.
        /// </exception>
        Task<List<T>> GetRepositoryInfos<T>()
            where T : ModelRepositoryInfo;

        /// <summary>
        /// Pulls the model data for the passed parameters from the repository. 
        /// </summary>
        /// <param name="repositoryPartTypeName">Part type name of the repository.</param>
        /// <param name="id">Id of the repository.</param>
        /// <typeparam name="T">Interface of <see cref="IRepositoryModelContainer" /></typeparam>
        /// <returns>
        /// The pulled repository data as <see cref="ModelRepositoryData"/> type.
        /// </returns>
        /// <exception cref="InvalidOperationException">
        /// Thrown when the data of the repository container could not be pulled.
        /// </exception>
        Task<ModelRepositoryData> PullModelData<T>(string repositoryPartTypeName, Guid id)
             where T : IRepositoryModelContainer;

        /// <summary>
        /// Pulls the model data for the passed parameters from the repository. 
        /// </summary>
        /// <param name="repositoryContainer">Model container with the imnplemented "IRepositoryModelContainer" interfaces.</param>
        /// <param name="id">Id of the repository.</param>
        /// <returns>
        /// The pulled repository data as <see cref="ModelRepositoryData"/> type.
        /// </returns>
        /// <exception cref="InvalidOperationException">
        /// Thrown when the data of the repository container could not be pulled.
        /// </exception>
        Task<ModelRepositoryData> PullModelData<T>(T repositoryContainer, Guid id)
            where T : IRepositoryModelContainer;

        /// <summary>
        /// Pushes the model data from the passed parameters to the repository.  
        /// </summary>
        /// <param name="modelRepositoryData">The model data to push.</param>
        /// <returns>
        /// True if the model data was pushed to the repository, otherwise False.
        /// </returns>
        /// <exception cref="InvalidOperationException">
        /// Thrown when the model data could not be pushed.
        /// </exception>
        Task<bool> PushModelData<T>(ModelRepositoryData modelRepositoryData)
            where T : IRepositoryModelContainer;

        /// <summary>
        /// Pushes the model data from the passed parameters to the repository. 
        /// </summary>
        /// <param name="repositoryContainer">The repository container to push. This container must implement the interface "IRepositoryModelContainer".</param>
        /// <param name="id">The id of the repository.</param>
        /// <returns>
        /// True if the model data was pushed to the repository, otherwise False.
        /// </returns>
        /// <exception cref="InvalidOperationException">
        /// Thrown when the model data could not be pushed.
        /// </exception>
        Task<bool> PushModelData<T>(T repositoryContainer)
            where T : IRepositoryModelContainer;

        /// <summary>
        /// Removes the model data for the passed parameters in the repository. 
        /// </summary>
        /// <param name="id">The id of the repository.</param>
        /// <returns>
        /// True if the model data was removed from the repository, otherwise False.
        /// </returns>
        /// <exception cref="InvalidOperationException">
        /// Thrown when the data of the repository container could not be removed.
        /// </exception>
        Task<bool> RemoveModelData<T>(Guid id)
            where T : IRepositoryModelContainer;
    }
}
