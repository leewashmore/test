﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using System.ServiceModel.Activation;
using System.Resources;
using GreenField.Web.Helpers.Service_Faults;
using GreenField.Web.Helpers;
using GreenField.DataContracts;
using GreenField.DAL;
using System.Data.Objects;
using GreenField.Web.DimensionEntitiesService;
using System.Configuration;
using GreenField.Web.DataContracts;
using System.Data;
using GreenField.DataContracts.DataContracts;

namespace GreenField.Web.Services
{
    /// <summary>
    /// DCF Operations
    /// </summary>
    [ServiceContract]
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed)]
    public class DCFOperations
    {
        public ResourceManager ServiceFaultResourceManager
        {
            get
            {
                return new ResourceManager(typeof(FaultDescriptions));
            }
        }

        private Entities dimensionEntity;
        public Entities DimensionEntity
        {
            get
            {
                if (null == dimensionEntity)
                    dimensionEntity = new Entities(new Uri(ConfigurationManager.AppSettings["DimensionWebService"]));

                return dimensionEntity;
            }
        }

        /// <summary>
        /// Operation Contract for DCFAnalysisSummary
        /// </summary>
        /// <param name="entitySelectionData">Selected Security</param>
        /// <returns>Collection of DCFAnalysisSummaryData</returns>
        [OperationContract]
        [FaultContract(typeof(ServiceFault))]
        public List<DCFAnalysisSummaryData> RetrieveDCFAnalysisData(EntitySelectionData entitySelectionData)
        {
            try
            {
                string issuerId;
                List<DCFAnalysisSummaryData> result = new List<DCFAnalysisSummaryData>();
                List<DCFAnalysisSummaryData_Result> dbResult;

                ExternalResearchEntities entity = new ExternalResearchEntities();

                if (entitySelectionData == null)
                    return new List<DCFAnalysisSummaryData>();

                #region ServiceAvailabilityChecker
                
                bool isServiceUp;
                isServiceUp = CheckServiceAvailability.ServiceAvailability();

                if (!isServiceUp)
                    throw new Exception("Services are not available"); 

                #endregion

                GF_SECURITY_BASEVIEW securityDetails = DimensionEntity.GF_SECURITY_BASEVIEW
                    .Where(record => record.ASEC_SEC_SHORT_NAME == entitySelectionData.InstrumentID &&
                        record.ISSUE_NAME == entitySelectionData.LongName &&
                        record.TICKER == entitySelectionData.ShortName).FirstOrDefault();

                issuerId = securityDetails.ISSUER_ID;
                if (issuerId == null)
                    return new List<DCFAnalysisSummaryData>();

                dbResult = entity.RetrieveDCFAnalysisSummaryData(issuerId, "PRIMARY", "A", "FISCAL", "USD").ToList();

                DCFAnalysisSummaryData data = new DCFAnalysisSummaryData();

                data.SecurityId = securityDetails.ASEC_SEC_SHORT_NAME;
                data.IssuerId = securityDetails.ISSUER_ID;
                data.Beta = (securityDetails.BARRA_BETA == null) ?
                    (Convert.ToDecimal(securityDetails.BETA)) : (Convert.ToDecimal(securityDetails.BARRA_BETA));
                data.CostOfDebt = Convert.ToDecimal(securityDetails.WACC_COST_DEBT);
                data.MarginalTaxRate = dbResult.Where(a => a.DATA_ID == 232).Select(a => a.AMOUNT).FirstOrDefault();
                data.GrossDebt = dbResult.Where(a => a.DATA_ID == 90).Select(a => a.AMOUNT).FirstOrDefault();

                result.Add(data);
                return result;
            }
            catch (Exception ex)
            {
                ExceptionTrace.LogException(ex);
                string networkFaultMessage = ServiceFaultResourceManager.GetString("NetworkFault").ToString();
                throw new FaultException<ServiceFault>(new ServiceFault(networkFaultMessage), new FaultReason(ex.Message));
            }
        }
        /// <summary>
        /// Gets FreCashFlows Data
        /// </summary>
        /// <param name="securityId"></param>
        /// <returns>FreCashFlows data</returns>
        [OperationContract]
        [FaultContract(typeof(ServiceFault))]
        public List<FreeCashFlowsData> RetrieveFreCashFlowsData(EntitySelectionData entitySelectionData)
        {
            try
            {
                List<FreeCashFlowsData> result = new List<FreeCashFlowsData>();
                //List<GetFreeCashFlowsData_Result> resultDB = new List<GetFreeCashFlowsData_Result>();
                ExternalResearchEntities extResearch = new ExternalResearchEntities();
                if (entitySelectionData == null)
                    return null;

                DimensionEntitiesService.Entities entity = DimensionEntity;

                bool isServiceUp;
                isServiceUp = CheckServiceAvailability.ServiceAvailability();

                if (!isServiceUp)
                    throw new Exception("Services are not available");

                //Retrieving data from security view
                DimensionEntitiesService.GF_SECURITY_BASEVIEW data = entity.GF_SECURITY_BASEVIEW
                    .Where(record => record.TICKER == entitySelectionData.ShortName
                        && record.ISSUE_NAME == entitySelectionData.LongName
                        && record.ASEC_SEC_SHORT_NAME == entitySelectionData.InstrumentID
                        && record.SECURITY_TYPE == entitySelectionData.SecurityType)
                    .FirstOrDefault();

                if (data == null)
                    return null;

                FreeCashFlowsData FreeCashFlowsData = new FreeCashFlowsData();                

                ////Retrieving data from Period Financials table
                //resultDB = extResearch.ExecuteStoreQuery<GetFreeCashFlowsData_Result>("exec GetFreeCashFlowsData @IssuerID={0}", Convert.ToString(data.ISSUER_ID)).ToList();



                
                //result.Add(FreeCashFlowsData);

                return result;
            }
            catch (Exception ex)
            {
                ExceptionTrace.LogException(ex);
                string networkFaultMessage = ServiceFaultResourceManager.GetString("NetworkFault").ToString();
                throw new FaultException<ServiceFault>(new ServiceFault(networkFaultMessage), new FaultReason(ex.Message));
            }
        }
    }
}