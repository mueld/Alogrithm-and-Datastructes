using Bruderer.Core.Infrastructure.PLC.TwinCAT3.Connection;
using System;
using TwinCAT.Ads.SumCommand;

namespace Bruderer.Core.Infrastructure.PLC.TwinCAT3.Exceptions
{
    [Serializable]
    public class TC3WriteSymbolException : InvalidOperationException
    {
        public const string DefaultMessage = "Error writing TwinCAT 3 symbols.";

        public TC3WriteSymbolException(string contextName, TC3VariableBlock tc3VariableBlock)
            : base(DefaultMessage)
        {
            InjectData(contextName, tc3VariableBlock);
        }

        public TC3WriteSymbolException(string contextName, ResultSumCommand resultSumCommand)
           : base(DefaultMessage)
        {
            InjectData(contextName, resultSumCommand);
        }

        public TC3WriteSymbolException(string contextName, TC3VariableBlock tc3VariableBlock, Exception inner)
            : base(DefaultMessage, inner)
        {
            InjectData(contextName, tc3VariableBlock);
        }

        private void InjectData(string contextName, TC3VariableBlock tc3VariableBlock)
        {
            Data.Add("ContextName", contextName);

            if (tc3VariableBlock == null)
            {
                Data.Add("TC3_VariableBLock_NULL", true);
                return;
            }

            if (tc3VariableBlock.Variables == null)
            {
                Data.Add("TC3_Variables_NULL", true);
                return;
            }

            Data.Add("TC3_VariablesCount", tc3VariableBlock.Variables.Count);

            if (tc3VariableBlock.VariableValues == null)
            {
                Data.Add("TC3_VariableValues_NULL", true);
                return;
            }

            Data.Add("TC3_VariableValuesCount", tc3VariableBlock.VariableValues.Count);

            for (int i = 0; i < tc3VariableBlock.Variables.Count; i++)
            {
                var tc3Variable = tc3VariableBlock.Variables[i];
                if (tc3Variable == null)
                {
                    Data.Add("TC3_Variable_NULL_Index", i);
                    continue;
                }

                if (tc3Variable.ModelVariable == null)
                {
                    Data.Add("TC3_ModelVariable_NULL_Index", i);
                    continue;
                }

                if (tc3Variable.Symbol == null)
                {
                    Data.Add("TC3_Symbol_NULL_Index", i);
                    continue;
                }

                if (i >= tc3VariableBlock.VariableValues.Count)
                {
                    Data.Add($"TC3_Variable_{tc3Variable.ModelVariable.ModelLink.Key}_ValueIndexMismatch", i);
                    continue;
                }

                var writeRequestValue = tc3VariableBlock.VariableValues[i];
                if (writeRequestValue == null)
                {
                    Data.Add("TC3_VariableValue_NULL_Index", i);
                    continue;
                }

                Data.Add($"TC3_Variable_{tc3Variable.ModelVariable.ModelLink.Key}_Value", writeRequestValue);
                continue;
            }
        }

        private void InjectData(string contextName, ResultSumCommand resultSumCommand)
        {
            Data.Add("ContextName", contextName);

            if (resultSumCommand == null)
            {
                Data.Add("TC3_ResultSumCommand_NULL", true);
                return;
            }

            Data.Add("TC3_ADS_ErrorCode", resultSumCommand.ErrorCode.ToString());
            Data.Add("TC3_ADS_FirstSubErrorCode", resultSumCommand.FirstSubError.ToString());
            Data.Add("TC3_ADS_OverallErrorCode", resultSumCommand.OverallError.ToString());

            foreach (var subError in resultSumCommand.SubErrors)
                Data.Add("TC3_ADS_SubErrorCode", subError.ToString());
        }
    }
}
