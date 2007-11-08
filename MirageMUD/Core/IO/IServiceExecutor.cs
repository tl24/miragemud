using System;
using System.Collections.Generic;
using System.Text;

namespace Mirage.Core.IO
{
    /// <summary>
    /// A service method
    /// </summary>
    public delegate void ServiceMethod();

    /// <summary>
    /// A service that runs in the MirageServer execution loop.  A service can have multiple
    /// execution points.  It does this by registering different method keys in the config file.
    /// The MirageServer will then request the ServiceMethod to execute by calling GetServiceMethod.
    /// The service will first be started before any methods or called.  The service should set the
    /// IsStarted flag otherwise it may be restarted repeatedly.
    /// </summary>
    public interface IServiceExecutor
    {
        /// <summary>
        /// Returns the servicemethod associated with a key.  This is so that the service
        /// may called multiple times at different points in the execution loop.
        /// </summary>
        /// <param name="key">key indicating the method to call, does not have to be a method name</param>
        /// <returns>ServiceMethod delegate</returns>
        ServiceMethod GetServiceMethod(string key);

        /// <summary>
        /// Will be called once before any method calls to start the service if necessary.  The service
        /// should set the IsStarted property to true.
        /// </summary>
        void Start();

        /// <summary>
        /// Stops the service on shutdown.
        /// </summary>
        void Stop();

        /// <summary>
        /// Test to see if the service has been started
        /// </summary>
        bool IsStarted { get; }
    }

    /// <summary>
    /// An entry for a service and its method key
    /// </summary>
    public class ServiceEntry
    {
        private IServiceExecutor _service;
        private string _methodKey;
        private ServiceMethod _method;

        public ServiceEntry(IServiceExecutor service, string methodKey)
        {
            _service = service;
            _methodKey = methodKey;
        }

        public IServiceExecutor Service
        {
            get { return this._service; }
            set { this._service = value; }
        }

        public string MethodKey
        {
            get { return this._methodKey; }
            set { this._methodKey = value; }
        }

        public void Execute()
        {
            if (_method == null)
                _method = Service.GetServiceMethod(MethodKey);
            _method();
        }

        public override string ToString()
        {
            return "ServiceEntry(" + Service.GetType().Name + ":" + MethodKey + ")";
        }
    }
}
