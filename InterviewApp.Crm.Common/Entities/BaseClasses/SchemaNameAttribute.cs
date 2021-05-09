using System;

namespace InterviewApp.Crm.Common.Entities.BaseClasses
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Enum | AttributeTargets.Field)]
    public class SchemaNameAttribute : Attribute
    {
        public SchemaNameAttribute(string schemaName, string entity = null, bool ignoreTests = false,
            bool ignoreDefault = false)
        {
            SchemaName = schemaName;
            Entity = entity;
            IgnoreTests = ignoreTests;
            IgnoreDefault = ignoreDefault;
        }

        public string SchemaName { get; set; }

        public string Entity { get; set; }

        public bool IgnoreTests { get; set; }

        public bool IgnoreDefault { get; set; }
    }
}
