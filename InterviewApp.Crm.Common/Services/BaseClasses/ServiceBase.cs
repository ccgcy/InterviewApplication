using InterviewApp.Crm.Common.Controllers.BaseClasses;
using Microsoft.Xrm.Sdk;
using System;
using System.Reflection;

namespace InterviewApp.Crm.Common.Services.BaseClasses
{
    /// <summary>
    ///     This is the base class to all Services. Provides lazy loading access to all other necessary services.
    ///     and creates the necessary controller.
    /// </summary>
    /// <typeparam name="T">The type of the controller to use</typeparam>
    public class ServiceBase<T> where T : ControllerBase
    {
        private readonly IOrganizationService _service;
        private T _controller;
        private CandidateService _lazyCandidateService;

        protected ServiceBase(IOrganizationService service)
        {
            _service = service;
        }

        protected T Controller
        {
            get
            {
                if (_controller == null)
                {
                    _controller = (T)typeof(T).GetConstructors(BindingFlags.Instance | BindingFlags.NonPublic)[0]
                        .Invoke(new object[] { _service });
                }

                return _controller;
            }
        }

        protected CandidateService CandidateService =>
            _lazyCandidateService ?? (_lazyCandidateService = new CandidateService(_service));

        public Entity Retrieve(EntityReference target, params string[] columns)
        {
            return Controller.Retrieve(target.Id, columns);
        }

        public Entity Retrieve(Guid id, params string[] columns)
        {
            return Controller.Retrieve(id, columns);
        }
    }
}
