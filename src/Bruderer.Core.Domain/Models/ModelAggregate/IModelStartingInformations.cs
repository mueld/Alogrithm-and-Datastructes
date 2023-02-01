using Bruderer.Core.Domain.Models.ModelRPCAggregate;
using Bruderer.Core.Domain.Models.ModelVariableAggregate;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bruderer.Core.Domain.Models.ModelAggregate
{
    public interface IModelStartingInformations
    {
        /// <summary>
        /// Method for the model provider to obtain all relevant model variables. Provider have to invoke this method while its start. 
        /// </summary>
        List<IModelVariable> GetAllRelevantVariables();

        /// <summary>
        /// Method for the model provider to obtain all relevant model rpc's. Provider have to invoke this method while its start. 
        /// </summary>
        List<IModelRPC> GetAllRelevantRPCs();
    }
}
