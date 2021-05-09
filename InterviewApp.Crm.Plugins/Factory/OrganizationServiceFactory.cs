using Microsoft.Xrm.Sdk;
using System;

namespace InterviewApp.Plugin.Common
{
    public class OrganizationServiceFactory
    {
        public IOrganizationService CreateOrganizationService(IServiceProvider serviceProvider)
        {
            var context = (IPluginExecutionContext)serviceProvider.GetService(typeof(IPluginExecutionContext));
            var serviceFactory =
                (IOrganizationServiceFactory)serviceProvider.GetService(typeof(IOrganizationServiceFactory));
            var gateway = new CRMGateway(serviceFactory, context.UserId);
            return gateway.CreateOrganizationService();
        }
    }
}
