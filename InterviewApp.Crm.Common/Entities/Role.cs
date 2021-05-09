using InterviewApp.Crm.Common.Entities.BaseClasses;

namespace InterviewApp.Crm.Common.Entities
{
    [SchemaName(EntityLogicalName)]
    public class Role : EntityBase<Role>
    {
        public const string EntityLogicalName = "new_role";

        public const string StateCode = "statecode";
        public const string StatusCode = "statuscode";

        [SchemaName("statecode")]
        public enum State
        {
            Active = 0,
            Inactive = 1
        }

        [SchemaName("statuscode")]
        public enum Status
        {
            Open = 1,
            Closed = 100000004,
            Hired = 2,
            Canceled = 100000005
        }
    }
}
