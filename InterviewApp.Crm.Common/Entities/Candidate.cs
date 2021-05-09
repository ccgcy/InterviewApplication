using InterviewApp.Crm.Common.Entities.BaseClasses;

namespace InterviewApp.Crm.Common.Entities
{
    [SchemaName(EntityLogicalName)]
    public class Candidate : EntityBase<Candidate>
    {
        public const string EntityLogicalName = "new_candidate";

        public const string StateCode = "statecode";
        public const string StatusCode = "statuscode";
        public const string AppliedRole = "new_appliedrole";

        [SchemaName("statecode")]
        public enum State
        {
            Active = 0,
            Inactive = 1
        }

        [SchemaName("statuscode")]
        public enum Status
        {
            Open = 100000003,
            OnHold = 100000004,
            ApplicationChecked = 1,
            Interview = 100000001,
            Shortlisted = 100000000,
            Rejected = 2,
            Hired = 100000002
        }
    }
}
