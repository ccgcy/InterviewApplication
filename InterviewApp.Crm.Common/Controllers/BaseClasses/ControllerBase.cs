using InterviewApp.Crm.Common.Entities.BaseClasses;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using System;
using System.Linq;
using System.Reflection;

namespace InterviewApp.Crm.Common.Controllers.BaseClasses
{
    public abstract class ControllerBase
    {
        protected IOrganizationService Service { get; set; }
        protected internal abstract Entity Retrieve(Guid id, params string[] cols);
        protected internal abstract Entity Retrieve(EntityReference reference, params string[] cols);
        protected internal abstract void SetStatus(Guid entityId, int stateCode, int statusCode);
    }

    /// <summary>
    ///     This is the base class to all controllers.
    ///     provides basic functionality common to all controllers such as retrieve.
    ///     The default ColumnSet used is based on the attributes found in the given T.
    /// </summary>
    /// <typeparam name="T">The entity this controller deals with.</typeparam>
    public abstract class ControllerBase<T> : ControllerBase where T : EntityBase<T>
    {
        protected internal ControllerBase(IOrganizationService service)
        {
            Service = service;
        }

        /// <summary>
        ///     Ideally, this would be overridden in a derived class.
        /// </summary>
        protected virtual ColumnSet DefaultColumnSet =>
            new ColumnSet(GetDefaultReturnAttributes());

        protected internal override Entity Retrieve(Guid id, params string[] cols)
        {
            var columnSet = DefaultColumnSet;
            if (cols.Length > 0)
            {
                columnSet = new ColumnSet(cols);
            }

            return Service.Retrieve(GetSchemaName(), id, columnSet);
        }

        protected internal override Entity Retrieve(EntityReference reference, params string[] cols)
        {
            return Retrieve(reference.Id, cols);
        }

        /// <summary>
        ///     by default this returns all fields on given EntityBase (T). override this if you require more or less fields.
        /// </summary>
        /// <returns></returns>
        protected virtual string[] GetDefaultReturnAttributes()
        {
            //this is overkill, but the practice here is to use late binding, and i don't want to see anyone use new ColumnSet(true);
            //if you are worried about speed (due to reflection), override it in a child class.
            //this could be on an interface instead of being reflection in a base class, but i cannot see this being maintained
            //if manual maintenance is required.
            return typeof(T).GetFields(BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy)
                .Where(fi =>
                    fi.IsLiteral && !fi.IsInitOnly &&
                    !fi.GetCustomAttributes<SchemaNameAttribute>().Any(x => x.IgnoreDefault))
                .Select(x => x.GetRawConstantValue().ToString()).Where(x => x != GetSchemaName()).ToArray();
        }

        protected static string GetSchemaName()
        {
            return ((SchemaNameAttribute)typeof(T).GetCustomAttributes(typeof(SchemaNameAttribute), false)
                .FirstOrDefault())?.SchemaName;
        }

        protected internal Entity Create(Entity e)
        {
            e.Id = Service.Create(e);
            return e;
        }

        protected internal override void SetStatus(Guid entityId, int stateCode, int statusCode)
        {
            var entity = new Entity(GetSchemaName(), entityId);
            entity["statecode"] = new OptionSetValue(stateCode);
            entity["statuscode"] = new OptionSetValue(statusCode);
            Service.Update(entity);
        }
    }
}
