using InterviewApp.Crm.Common.Controllers.BaseClasses;
using InterviewApp.Crm.Common.Entities;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using System;
using System.Collections.Generic;

namespace InterviewApp.Crm.Common.Controllers
{
    public class CandidateController : ControllerBase<Candidate>
    {
        internal CandidateController(IOrganizationService service) : base(service)
        {
        }

        public List<Entity> GetActiveCandidatesForRole(EntityReference roleRef)
        {
            if (roleRef == null)
            {
                throw new ArgumentNullException(nameof(roleRef));
            }

            if (roleRef.LogicalName != Role.EntityLogicalName)
            {
                throw new ArgumentException(nameof(roleRef));
            }

            var pageNumber = 1;
            var pagingCookie = string.Empty;
            EntityCollection resp;
            var result = new List<Entity>();

            do
            {
                var query = new QueryExpression(Candidate.EntityLogicalName) { ColumnSet = DefaultColumnSet };

                query.Criteria.AddCondition(new ConditionExpression(Candidate.AppliedRole, ConditionOperator.Equal,
                    roleRef.Id));
                query.Criteria.AddCondition(Candidate.StateCode, ConditionOperator.Equal,
                    (int)Candidate.State.Active);

                query.PageInfo = new PagingInfo { PageNumber = 1, Count = 5000 };
                if (pageNumber != 1)
                {
                    query.PageInfo.PageNumber = pageNumber;
                    query.PageInfo.PagingCookie = pagingCookie;
                }

                resp = Service.RetrieveMultiple(query);
                if (resp.MoreRecords)
                {
                    pageNumber++;
                    pagingCookie = resp.PagingCookie;
                }

                result.AddRange(resp.Entities);
            } while (resp.MoreRecords);

            return result;
        }

        public void DeactivateCandidate(Entity activeCandidate)
        {
            var updateEntity = new Entity(Candidate.EntityLogicalName) { Id = activeCandidate.Id };

            updateEntity.Attributes.Add(Candidate.StateCode, new OptionSetValue((int)Candidate.State.Inactive));
            updateEntity.Attributes.Add(Candidate.StatusCode, new OptionSetValue((int)Candidate.Status.Rejected));

            Service.Update(updateEntity);
        }
    }
}
