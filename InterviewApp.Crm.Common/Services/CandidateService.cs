using InterviewApp.Crm.Common.Controllers;
using InterviewApp.Crm.Common.Entities;
using InterviewApp.Crm.Common.Services.BaseClasses;
using Microsoft.Xrm.Sdk;
using System;

namespace InterviewApp.Crm.Common.Services
{
    public class CandidateService : ServiceBase<CandidateController>
    {
        public CandidateService(IOrganizationService service) : base(service)
        {
        }

        public void RejectOtherCandidates(Entity candidate)
        {
            if (candidate is null)
            {
                throw new ArgumentNullException(nameof(candidate));
            }

            if (candidate.LogicalName != Candidate.EntityLogicalName)
            {
                throw new ArgumentException(nameof(candidate));
            }

            var candidateState = candidate.GetAttributeValue<OptionSetValue>(Candidate.StateCode).Value;
            var applicationStage = candidate.GetAttributeValue<OptionSetValue>(Candidate.StatusCode).Value;

            if (candidateState == (int)Candidate.State.Inactive && applicationStage == (int)Candidate.Status.Hired
                && candidate.Contains(Candidate.AppliedRole))
            {
                var appliedRole = candidate.GetAttributeValue<EntityReference>(Candidate.AppliedRole);
                var otherActiveCandidates = Controller.GetActiveCandidatesForRole(appliedRole);

                if (otherActiveCandidates != null && otherActiveCandidates.Count > 0)
                {
                    foreach (var activeCandidate in otherActiveCandidates)
                    {
                        Controller.DeactivateCandidate(activeCandidate);
                    }
                }
            }
        }
    }
}
