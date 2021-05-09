using Microsoft.Win32;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Client;
using System;
using System.Net;
using System.ServiceModel.Description;

namespace InterviewApp.Plugin.Common
{
    public class CRMGateway
    {
        private readonly Guid _callerId;
        private readonly IOrganizationServiceFactory _organizationServiceFactory;


        /// <summary>
        ///     Lazy implementation of organisation service associated with this gateway.
        /// </summary>
        private IOrganizationService _gatewayService;

        /// <summary>
        ///     CRM Gateway Constructor (Used by Web site if default credentials are speficied)
        /// </summary>
        /// <param name="organisationName">Name of the CRM Organisation</param>
        /// <param name="credentials">Credentials to be used when connecting to the CRM Server</param>
        public CRMGateway(string organisationName, ICredentials credentials)
        {
            if (string.IsNullOrEmpty(organisationName))
            {
                throw new ArgumentNullException(nameof(organisationName));
            }

            OrganisationName = organisationName;
            Credentials = credentials;
        }

        /// <summary>
        ///     CRM Gateway Constructor (Used primarily from plugins)
        /// </summary>
        /// <param name="organizationServiceFactory"> Organizationservice factory instantiated from the plugin Iserviceprovider</param>
        /// <param name="callerId">Id of the calling user</param>
        public CRMGateway(IOrganizationServiceFactory organizationServiceFactory, Guid callerId)
        {
            _organizationServiceFactory = organizationServiceFactory ??
                                          throw new ArgumentNullException(nameof(organizationServiceFactory));
            _callerId = callerId;
        }

        /// <summary>
        ///     CRM Gateway Constructor (Used by Windows Services, Web site & Standalone applications.)
        /// </summary>
        /// <param name="serverName"> string Servername </param>
        /// <param name="port"> int Port number</param>
        /// <param name="organisationName">Name of the CRM Organisation</param>
        /// <param name="credentials">Credentials to be used when connecting to the CRM Server</param>
        public CRMGateway(string serverName, int port, string organisationName, ICredentials credentials)
        {
            if (string.IsNullOrEmpty(organisationName))
            {
                throw new ArgumentNullException(nameof(organisationName));
            }

            if (port == 0)
            {
                throw new ArgumentNullException(nameof(port));
            }

            if (string.IsNullOrEmpty(serverName))
            {
                throw new ArgumentNullException(nameof(serverName));
            }

            OrganisationName = organisationName;
            Credentials = credentials;
            ServerName = serverName;
            Port = port;
        }

        public string ServerName { get; }
        public int Port { get; }
        public string OrganisationName { get; }
        public ICredentials Credentials { get; }

        public IOrganizationService GatewayService =>
            _gatewayService ?? (_gatewayService = CreateOrganizationService());

        public IOrganizationService CreateOrganizationService()
        {
            if (_organizationServiceFactory != null)
            {
                return _organizationServiceFactory.CreateOrganizationService(_callerId);
            }

            var clientCredentials = new ClientCredentials();
            clientCredentials.Windows.ClientCredential = (NetworkCredential)Credentials;

            return CreateIOrganizationService(clientCredentials);
        }

        private IOrganizationService CreateIOrganizationService(ClientCredentials clientCredentials)
        {
            Uri organizationUri;

            if (!string.IsNullOrEmpty(ServerName) && Port != 0)
            {
                organizationUri =
                    new Uri($"http://{ServerName}:{Port}/{OrganisationName}/XRMServices/2011/Organization.svc");
            }
            else
            {
                // Read registry key on 64 bit OS
                var localKey = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry64);
                localKey = localKey.OpenSubKey(@"SOFTWARE\Microsoft\MSCRM");

                var serverUrl = localKey?.GetValue("ServerUrl").ToString();

                if (string.IsNullOrEmpty(serverUrl))
                {
                    throw new InvalidOperationException("ServerUrl was found to be null or empty");
                }

                organizationUri = new Uri(string.Concat(serverUrl.Replace("MSCRMServices", OrganisationName),
                    "/XRMServices/2011/Organization.svc"));
            }

            var orgService = new OrganizationServiceProxy(organizationUri, null, clientCredentials, null);

            //This line is required to enable Early Bound Types
            orgService.EnableProxyTypes();
            orgService.Timeout = new TimeSpan(0, 9, 0);

            return orgService;
        }
    }
}
