using Microsoft.Xrm.Sdk;
using System;

namespace InterviewApp.Plugin.Common
{
    public class PluginExecutionManager
    {
        public PluginExecutionManager(IPluginExecutionContext context, string entityName)
        {
            if (string.IsNullOrEmpty(entityName))
            {
                throw new InvalidPluginExecutionException("Plugin Execution Manager requires a valid entity name");
            }

            PluginExecutionContext =
                context ?? throw new InvalidPluginExecutionException(
                    "Plugin Execution Manager requires a valid IPluginExecutionContext");
            EntityName = entityName;
        }

        /// <summary>
        ///     Returns true if the execution context is for the Create Message
        /// </summary>
        public bool IsCreate => ExistsInContext(PluginMessage.Create);

        /// <summary>
        ///     Returns true if the execution context is for the Delete Message
        /// </summary>
        public bool IsDelete => ExistsInContext(PluginMessage.Delete);

        /// <summary>
        ///     Returns true if the execution context is for the SetStateDynamicEntity Message
        /// </summary>
        public bool IsSetStateDynamicEntity => ExistsInContext(PluginMessage.SetStateDynamicEntity);

        /// <summary>
        ///     Returns true if the execution context is for the SetState Message
        /// </summary>
        public bool IsSetState => ExistsInContext(PluginMessage.SetState);

        /// <summary>
        ///     Returns true if the execution context is for the AddUserToRecordTeam Message
        /// </summary>
        public bool IsAddUserToRecordTeam => ExistsInContext(PluginMessage.AddUserToRecordTeam);

        /// <summary>
        ///     Returns true if the execution context is for the AddUserToRecordTeam Message
        /// </summary>
        public bool IsRemoveUserFromRecordTeam => ExistsInContext(PluginMessage.RemoveUserFromRecordTeam);


        /// <summary>
        ///     Returns true if the execution context is for the Close Message
        /// </summary>
        public bool IsClose => ExistsInContext(PluginMessage.Close);

        public Guid GetId
        {
            get
            {
                var id = Guid.Empty;
                if (IsCreate || IsUpdate)
                {
                    id = GetInputParameter<Entity>(InputParameters.Target).Id;
                }
                else if (IsDelete)
                {
                    id = GetInputParameter<EntityReference>(InputParameters.Target).Id;
                }
                else if (IsSetStateDynamicEntity || IsSetState)
                {
                    if (PluginExecutionContext.InputParameters.ContainsKey(InputParameters.EntityMoniker))
                    {
                        id = GetInputParameter<EntityReference>(InputParameters.EntityMoniker).Id;
                    }
                    else if (PluginExecutionContext.InputParameters.ContainsKey(InputParameters.Target))
                    {
                        id = GetInputParameter<Entity>(InputParameters.Target).Id;
                    }
                }
                else if (IsClose)
                {
                    var incidentResolution = PluginExecutionContext.InputParameters.ContainsKey("IncidentResolution")
                        ? PluginExecutionContext.InputParameters["IncidentResolution"] as Entity
                        : null;

                    if (incidentResolution != null && incidentResolution.Contains("incidentid"))
                    {
                        if (incidentResolution.Attributes["incidentid"] is EntityReference entityReference)
                        {
                            return entityReference.Id;
                        }
                    }
                }

                return id;
            }
        }

        public int ContextDepth => PluginExecutionContext.Depth;

        public IPluginExecutionContext PluginExecutionContext { get; }

        public string EntityName { get; }

        public Guid UserId => PluginExecutionContext.UserId;


        public int GetState
        {
            get
            {
                if (!PluginExecutionContext.InputParameters.ContainsKey(InputParameters.State) ||
                    !(PluginExecutionContext.InputParameters[InputParameters.State] is OptionSetValue))
                {
                    throw new InvalidPluginExecutionException("Context does not contain State");
                }

                var optionSetValue = (OptionSetValue)PluginExecutionContext.InputParameters[InputParameters.State];
                return optionSetValue.Value;
            }
        }

        public int GetStatus
        {
            get
            {
                if (!PluginExecutionContext.InputParameters.ContainsKey(InputParameters.Status) ||
                    !(PluginExecutionContext.InputParameters[InputParameters.Status] is OptionSetValue))
                {
                    throw new InvalidPluginExecutionException("Context does not contain Status");
                }

                var optionSetValue = (OptionSetValue)PluginExecutionContext.InputParameters[InputParameters.Status];
                return optionSetValue.Value;
            }
        }

        public EntityReference GetRecord
        {
            get
            {
                if (!PluginExecutionContext.InputParameters.ContainsKey(InputParameters.Record))
                {
                    throw new InvalidPluginExecutionException("Context does not contain Record");
                }

                var record = (EntityReference)PluginExecutionContext.InputParameters[InputParameters.Record];
                return record;
            }
        }

        public Guid GetSystemUserId
        {
            get
            {
                if (!PluginExecutionContext.InputParameters.ContainsKey(InputParameters.SystemUserId))
                {
                    throw new InvalidPluginExecutionException("Context does not contain SystemUserId");
                }

                var systemUserId = (Guid)PluginExecutionContext.InputParameters[InputParameters.SystemUserId];
                return systemUserId;
            }
        }

        public Guid GetTeamTemplateId
        {
            get
            {
                if (!PluginExecutionContext.InputParameters.ContainsKey(InputParameters.TeamTemplateId))
                {
                    throw new InvalidPluginExecutionException("Context does not contain TeamTemplateId");
                }

                var teamTemplateId = (Guid)PluginExecutionContext.InputParameters[InputParameters.TeamTemplateId];
                return teamTemplateId;
            }
        }

        public bool IsUpdate => ExistsInContext(PluginMessage.Update);

        /// <summary>
        ///     Returns true if the execution context is for the Pre-Event Pre-validation (outside transaction) stage
        /// </summary>
        public bool IsPreEventPreValidation => PluginExecutionContext.Stage.Equals(10);

        /// <summary>
        ///     Returns true if the execution context is for the Pre-Event Pre-validation (inside transaction) stage
        /// </summary>
        public bool IsPreEventPreOperation => PluginExecutionContext.Stage.Equals(20);


        /// <summary>
        ///     Returns true if the execution context is for the Post-Event Post-Operation ( inside transaction) stage
        /// </summary>
        public bool IsPostEventPostOperation => PluginExecutionContext.Stage.Equals(40);

        private T GetInputParameter<T>(string parameterName) where T : new()
        {
            if (PluginExecutionContext.InputParameters.ContainsKey(parameterName) &&
                PluginExecutionContext.InputParameters[parameterName] is T)
            {
                return (T)PluginExecutionContext.InputParameters[parameterName];
            }

            return new T();
        }

        private bool ExistsInContext(PluginMessage pluginMessage)
        {
            var messageName = pluginMessage.ToString();
            return PluginExecutionContext.MessageName.Equals(messageName);
        }

        public TEntity GetPreImage<TEntity>(string entityLogicalName) where TEntity : Entity
        {
            if (!PluginExecutionContext.PreEntityImages.ContainsKey(entityLogicalName))
            {
                throw new InvalidPluginExecutionException("Context does not contain pre entity image " +
                                                          entityLogicalName);
            }

            return PluginExecutionContext.PreEntityImages[entityLogicalName].ToEntity<TEntity>();
        }

        public TEntity GetPostImage<TEntity>(string entityLogicalName) where TEntity : Entity
        {
            if (!PluginExecutionContext.PostEntityImages.ContainsKey(entityLogicalName))
            {
                throw new InvalidPluginExecutionException("Context does not contain post entity image " +
                                                          entityLogicalName);
            }

            return PluginExecutionContext.PostEntityImages[entityLogicalName].ToEntity<TEntity>();
        }

        public TEntity GetTargetEntity<TEntity>() where TEntity : Entity
        {
            return ((Entity)PluginExecutionContext.InputParameters[InputParameters.Target]).ToEntity<TEntity>();
        }

        public bool EntityIs(string entityLogicalName)
        {
            return EntityName == entityLogicalName;
        }

        public bool ContainsPostImage(string entityLogicalName)
        {
            return PluginExecutionContext.PostEntityImages.ContainsKey(entityLogicalName);
        }

        public void ValidateExecution()
        {
            if (IsCreate || IsUpdate)
            {
                ValidatePluginExecution();
            }
            else if (IsDelete)
            {
                ValidateDeletePluginExecution();
            }
            else if (IsSetStateDynamicEntity)
            {
                ValidateSetStateDynamicEntityPluginExecution();
            }
        }

        private void ValidatePluginExecution()
        {
            if (PluginExecutionContext.InputParameters.Contains(InputParameters.Target) &&
                PluginExecutionContext.InputParameters[InputParameters.Target] is Entity)
            {
                var entity = (Entity)PluginExecutionContext.InputParameters[InputParameters.Target];

                if (entity.LogicalName == EntityName)
                {
                    return;
                }

                var errorMessage =
                    $"The entity execution did not match. Context Entity was: {entity.LogicalName}, but the plugin was designed for: {EntityName}.";
                throw new InvalidOperationException(errorMessage);
            }

            throw new InvalidOperationException("Input Parameters did not contain a Target and was not an Entity type");
        }

        private void ValidateDeletePluginExecution()
        {
            if (PluginExecutionContext.InputParameters.Contains(InputParameters.Target) &&
                PluginExecutionContext.InputParameters[InputParameters.Target] is EntityReference)
            {
                var entity = (EntityReference)PluginExecutionContext.InputParameters[InputParameters.Target];

                if (entity.LogicalName == EntityName)
                {
                    return;
                }

                var errorMessage =
                    $"The entity execution did not match. Context Entity was: {entity.LogicalName}, but the plugin was designed for: {EntityName}.";
                throw new InvalidOperationException(errorMessage);
            }

            throw new InvalidOperationException(
                "Input Parameters did not contain a Target and was not an EntityReference type");
        }

        private void ValidateSetStateDynamicEntityPluginExecution()
        {
            if (PluginExecutionContext.InputParameters.Contains(InputParameters.EntityMoniker) &&
                PluginExecutionContext.InputParameters[InputParameters.EntityMoniker] is EntityReference)
            {
                var entity = (EntityReference)PluginExecutionContext.InputParameters[InputParameters.EntityMoniker];

                if (entity.LogicalName == EntityName)
                {
                    return;
                }

                var errorMessage =
                    $"The entity execution did not match. Context Entity was: {entity.LogicalName}, but the plugin was designed for: {EntityName}.";
                throw new InvalidOperationException(errorMessage);
            }

            if (PluginExecutionContext.InputParameters.Contains(InputParameters.Target) &&
                PluginExecutionContext.InputParameters[InputParameters.Target] is Entity)
            {
                var entity = (Entity)PluginExecutionContext.InputParameters[InputParameters.Target];

                if (entity.LogicalName == EntityName)
                {
                    return;
                }

                var errorMessage =
                    $"The entity execution did not match. Context Entity was: {entity.LogicalName}, but the plugin was designed for: {EntityName}.";
                throw new InvalidOperationException(errorMessage);
            }

            var names = string.Empty;
            foreach (var key in PluginExecutionContext.InputParameters.Keys)
            {
                names += "'" + key + "-" + PluginExecutionContext.InputParameters[key].GetType() + "'; ";
            }

            var message =
                $"Plugin execution context must contain an EntityReference. ContextParameterCount:{PluginExecutionContext.InputParameters.Count}. Names:{names}";
            throw new InvalidOperationException(message);
        }
    }

    public class InputParameters
    {
        public static string Target = "Target";
        public static string EntityMoniker = "EntityMoniker";
        public static string IncidentResolution = "IncidentResolution";
        public static string State = "State";
        public static string Status = "Status";
        public static string Record = "Record";
        public static string SystemUserId = "SystemUserId";
        public static string TeamTemplateId = "TeamTemplateId";
    }

    public enum PluginMessage
    {
        Create,
        Update,
        Delete,
        SetState,
        SetStateDynamicEntity,
        Close,
        AddUserToRecordTeam,
        RemoveUserFromRecordTeam
    }
}
