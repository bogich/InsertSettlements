using System;
using System.Configuration;
using System.Threading;
using Gis.Infrastructure.OrganizationsRegistryCommonService;
using Gis.Crypto;

namespace Gis.Helpers.HelperOrganizationRegistryCommonService
{
    class HelperOrganizationRegistryCommonService
    {
        /// <summary>
        /// Экспорт сведений из реестра организаций
        /// </summary>
        /// <param name="_orgRootEntityGUID">
        /// Идентификатор корневой сущности организации в реестре организаций
        /// </param>
        /// <returns></returns>
        public exportOrgRegistryResponse GetOrgRegistry(string _orgRootEntityGUID)
        {
            var srvOrgRegistry = new RegOrgPortsTypeClient();
            srvOrgRegistry.ClientCredentials.UserName.UserName = ConfigurationManager.AppSettings["_login"];
            srvOrgRegistry.ClientCredentials.UserName.Password = ConfigurationManager.AppSettings["_pass"];

            var reqOrgRegistry = new exportOrgRegistryRequest1
            {
                ISRequestHeader = new ISRequestHeader
                {
                    MessageGUID = Guid.NewGuid().ToString(),
                    Date = DateTime.Now
                },
                exportOrgRegistryRequest = new exportOrgRegistryRequest
                {
                    version = "10.0.2.1",
                    Id = CryptoConsts.CONTAINER_ID,
                    SearchCriteria = new exportOrgRegistryRequestSearchCriteria[]
                    {
                        new exportOrgRegistryRequestSearchCriteria
                        {
                            ItemsElementName = new ItemsChoiceType3[]
                            {
                                ItemsChoiceType3.orgRootEntityGUID
                            },
                            Items = new string[]
                            {
                                _orgRootEntityGUID
                            }
                        }
                    }
                }
            };

            exportOrgRegistryResponse resOrgRegistry = null;
            do
            {
                try
                {
                    resOrgRegistry = srvOrgRegistry.exportOrgRegistry(reqOrgRegistry);
                }
                catch (Exception e)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine(e.Message);
                    Console.ResetColor();
                    Thread.Sleep(1000);
                }
            }
            while (resOrgRegistry is null);

            return resOrgRegistry;
        }
    }
}
