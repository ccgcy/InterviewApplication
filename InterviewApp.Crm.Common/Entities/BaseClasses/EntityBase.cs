using Microsoft.Xrm.Sdk;
using System.Linq;

namespace InterviewApp.Crm.Common.Entities.BaseClasses
{
    /// <summary>
    ///     This class and its derivatives exist purely to provide a list of strings to use as attributes.
    ///     Typically early binding would be used in its place for ease of maintenance, but it is standard here to use late
    ///     binding most likely due to the number of people that work on any project at any given time.
    ///     if you don't want to unit test the availability of attributes and entities, you can safely remove this class,
    ///     remove this as the base class of all derived classes and delete the associated unit tests.
    /// </summary>
    /// <typeparam name="T">This needs to be the type that is implementing this abstract class.</typeparam>
    public abstract class EntityBase<T>
    {
        public static string GetSchemaName()
        {
            return ((SchemaNameAttribute)typeof(T).GetCustomAttributes(typeof(SchemaNameAttribute), false)
                .FirstOrDefault())?.SchemaName;
        }

        public static Entity Create()
        {
            return new Entity(GetSchemaName());
        }
    }
}
