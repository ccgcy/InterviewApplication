using Microsoft.Xrm.Sdk;

namespace InterviewApp.Plugin.Common
{
    public class PluginExecutionManagerFactory
    {
        public PluginExecutionManager Create(IPluginExecutionContext context)
        {
            return new PluginExecutionManager(context, context.PrimaryEntityName);
        }
    }
}
