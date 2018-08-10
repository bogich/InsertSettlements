using System;
using Gis.Crypto;
using Gis.Infrastructure.BillsService;
using System.Configuration;
using System.Threading;
using System.Collections.Generic;

namespace Gis.Helpers.HelperBillService
{
    /// <summary>
    /// Сервис обмена сведениями о начислениях, взаиморасчетах
    /// </summary>
    class HelperBillService
    {
        /// <summary>
        /// ВИ_ИЛС_ПД_ЭКСП. экспорт платежных документов
        /// </summary>
        /// <param name="_orgPPAGUID">Идентификатор зарегистрированной организации</param>
        /// <param name="_Year">Год</param>
        /// <param name="_Month">Месяц</param>
        /// <param name="_UnifiedAccountNumber">Единый лицевой счет</param>
        /// <returns></returns>
        public exportPaymentDocumentDataResponse GetPaymentDocumentData(string _orgPPAGUID, Int16 _Year, int _Month, string _UnifiedAccountNumber)
        {
            var srvBillsService = new BillsPortsTypeClient();
            srvBillsService.ClientCredentials.UserName.UserName = ConfigurationManager.AppSettings["_login"];
            srvBillsService.ClientCredentials.UserName.Password = ConfigurationManager.AppSettings["_pass"];

            var reqBillServiceExpDoc = new exportPaymentDocumentDataRequest
            {
                RequestHeader = new RequestHeader
                {
                    Date = DateTime.Now,
                    MessageGUID = Guid.NewGuid().ToString(),
                    ItemElementName = ItemChoiceType1.orgPPAGUID,
                    Item = _orgPPAGUID
                },
                exportPaymentDocumentRequest = new exportPaymentDocumentRequest
                {
                    version = "11.2.0.10",
                    ItemsElementName = new ItemsChoiceType3[]
                    {
                        ItemsChoiceType3.Year,
                        ItemsChoiceType3.Month,
                        ItemsChoiceType3.UnifiedAccountNumber
                    },
                    Items = new object[]
                    {
                        _Year,
                        _Month,
                        _UnifiedAccountNumber
                    }
                }
            };

            var resBillServiceExpDoc = srvBillsService.exportPaymentDocumentData(reqBillServiceExpDoc);

            return resBillServiceExpDoc;
        }

        /// <summary>
        /// Запрос на импорт платежных документов
        /// </summary>
        /// <param name="_orgPPAGUID">Идентификатор зарегистрированной организации</param>
        /// <param name="_accountGUID">Идентификатор лицевого счета</param>
        /// <returns></returns>
        public importPaymentDocumentDataResponse SetPaymentDocumentData(string _orgPPAGUID, string _accountGUID)
        {
            var srvBillsService = new BillsPortsTypeClient();
            srvBillsService.ClientCredentials.UserName.UserName = ConfigurationManager.AppSettings["_login"];
            srvBillsService.ClientCredentials.UserName.Password = ConfigurationManager.AppSettings["_pass"];

            var PITG = Guid.NewGuid().ToString();

            var reqBillsServiceImpDoc = new importPaymentDocumentDataRequest
            {
                RequestHeader = new RequestHeader
                {
                    Date = DateTime.Now,
                    MessageGUID = Guid.NewGuid().ToString(),
                    ItemElementName = ItemChoiceType1.orgPPAGUID,
                    Item = _orgPPAGUID
                },
                importPaymentDocumentRequest = new importPaymentDocumentRequest
                {
                    version = "11.2.0.16",
                    Items = new object[]
                    {
                        //true,
                        (int)DateTime.Now.Month, //(int)12, //Месяц
                        (short)DateTime.Now.Year, //(short)2017, //Год
                        new importPaymentDocumentRequestPaymentInformation
                        {
                            TransportGUID = PITG,
                            BankBIK = "046577904",
                            operatingAccountNumber = "40702810800010003850"
                        },
                        new importPaymentDocumentRequestPaymentDocument
                        {
                            TransportGUID = Guid.NewGuid().ToString(),
                            AccountGuid = _accountGUID, //GUID лицевого счета
                            PaymentInformationKey = PITG, //GUID платежных реквизитов
                            Item = true,
                            Items = new object[]
                            {
                                new PaymentDocumentTypeChargeInfo
                                {
                                    Item = new PDServiceChargeTypeMunicipalService
                                    {
                                        AccountingPeriodTotal = 100, //Всего начислено за расчетный период
                                        TotalPayable = 100, //Итого к оплате за расчетный период
                                        Rate = 10m, //Тариф
                                        ServiceType = new nsiRef
                                        {
                                            Code = "1.1",
                                            GUID = "e12e7b13-4e81-41b4-86d1-724fe9c0b874"
                                        },
                                        Consumption = new PDServiceChargeTypeMunicipalServiceVolume[]
                                        {
                                            new PDServiceChargeTypeMunicipalServiceVolume
                                            {

                                                typeSpecified = true,
                                                type = PDServiceChargeTypeMunicipalServiceVolumeType.I, //(I)ndividualConsumption - индивидульное потребление (O)verallNeeds - общедомовые нужды
                                                determiningMethodSpecified = true,
                                                determiningMethod = PDServiceChargeTypeMunicipalServiceVolumeDeterminingMethod.N, //(N)orm - Норматив (M)etering device - Прибор учета (O)ther - Иное
                                                Value = 10m
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            };

            var resBillServiceImpDoc = srvBillsService.importPaymentDocumentData(reqBillsServiceImpDoc);

            return resBillServiceImpDoc;
        }

        /// <summary>
        /// Экспорт информации о расчетах по ДРСО
        /// </summary>
        /// <param name="_orgPPAGUID">Идентификатор зарегистрированной организации</param>
        /// <param name="_ContractRootGUID">Идентификатор договора ресурсоснабжения в ГИС ЖКХ</param>
        /// <returns></returns>
        public exportSettlementsResponse GetSettlements(string _orgPPAGUID, string _ContractRootGUID)
        {
            var srvBillsService = new BillsPortsTypeClient();
            srvBillsService.ClientCredentials.UserName.UserName = ConfigurationManager.AppSettings["_login"];
            srvBillsService.ClientCredentials.UserName.Password = ConfigurationManager.AppSettings["_pass"];

            var reqExportSettlements = new exportSettlementsRequest1
            {
                RequestHeader = new RequestHeader
                {
                    Date = DateTime.Now,
                    MessageGUID = Guid.NewGuid().ToString(),
                    ItemElementName = ItemChoiceType1.orgPPAGUID,
                    Item = _orgPPAGUID
                },
                exportSettlementsRequest = new exportSettlementsRequest
                {
                    version = "10.0.2.1",
                    Id = CryptoConsts.CONTAINER_ID,
                    ItemsElementName = new ItemsChoiceType10[]
                    {
                        ItemsChoiceType10.ContractRootGUID
                    },
                    Items = new object[]
                    {
                        _ContractRootGUID
                    }
                }
            };

            exportSettlementsResponse resExportSettlementsResponse = null;
            do
            {
                try
                {
                    resExportSettlementsResponse = srvBillsService.exportSettlements(reqExportSettlements);
                }
                catch (Exception e)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine(e.Message);
                    Console.ResetColor();
                    Thread.Sleep(1000);

                    if(e.Message.Contains("EXP002002"))
                    {
                        reqExportSettlements.RequestHeader.MessageGUID = Guid.NewGuid().ToString();
                    }
                }
            }
            while (resExportSettlementsResponse is null);

            return resExportSettlementsResponse;
        }

        /// <summary>
        /// Импорт информации о состоянии расчетов от имени РСО
        /// </summary>
        /// <param name="_orgPPAGUID">Идентификатор зарегистрированной организации/param>
        /// <param name="_ContractRootGUID">Идентификатор договора ресурсоснабжения в ГИС ЖКХ. Исключает параметр _SettlementGUID</param>
        /// <param name="_SettlementGUID">Идентификатор информации о состоянии расчетов (обязателен при изменении или аннулировании информации о состоянии расчетов). Исключает параметр _ContractRootGUID</param>
        /// <param name="_Period">Период (ММ.ГГГГ)</param>
        /// <param name="_Credted">Начислено</param>
        /// <param name="_Debts">Поступило</param>
        /// <param name="_Receipt">Задолженность</param>
        /// <param name="_Overpayment">Переплата</param>
        /// <returns></returns>
        public importRSOSettlementsResponse SetRSOSettlements(string _orgPPAGUID, string _ContractRootGUID, string _SettlementGUID, DateTime _Period, decimal _Credted, decimal _Receipt, decimal _Debts, decimal _Overpayment)
        {
            var srvBillsService = new BillsPortsTypeClient();
            srvBillsService.ClientCredentials.UserName.UserName = ConfigurationManager.AppSettings["_login"];
            srvBillsService.ClientCredentials.UserName.Password = ConfigurationManager.AppSettings["_pass"];

            List<importRSOSettlementsRequestImportSettlement> ListImportRSOSettlementsRequestImportSettlement = new List<importRSOSettlementsRequestImportSettlement>();

            if((_ContractRootGUID is null) && !(_SettlementGUID is null))
            {
                ListImportRSOSettlementsRequestImportSettlement.Add(
                    new importRSOSettlementsRequestImportSettlement
                    {
                        TransportGUID = Guid.NewGuid().ToString(),
                        SettlementGUID = _SettlementGUID,
                        Item = new importRSOSettlementsRequestImportSettlementSettlement
                        {
                            ReportingPeriod = new importRSOSettlementsRequestImportSettlementSettlementReportingPeriod[]
                                {
                                    new importRSOSettlementsRequestImportSettlementSettlementReportingPeriod
                                    {
                                        Year = (short)_Period.Year,
                                        Month = _Period.Month,
                                        Item = new ReportPeriodRSOInfoType
                                        {
                                            Credted = _Credted,
                                            Receipt = _Receipt,
                                            Debts = _Debts,
                                            Overpayment = _Overpayment
                                        }
                                    }
                                }
                        }
                    }
                    );
            }

            if (!(_ContractRootGUID is null) && (_SettlementGUID is null))
            {
                ListImportRSOSettlementsRequestImportSettlement.Add(
                    new importRSOSettlementsRequestImportSettlement
                    {
                        TransportGUID = Guid.NewGuid().ToString(),
                        Item = new importRSOSettlementsRequestImportSettlementSettlement
                        {
                            Contract = new importRSOSettlementsRequestImportSettlementSettlementContract
                            {
                                ContractRootGUID = _ContractRootGUID
                            },
                            ReportingPeriod = new importRSOSettlementsRequestImportSettlementSettlementReportingPeriod[]
                                {
                                    new importRSOSettlementsRequestImportSettlementSettlementReportingPeriod
                                    {
                                        Year = (short)_Period.Year,
                                        Month = _Period.Month,
                                        Item = new ReportPeriodRSOInfoType
                                        {
                                            Credted = _Credted,
                                            Receipt = _Receipt,
                                            Debts = _Debts,
                                            Overpayment = _Overpayment
                                        }
                                    }
                                }
                        }
                    }
                    );
            }

                var reqImportSettlements = new importRSOSettlementsRequest1
            {
                RequestHeader = new RequestHeader
                {
                    Date = DateTime.Now,
                    MessageGUID = Guid.NewGuid().ToString(),
                    ItemElementName = ItemChoiceType1.orgPPAGUID,
                    Item = _orgPPAGUID
                },
                importRSOSettlementsRequest = new importRSOSettlementsRequest
                {
                    version = "10.0.2.1",
                    Id = CryptoConsts.CONTAINER_ID,
                    importSettlement = ListImportRSOSettlementsRequestImportSettlement.ToArray()
                }
            };

            importRSOSettlementsResponse resImportSettlementsResponse = null;
            do
            {
                try
                {
                    resImportSettlementsResponse = srvBillsService.importRSOSettlements(reqImportSettlements);
                }
                catch (Exception e)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine(e.Message);
                    Console.ResetColor();
                    Thread.Sleep(1000);
                }
            }
            while (resImportSettlementsResponse is null);

            return resImportSettlementsResponse;
        }
    }
}