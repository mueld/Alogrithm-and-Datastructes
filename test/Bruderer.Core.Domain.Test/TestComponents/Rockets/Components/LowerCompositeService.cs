using Bruderer.Core.Domain.Attributes;
using Bruderer.Core.Domain.Models.ModelComponentContainerAggregate;
using Bruderer.Core.Domain.Models.ModelIdentityUserAggregate;
using Bruderer.Core.Domain.Models.ModelRPCAggregate;
using Bruderer.Core.Domain.Models.ModelRPCAggregate.Arguments;
using Bruderer.Core.Domain.Models.ModelVariableAggregate;
using System.Collections.Generic;
using System.ComponentModel;

namespace Bruderer.Core.Domain.Test.TestComponents.Rockets.Components
{

    /// <summary>
    /// Number of variables of this Servive: 14
    /// Number of RPC's of this Servive: 1
    /// </summary>
    public class LowerCompositeService : ModelComponentContainer, IServiceModelContainer, IRepositoryModelContainer, IModelDependency
    {
        [DisplayName("CryogenicMainCoreStorage")]
        [Description("CryogenicMainCoreStorage")]
        public virtual CryogenicMainCoreStorageType CryogenicMainCoreStorage { get; set; } = new();

        [DisplayName("SolidRocketBoosters")]
        [Description("SolidRocketBoosters")]
        [VisibilityAuthorization(ModelIdentityUserRoleEnumeration.Anonymous)]
        [ManipulationAuthorization(ModelIdentityUserRoleEnumeration.Operator)]
        public virtual ModelComponentContainerCollection<SolidRocketBoosterType> SolidRocketBoosters { get; set; } = new(2);

        [DisplayName("VulcainEngine")]
        [Description("VulcainEngine")]
        public virtual VulcainEngineType VulcainEngine { get; set; } = new();

        [DisplayName("StartEngine")]
        [Description("StartEngine")]
        public ModelRPC<StartEngineInputArgument, StartEngineOuputArgument> StartEngine { get; } = new();

        #region IRepositoryModelContainer

        public override string RepositoryTypeName { get; set; } = nameof(LowerCompositeService);
        public RepositoryCreationPolicyEnumeration RepositoryCreationPolicy { get; set; } = RepositoryCreationPolicyEnumeration.Unique;
        public List<string> RepositoryRelatedModelLinks { get; set; } = new();

        public bool IsEnabled { get; private set; }

        #endregion

        #region ISeedableModelContainer

        public virtual void DataSeeding()
        {

        }



        #endregion

        #region IServiceModelContainer
        public List<ModelVariableBase> GetAllActiveVariables()
        {
            return new();
        }
        #endregion

    }

    public class StartEngineInputArgument : ModelRPCInputArgumentContainer
    {
        [DisplayName("BoosterIndex")]
        [Description("BoosterIndex")]
        public ModelRPCInputArgument<ushort> BoosterIndex { get; set; } = new ModelRPCInputArgument<ushort>();
    }

    public class StartEngineOuputArgument : ModelRPCOutputArgumentContainer
    {
        [DisplayName("Result")]
        [Description("Result")]
        public ModelRPCInputArgument<int> Result { get; set; } = new ModelRPCInputArgument<int>();
    }
}
