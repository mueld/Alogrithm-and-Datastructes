using Bruderer.Core.Domain.Models.ModelComponentContainerAggregate;
using Bruderer.Core.Domain.Models.ModelVariableAggregate;
using System.Collections.Generic;
using System.ComponentModel;

namespace Bruderer.Core.Domain.Test.TestComponents.Rockets.Components
{
    public class UpperCompositeService : ModelComponentContainer, IServiceModelContainer, IRepositoryModelContainer
    {
        [DisplayName("Fairing")]
        [Description("Fairing")]
        public virtual FairingType Fairing { get; set; } = new();

        [DisplayName("Speltra")]
        [Description("Speltra")]
        public virtual SpeltraType Speltra { get; set; } = new();

        [DisplayName("PayloadAdaptors")]
        [Description("PayloadAdaptors")]
        public virtual ModelComponentContainerCollection<PayloadAdaptorType> PayloadAdaptors { get; set; } = new(2);

        [DisplayName("Dynamic Palyoad Adaptors")]
        [Description("PayloadAdaptors")]
        public virtual ModelComponentContainerCollection<PayloadAdaptorType> DynPayloadAdaptors { get; set; } = new();

        [DisplayName("VehiculeEquipmentBay")]
        [Description("VehiculeEquipmentBay")]
        public virtual VehiculeEquipmentBayType VehiculeEquipmentBay { get; set; } = new();

        [DisplayName("NestedService")]
        [Description("NestedService")]
        public virtual LowerCompositeService NestedService { get; set; } = new();

        #region IRepositoryModelContainer

        public override string RepositoryTypeName { get; set; } = nameof(UpperCompositeService);
        public RepositoryCreationPolicyEnumeration RepositoryCreationPolicy { get; set; } = RepositoryCreationPolicyEnumeration.Unique;
        public List<string> RepositoryRelatedModelLinks { get; set; } = new();

        public bool IsEnabled   { get; private set; }

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
}
