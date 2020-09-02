/*
 * Based on OpenIZ - Based on OpenIZ, Copyright (C) 2015 - 2019 Mohawk College of Applied Arts and Technology
 * Portions Copyright 2019-2020, Fyfe Software Inc. and the SanteSuite Contributors (See NOTICE)
 * 
 * Licensed under the Apache License, Version 2.0 (the "License"); you 
 * may not use this file except in compliance with the License. You may 
 * obtain a copy of the License at 
 * 
 * http://www.apache.org/licenses/LICENSE-2.0 
 * 
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS, WITHOUT
 * WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied. See the 
 * License for the specific language governing permissions and limitations under 
 * the License.
 * 
 * User: fyfej (<Unknown>)
 * Date: 2020-9-2
 */
using SanteDB.Core;
using SanteDB.Core.Jobs;
using SanteDB.Core.Services;
using System;
using System.Collections.Generic;
using System.Text;

namespace job_example
{
    /// <summary>
    /// A daemon service which registers the jobs in this project
    /// </summary>
    public class RegisterJobDaemon : IDaemonService
    {
        /// <summary>
        /// True if the daemon service is running
        /// </summary>
        public bool IsRunning => false;

        /// <summary>
        /// Gets the name of the service
        /// </summary>
        public string ServiceName => "Same JOB Registration Daemon";

        /// <summary>
        /// Fired when the daemon is starting
        /// </summary>
        public event EventHandler Starting;
        /// <summary>
        /// Fired when the daemon has started
        /// </summary>
        public event EventHandler Started;
        /// <summary>
        /// Fired when the daemon is stopping
        /// </summary>
        public event EventHandler Stopping;
        /// <summary>
        /// Fired when the daemon has stopped
        /// </summary>
        public event EventHandler Stopped;
        
        /// <summary>
        /// Start the service and register the jobs
        /// </summary>
        public bool Start()
        {
            this.Starting?.Invoke(this, EventArgs.Empty);

            // Get the job manager
            var jobManager = ApplicationServiceContext.Current.GetService<IJobManagerService>();

            // Register the jobs
            jobManager.AddJob(new HelloWorldJob(), TimeSpan.MaxValue, JobStartType.Never);
            jobManager.AddJob(new HelloWorldJob(), TimeSpan.MaxValue, JobStartType.Never);
            this.Started?.Invoke(this, EventArgs.Empty);
            return true;
        }

        /// <summary>
        /// Stop the service 
        /// </summary>
        public bool Stop()
        {
            this.Stopping?.Invoke(this, EventArgs.Empty);

            this.Stopped?.Invoke(this, EventArgs.Empty);
            return true;
        }
    }
}
