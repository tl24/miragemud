using System;
using System.Collections.Generic;
using System.Text;

namespace Mirage.Core
{
    /// <summary>
    /// Base class for services
    /// </summary>
    public abstract class ServiceExecutorBase : IServiceExecutor
    {
        private bool _isStarted = false;

        /// <summary>
        /// Child classes should override this method for ServiceMethod lookup
        /// </summary>
        /// <param name="key">the key for the service method</param>
        /// <returns>ServiceMethod delegate</returns>
        public abstract ServiceMethod GetServiceMethod(string key);

        /// <summary>
        /// Starts the service
        /// </summary>
        public void Start()
        {
            OnStart();
            _isStarted = true;
        }

        /// <summary>
        /// Child classes should override this if they need to do something on startup
        /// </summary>
        protected virtual void OnStart() {
        }

        public void Stop()
        {
            OnStop();
            _isStarted = false;
        }

        /// <summary>
        /// Child classes should override this if they need to do something on stopping
        /// </summary>
        protected virtual void OnStop()
        {            
        }

        /// <summary>
        /// Has the service been started
        /// </summary>
        public bool IsStarted
        {
            get { return _isStarted; }
        }

    }
}
