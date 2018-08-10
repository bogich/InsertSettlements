using System;
using Gis.Crypto;
using Gis.Infrastructure.HouseManagementService;
using System.Threading;
using System.Configuration;
using System.Runtime.InteropServices;

namespace Gis.Helpers.HelperHouseManagementService
{
    /// <summary>
    /// 2.2.6 Сервис обмена сведениями о жилищном фонде
    /// </summary>
    class HelperHouseManagementService
    {
        /// <summary>
        /// Экспорт договоров ресурсоснабжения
        /// </summary>
        /// <param name="_orgPPAGUID">
        /// Идентификатор зарегистрированной организации
        /// </param>
        /// <param name="_ExportContractRootGUID">
        /// Корневой идентификатор договора ресурсоснабжения в ГИС ЖКХ для установки экспорта следующей 1000 договоров При первичном экспорте не заполняется
        /// </param>
        /// <returns></returns>
        public exportSupplyResourceContractDataResponse GetSupplyResourceContractData(string _orgPPAGUID, [Optional] string _ExportContractRootGUID)
        {
            var srvHouseMgmt = new HouseManagementPortsTypeClient();
            srvHouseMgmt.ClientCredentials.UserName.UserName = ConfigurationManager.AppSettings["_login"];
            srvHouseMgmt.ClientCredentials.UserName.Password = ConfigurationManager.AppSettings["_pass"];

            var reqHouseMgmt = new exportSupplyResourceContractDataRequest
            {
                RequestHeader = new RequestHeader
                {
                    Date = DateTime.Now,
                    MessageGUID = Guid.NewGuid().ToString(),
                    ItemElementName = ItemChoiceType2.orgPPAGUID,
                    Item = _orgPPAGUID
                },
                exportSupplyResourceContractRequest = new exportSupplyResourceContractRequest
                {
                    version = "11.11.0.2",
                    Id = CryptoConsts.CONTAINER_ID,
                    ItemsElementName = new ItemsChoiceType22[]
                    {
                        ItemsChoiceType22.ExportContractRootGUID
                    },
                    Items = new object[]
                    {
                        _ExportContractRootGUID
                    }
                }
            };

            exportSupplyResourceContractDataResponse resHouseMgmt = null;
            do
            {
                try
                {
                    resHouseMgmt = srvHouseMgmt.exportSupplyResourceContractData(reqHouseMgmt);
                }
                catch (Exception e)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine(e.Message);
                    Console.ResetColor();
                    Thread.Sleep(1000);
                }
            }
            while (resHouseMgmt is null);

            return resHouseMgmt;
        }

        /// <summary>
        /// Экспорт объектов жилищного фонда из договоров ресурсоснабжения
        /// </summary>
        /// <param name="_orgPPAGUID">
        /// Идентификатор зарегистрированной организации
        /// </param>
        /// <param name="_ContractRootGUID">
        /// Идентификатор договора ресурсоснабжения в ГИС ЖКХ
        /// </param>
        /// <returns></returns>
        public exportSupplyResourceContractObjectAddressDataResponse GetSupplyResourceContractObjectAddressData(string _orgPPAGUID, string _ContractRootGUID)
        {
            var srvHouseMgmt = new HouseManagementPortsTypeClient();
            srvHouseMgmt.ClientCredentials.UserName.UserName = ConfigurationManager.AppSettings["_login"];
            srvHouseMgmt.ClientCredentials.UserName.Password = ConfigurationManager.AppSettings["_pass"];

            var reqHouseMgmtExportObjectAddressData = new exportSupplyResourceContractObjectAddressDataRequest
            {
                RequestHeader = new RequestHeader
                {
                    Date = DateTime.Now,
                    MessageGUID = Guid.NewGuid().ToString(),
                    ItemElementName = ItemChoiceType2.orgPPAGUID,
                    Item = _orgPPAGUID
                },
                exportSupplyResourceContractObjectAddressRequest = new exportSupplyResourceContractObjectAddressRequest
                {
                    version = "11.6.0.1",
                    Id = CryptoConsts.CONTAINER_ID,
                    ItemsElementName = new ItemsChoiceType26[]
                    {
                        ItemsChoiceType26.ContractRootGUID
                    },
                    Items = new string[]
                    {
                        _ContractRootGUID
                    }
                }
            };

            exportSupplyResourceContractObjectAddressDataResponse resHouseMgmtExportObjectAddressData = null;
            do
            {
                try
                {
                    resHouseMgmtExportObjectAddressData = srvHouseMgmt.exportSupplyResourceContractObjectAddressData(reqHouseMgmtExportObjectAddressData);
                }
                catch (Exception e)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine(e.Message);
                    Console.ResetColor();
                    Thread.Sleep(1000);
                }
            }
            while (resHouseMgmtExportObjectAddressData is null);

            return resHouseMgmtExportObjectAddressData;
        }

        /// <summary>
        /// Экспорт лицевых счетов
        /// </summary>
        /// <param name="_orgPPAGUID">
        /// Идентификатор зарегистрированной организации
        /// </param>
        /// <param name="_FIASHouseGuid">
        /// Глобальный уникальный идентификатор дома по ФИАС
        /// </param>
        /// <returns></returns>
        public exportAccountDataResponse GetAccountData(string _orgPPAGUID, string _FIASHouseGuid)
        {
            var srvHouseMgmt = new HouseManagementPortsTypeClient();
            srvHouseMgmt.ClientCredentials.UserName.UserName = ConfigurationManager.AppSettings["_login"];
            srvHouseMgmt.ClientCredentials.UserName.Password = ConfigurationManager.AppSettings["_pass"];

            var reqHouseMgmtExpAcc = new exportAccountDataRequest
            {
                RequestHeader = new RequestHeader
                {
                    Date = DateTime.Now,
                    MessageGUID = Guid.NewGuid().ToString(),
                    ItemElementName = ItemChoiceType2.orgPPAGUID,
                    Item = _orgPPAGUID
                },
                exportAccountRequest = new exportAccountRequest
                {
                    version = "10.0.1.1",
                    Id = CryptoConsts.CONTAINER_ID,
                    ItemsElementName = new ItemsChoiceType10[]
                    {
                        ItemsChoiceType10.FIASHouseGuid
                    },
                    Items = new string[]
                    {
                        _FIASHouseGuid
                    }
                }
            };

            exportAccountDataResponse resHouseMgmtExpAcc = null;
            do
            {
                try
                {
                    resHouseMgmtExpAcc = srvHouseMgmt.exportAccountData(reqHouseMgmtExpAcc);
                }
                catch (Exception e)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine(e.Message);
                    Console.ResetColor();
                    Thread.Sleep(1000);
                }
            }
            while (resHouseMgmtExpAcc is null);

            return resHouseMgmtExpAcc;
        }

        /// <summary>
        /// Экспорт ПУ
        /// </summary>
        /// <param name="_orgPPAGUID">
        /// Идентификатор зарегистрированной организации
        /// </param>
        /// <param name="_FIASHouseGuid">
        /// Глобальный уникальный идентификатор дома по ФИАС
        /// </param>
        /// <returns></returns>
        public exportMeteringDeviceDataResponse GetMeteringDeviceData(string _orgPPAGUID, string _FIASHouseGuid)
        {
            var srvHouseMgmt = new HouseManagementPortsTypeClient();
            srvHouseMgmt.ClientCredentials.UserName.UserName = ConfigurationManager.AppSettings["_login"];
            srvHouseMgmt.ClientCredentials.UserName.Password = ConfigurationManager.AppSettings["_pass"];

            var reqHouseMgmt = new exportMeteringDeviceDataRequest1
            {
                RequestHeader = new RequestHeader
                {
                    Date = DateTime.Now,
                    MessageGUID = Guid.NewGuid().ToString(),
                    ItemElementName = ItemChoiceType2.orgPPAGUID,
                    Item = _orgPPAGUID
                },
                exportMeteringDeviceDataRequest = new exportMeteringDeviceDataRequest
                {
                    version = "11.1.0.2",
                    Id = CryptoConsts.CONTAINER_ID,
                    ItemsElementName = new[]
                    {
                        ItemsChoiceType4.FIASHouseGuid
                    },
                    Items = new[]
                    {
                        _FIASHouseGuid
                    }
                }
            };

            exportMeteringDeviceDataResponse resHouseMgmt = null;
            do
            {
                try
                {
                    resHouseMgmt = srvHouseMgmt.exportMeteringDeviceData(reqHouseMgmt);
                }
                catch (Exception e)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine(e.Message);
                    Console.ResetColor();
                    Thread.Sleep(1000);
                }
            }
            while (resHouseMgmt is null);

            return resHouseMgmt;
        }

        /// <summary>
        /// Импорт новостей для информирования граждан
        /// </summary>
        /// <param name="_orgPPAGUID">
        /// Идентификатор зарегистрированной организации
        /// </param>
        /// <param name="_FIASHouseGuid">
        /// Глобальный уникальный идентификатор дома по ФИАС
        /// </param>
        /// <returns>
        /// Статус обработки импорта данных при синхронном обмене
        /// </returns>
        public importNotificationDataResponse SetNotificationData(string _orgPPAGUID, string _FIASHouseGuid, string _content)
        {
            var srvHouseMgmt = new HouseManagementPortsTypeClient();
            srvHouseMgmt.ClientCredentials.UserName.UserName = ConfigurationManager.AppSettings["_login"];
            srvHouseMgmt.ClientCredentials.UserName.Password = ConfigurationManager.AppSettings["_pass"];

            var reqHouseMgmtNotificationfData = new importNotificationDataRequest
            {
                RequestHeader = new RequestHeader
                {
                    Date = DateTime.Now,
                    MessageGUID = Guid.NewGuid().ToString(),
                    ItemElementName = ItemChoiceType2.orgPPAGUID,
                    Item = _orgPPAGUID
                },
                importNotificationRequest = new importNotificationRequest
                {
                    version = "11.6.0.2",
                    Id = CryptoConsts.CONTAINER_ID,
                    notification = new importNotificationRequestNotification[]
                    {
                        new importNotificationRequestNotification
                        {
                            TransportGUID = Guid.NewGuid().ToString(),
                            Item = new importNotificationRequestNotificationCreate
                            {
                                Topic = "Поверка прибора учета",
                                content = _content,
                                IsShipOffSpecified = true,
                                IsShipOff = true, //Направить новость адресатам
                                ItemsElementName = new ItemsChoiceType14[]
                                {
                                    ItemsChoiceType14.FIASHouseGuid //Отпарвляем новость жителям дома
                                },
                                Items = new object[]
                                {
                                    _FIASHouseGuid //FIASGUID дома, для которого отправляем новость
                                },
                                Items1ElementName = new Items1ChoiceType[]
                                {
                                    Items1ChoiceType.IsNotLimit //Период актуальности - не ограничено
                                    //Items1ChoiceType.StartDate,
                                    //Items1ChoiceType.EndDate
                                },
                                Items1 = new object[]
                                {
                                    true
                                    //DateTime.Now,
                                    //DateTime.Now.AddDays(60)
                                }
                            }
                        }
                    }
                }
            };

            importNotificationDataResponse resHouseMgmt = null;
            do
            {
                try
                {
                    resHouseMgmt = srvHouseMgmt.importNotificationData(reqHouseMgmtNotificationfData);
                }
                catch (Exception e)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine(e.Message);
                    Console.ResetColor();
                    Thread.Sleep(1000);
                }
            }
            while (resHouseMgmt is null);

            return resHouseMgmt;
        }
    }
}