using InterviewApp.Crm.Common.Entities;
using InterviewApp.Crm.Common.Services;
using Microsoft.Xrm.Sdk;
using System;

namespace InterviewApp.Crm.Plugins
{
    [CrmPluginRegistration("Update", "new_candidate", StageEnum.PostOperation,
        ExecutionModeEnum.Asynchronous, "",
        "InterviewApp.Crm.Plugins.RejectOtherCandidatesAfterHiring: Update of new_candidate", 1, IsolationModeEnum.None,
        Image1Type = ImageTypeEnum.PostImage, Image1Name = "PostImage", Image1Attributes = "",
        Description = "InterviewApp.Crm.Plugins.RejectOtherCandidatesAfterHiring: Update of new_candidate",
        Id = "")]
    [CrmPluginRegistration("SetState", "new_candidate", StageEnum.PostOperation, ExecutionModeEnum.Asynchronous,
        "", "InterviewApp.Crm.Plugins.RejectOtherCandidatesAfterHiring: SetState of new_candidate", 1,
        IsolationModeEnum.None, Image1Type = ImageTypeEnum.PostImage, Image1Name = "PostImage", Image1Attributes = "",
        Description = "InterviewApp.Crm.Plugins.RejectOtherCandidatesAfterHiring: SetState of new_candidate",
        Id = "")]

    public class RejectOtherCandidatesAfterHiring : PluginBase
    {
        protected override void ExecuteCrmPlugin(LocalPluginContext localContext)
        {
            if (localContext == null)
                throw new ArgumentNullException(nameof(localContext));

            if (!(localContext.PluginExecutionManager.IsUpdate &&
                  localContext.PluginExecutionManager.IsPostEventPostOperation ||
                  localContext.PluginExecutionManager.IsSetState &&
                  localContext.PluginExecutionManager.IsPostEventPostOperation))
            {
                throw new InvalidPluginExecutionException("Plugin executed on incorrect Message/Pipeline");
            }

            var postImage = localContext.PluginExecutionManager.GetPostImage<Entity>("PostImage");

            if (postImage.LogicalName != Candidate.EntityLogicalName)
            {
                throw new InvalidPluginExecutionException("Plugin executed on incorrect Entity");
            }

            var candidateService = new CandidateService(localContext.OrganizationService);
            candidateService.RejectOtherCandidates(postImage);
            localContext.Trace("PostImage process");
        }

        #region Constructor/Configuration
        private string _secureConfig = null;
        private string _unsecureConfig = null;

        public RejectOtherCandidatesAfterHiring(string unsecure, string secureConfig)
            : base(typeof(RejectOtherCandidatesAfterHiring))
        {
            _secureConfig = secureConfig;
            _unsecureConfig = unsecure;
        }
        #endregion
    }
}