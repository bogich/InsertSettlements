using System;
using System.Net;
using System.Linq;
using System.Data.SqlClient;
using System.Configuration;
using _OrganizationRegistryCommonService = Gis.Infrastructure.OrganizationsRegistryCommonService;
using _HouseManagementService = Gis.Infrastructure.HouseManagementService;
using _BillsService = Gis.Infrastructure.BillsService;
using Gis.Helpers.HelperOrganizationRegistryCommonService;
using Gis.Helpers.HelperHouseManagementService;
using Gis.Helpers.HelperBillService;
using Gis.Helpers.BaseClasses;
using Gis.Crypto;

namespace Gis
{
    class Program
	{
        private const string _orgPPAGUID = "04b83d24-6daa-47f9-bb4f-ce9330c3094d"; //Prod 
        //private const string _orgPPAGUID = "73075d22-be00-47c3-ad82-e95be27ac276"; // SIT01
        static string _ContractRootGUID;
        static string _OrgRootEntityGUID;
        static string _OrgRootOrganizationINN;
        static string _ExportContractRootGUID;
        static string _SettlementGUID;
        static bool isExistValue;
        static decimal _TotalDebSum;
        static decimal _TotalAvansSum;
        static decimal _TotalPayoffSum;
        static decimal _TotalSaldo;

        static void Main(string[] args)
		{
            ServicePointManager.ServerCertificateValidationCallback = delegate { return true; };

            HelperHouseManagementService helperHouseManagementService = new HelperHouseManagementService();
            HelperOrganizationRegistryCommonService helperOrganizationRegistryCommonService = new HelperOrganizationRegistryCommonService();
            HelperBillService helperBillService = new HelperBillService();

            string typeRes = null;
            do
            {
                foreach (var tmpSupplyResourceContract in helperHouseManagementService.GetSupplyResourceContractData(_orgPPAGUID, _ExportContractRootGUID).exportSupplyResourceContractResult.Items)
                {
                    typeRes = tmpSupplyResourceContract.ToString();
                    if (tmpSupplyResourceContract.GetType() == typeof(_HouseManagementService.exportSupplyResourceContractResultType))
                    {
                        var tmpExportSupplyResourceContract = tmpSupplyResourceContract as _HouseManagementService.exportSupplyResourceContractResultType;
                        if (tmpExportSupplyResourceContract.VersionStatus == _HouseManagementService.exportSupplyResourceContractResultTypeVersionStatus.Posted
                            && tmpExportSupplyResourceContract.Item1.GetType() == typeof(_HouseManagementService.ExportSupplyResourceContractTypeOrganization))
                        {
                            Console.WriteLine("ContractRootGUID: {0}", tmpExportSupplyResourceContract.ContractRootGUID);
                            _ContractRootGUID = tmpExportSupplyResourceContract.ContractRootGUID;
                            _OrgRootEntityGUID = ((_HouseManagementService.ExportSupplyResourceContractTypeOrganization)tmpExportSupplyResourceContract.Item1).orgRootEntityGUID;
                            #region Экспорт информации о второй стороне договора
                            foreach(var itemOrgRegistry in helperOrganizationRegistryCommonService.GetOrgRegistry(_OrgRootEntityGUID).exportOrgRegistryResult.Items)
                            {
                                var ResultTypeOrgVersion = itemOrgRegistry as _OrganizationRegistryCommonService.exportOrgRegistryResultType;
                                var ItemLegalType = ResultTypeOrgVersion.OrgVersion.Item as _OrganizationRegistryCommonService.LegalType;
                                Console.WriteLine("Organization full name: {0}", ItemLegalType.FullName);
                                _OrgRootOrganizationINN = ItemLegalType.INN;
                            }
                            #endregion
                            #region Экспорт состояния оплаты
                            foreach (var tmpExportSettlements in helperBillService.GetSettlements(_orgPPAGUID, _ContractRootGUID).exportSettlementsResult.Items)
                            {
                                if (tmpExportSettlements.GetType() == typeof(_BillsService.ErrorMessageType)
                                    && ((_BillsService.ErrorMessageType)tmpExportSettlements).ErrorCode != "INT002012")
                                {
                                    var ErrorMessage = tmpExportSettlements as _BillsService.ErrorMessageType;
                                    BaseClasses.OutputError(ErrorMessage.ErrorCode, ErrorMessage.Description);
                                }
                                else
                                {
                                    if (tmpExportSettlements.GetType() != typeof(_BillsService.ErrorMessageType))
                                    {
                                        var resultExportSettlements = tmpExportSettlements as _BillsService.ExportSettlementResultType;
                                        var reportingPeriodExportSettlements = resultExportSettlements.ReportingPeriod as _BillsService.ExportSettlementResultTypeReportingPeriod[];
                                        isExistValue = reportingPeriodExportSettlements
                                            .Any(obj => obj.ReportPeriodStatus.Status == _BillsService.ExportSettlementResultTypeReportingPeriodReportPeriodStatusStatus.Posted
                                            && obj.Month == DateTime.Now.AddMonths(-2).Month
                                            && obj.Year == DateTime.Now.AddMonths(-2).Year);
                                        _SettlementGUID = resultExportSettlements.SettlementGUID;
                                        _ContractRootGUID = null;
                                    }
                                    else
                                    {
                                        _SettlementGUID = null;
                                        isExistValue = false;
                                    }
                                    if (isExistValue)
                                    {
                                        BaseClasses.OutputMessage(_OrgRootOrganizationINN + " Data for the period " + DateTime.Now.AddMonths(-2).Month + "." + DateTime.Now.AddMonths(-2).Year + " already exist");
                                    }
                                    else
                                    {
                                        #region Получение сведений о расчетах
                                        string connectionString = @"Data Source=storage;Initial Catalog=master;User Id=" + ConfigurationManager.AppSettings["db_login"] + ";Password=" + ConfigurationManager.AppSettings["db_pass"];
                                        string sqlExpression = @"WITH t1
                                                                 AS (
                                                                 	SELECT 
                                                                 		CASE 
                                                                 			WHEN [BAccount_ID] = 62.0
                                                                 				THEN d.[InvSum] + d.[InvVat]
                                                                 			ELSE 0
                                                                 			END AS TotalDebSum
                                                                 		,CASE 
                                                                 			WHEN [BAccount_ID] = 62.5
                                                                 				THEN d.[InvSum] + d.[InvVat]
                                                                 			ELSE 0
                                                                 			END AS TotalAvansSum
                                                                 		,CASE 
                                                                 			WHEN p.PayoffType = 0
                                                                 				THEN p.PayoffSum
                                                                 			ELSE 0
                                                                 			END AS TotalPayoffSum
                                                                 	FROM [VKAbon].[dbo].[Debit] d
                                                                 	LEFT JOIN [VKAbon].[dbo].[Abon_Details] ad ON d.Abon_ID = ad.Abon_ID
                                                                 	LEFT JOIN (
                                                                 		SELECT Debit_ID
                                                                 			,SUM(PayoffSum) AS PayoffSum
                                                                 			,PayoffType
                                                                 		FROM [VKAbon].[dbo].Payoff
                                                                 		GROUP BY Debit_ID
                                                                 			,PayoffType
                                                                 		) p ON d.Debit_ID = p.Debit_ID
                                                                 	WHERE ad.INN = @INN
                                                                 		AND YEAR(DebitDate) = @yearnumber
                                                                 		AND MONTH([DebitDate]) = @monthnumber
                                                                 		AND ad.DEnd = '2079-01-01'
                                                                 	)
                                                                 SELECT
                                                                 	SUM(TotalDebSum) AS TotalDebSum
                                                                 	,SUM(TotalAvansSum) AS TotalAvansSum
                                                                 	,SUM(TotalPayoffSum) AS TotalPayoffSum
                                                                 	,SUM(TotalDebSum) - SUM(TotalPayoffSum) - SUM(TotalAvansSum) AS TotalSaldo
                                                                 FROM t1
                                                                 HAVING
                                                                 SUM(TotalDebSum) IS NOT NULL
                                                                 AND SUM(TotalAvansSum) IS NOT NULL
                                                                 AND SUM(TotalPayoffSum) IS NOT NULL
                                                                 AND SUM(TotalDebSum) - SUM(TotalPayoffSum) - SUM(TotalAvansSum) IS NOT NULL";
                                        using (SqlConnection connection = new SqlConnection(connectionString))
                                        {
                                            connection.Open();
                                            SqlCommand command = new SqlCommand(sqlExpression, connection);
                                            SqlParameter _INN = new SqlParameter("@INN", _OrgRootOrganizationINN);
                                            command.Parameters.Add(_INN);
                                            SqlParameter _Month = new SqlParameter("@monthnumber", DateTime.Now.AddMonths(-2).Month);
                                            command.Parameters.Add(_Month);
                                            SqlParameter _Year = new SqlParameter("@yearnumber", DateTime.Now.AddMonths(-2).Year);
                                            command.Parameters.Add(_Year);
                                            SqlDataReader reader = command.ExecuteReader();
                                            if (reader.HasRows)
                                            {
                                                while (reader.Read())
                                                {
                                                    _TotalDebSum = Convert.ToDecimal(reader["TotalDebSum"]);
                                                    _TotalAvansSum = Convert.ToDecimal(reader["TotalAvansSum"]);
                                                    _TotalPayoffSum = Convert.ToDecimal(reader["TotalPayoffSum"]);
                                                    _TotalSaldo = Convert.ToDecimal(reader["TotalSaldo"]);
                                                }
                                                #region Импорт состояния расчетов за период
                                                foreach (var itemResultImportSettlement in helperBillService.SetRSOSettlements(_orgPPAGUID, _ContractRootGUID, _SettlementGUID, DateTime.Now.AddMonths(-2), _TotalDebSum, _TotalPayoffSum, _TotalSaldo, _TotalAvansSum).ImportResult.Items)
                                                {
                                                    if (itemResultImportSettlement.GetType() == typeof(_BillsService.ErrorMessageType))
                                                    {
                                                        var ErrorMessage = itemResultImportSettlement as _BillsService.ErrorMessageType;
                                                        BaseClasses.OutputError(ErrorMessage.ErrorCode, ErrorMessage.Description);
                                                    }
                                                    else
                                                    {
                                                        var ResultType = itemResultImportSettlement as _BillsService.CommonResultType;
                                                        if (ResultType.Items[0].GetType() == typeof(_BillsService.CommonResultTypeError))
                                                        {
                                                            var ResultTypeError = ResultType.Items[0] as _BillsService.CommonResultTypeError;
                                                            BaseClasses.OutputError(ResultTypeError.ErrorCode, ResultTypeError.Description);
                                                        }
                                                        else
                                                        {
                                                            BaseClasses.OutputMessage(_OrgRootOrganizationINN + " Data import " + ResultType.Items[0]);
                                                        }
                                                    }
                                                }
                                                #endregion
                                            }
                                            else
                                            {
                                                BaseClasses.OutputMessage(_OrgRootOrganizationINN + " Settlements in our system is absent");
                                            }
                                            reader.Close();
                                            connection.Close();
                                        }
                                        #endregion
                                    }
                                }
                            }
                            #endregion
                            Console.WriteLine("-----------------------");
                        }
                    }
                    else
                    {
                        _ExportContractRootGUID = tmpSupplyResourceContract.ToString();
                    }
                }
            }
            while (typeRes != "True");

            Console.Read();
        }
    }
}