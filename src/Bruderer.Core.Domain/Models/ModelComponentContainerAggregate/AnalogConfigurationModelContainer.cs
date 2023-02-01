using Bruderer.Core.Domain.Models.ModelVariableAggregate;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;

namespace Bruderer.Core.Domain.Models.ModelComponentContainerAggregate
{
    [NotMapped]
    public class AnalogConfigurationModelContainer : ModelComponentContainer, IAnalogConfigurationModelContainer
    {
        #region ctor

        public AnalogConfigurationModelContainer(){ }

        #endregion
        #region props

        [DisplayName("x0")]
        [Description("The x0 of the AnalogItemConfiguration")]
        public virtual ModelVariable<short> x0 { get; set; } = new ModelVariable<short>();

        [DisplayName("x1")]
        [Description("The x1 of the AnalogItemConfiguration")]
        public virtual ModelVariable<short> x1 { get; set; } = new ModelVariable<short>();

        [DisplayName("ValidMin")]
        [Description("The ValidMin of the AnalogItemConfiguration")]
        public virtual ModelVariable<short> ValidMin { get; set; } = new ModelVariable<short>();

        [DisplayName("ValidMax")]
        [Description("The ValidMax of the AnalogItemConfiguration")]
        public virtual ModelVariable<short> ValidMax { get; set; } = new ModelVariable<short>();

        [DisplayName("y0")]
        [Description("The y0 of the AnalogItemConfiguration")]
        public virtual ModelVariable<float> y0 { get; set; } = new ModelVariable<float>();

        [DisplayName("y1")]
        [Description("The y1 of the AnalogItemConfiguration")]
        public virtual ModelVariable<float> y1 { get; set; } = new ModelVariable<float>();

        public void DataSeeding()
        {
            this.x0 = new ModelVariable<short>();
            this.x1 = new ModelVariable<short>();
            this.y0 = new ModelVariable<float>();
            this.y1 = new ModelVariable<float>();
            ValidMax = new ModelVariable<short>();
            ValidMin = new ModelVariable<short>();
        }

        public void DataSeeding(AnalogConfigurationDataSeedingType sensorConfiguration)
        {
            this.x0 = new ModelVariable<short>(sensorConfiguration.x0);
            this.x1 = new ModelVariable<short>(sensorConfiguration.x1);
            this.y0 = new ModelVariable<float>(sensorConfiguration.y0);
            this.y1 = new ModelVariable<float>(sensorConfiguration.y1);
            ValidMax = new ModelVariable<short>(sensorConfiguration.ValidMax);
            ValidMin = new ModelVariable<short>(sensorConfiguration.ValidMin);
         
        }
        #endregion
    }

    public class AnalogConfigurationDataSeedingType
    {
        public short x0 { get; set; } = 0;
        public short x1 { get; set; } = 0;
        public float y0 { get; set; } = 0.0f;
        public float y1 { get; set; } = 0.0f;
        public short ValidMax { get; set; } = 0;
        public short ValidMin { get; set; } = 0;
    }
}
