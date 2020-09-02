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
using SanteDB.Core.Diagnostics;
using SanteDB.Core.Services;
using System;
using System.Collections.Generic;
using System.Text;

namespace daemon_example
{
    /// <summary>
    /// Chained Daemon Service
    /// </summary>
    /// <remarks>This example illustrates how to have your daemon service wait until another daemon is started or the application service</remarks>
    [ServiceProvider("Chained Daemon", Dependencies = new Type[]{ typeof(HelloDaemon) })]
    public class ChainedDaemon : IDaemonService
    {

        // Tracer
        private Tracer m_tracer = Tracer.GetTracer(typeof(ChainedDaemon));

        /// <summary>
        /// True if the servic is running
        /// </summary>
        public bool IsRunning { get; private set; }

        /// <summary>
        /// Gets the service name
        /// </summary>
        public string ServiceName => "Chained Daemon";

        /// <summary>
        /// Fired when the daemon is starting
        /// </summary>
        public event EventHandler Starting;
        /// <summary>
        /// Fired when the service has started
        /// </summary>
        public event EventHandler Started;
        /// <summary>
        /// Fired when the service is stopping
        /// </summary>
        public event EventHandler Stopping;
        /// <summary>
        /// Fired when the service has stopped
        /// </summary>
        public event EventHandler Stopped;

        /// <summary>
        /// Internal startup 
        /// </summary>
        public void StartInternal()
        {
            this.m_tracer.TraceInfo("The HelloWorld daemon has finished starting!");

            this.Starting?.Invoke(this, EventArgs.Empty);

            // DO startup stuff here

            this.Started?.Invoke(this, EventArgs.Empty);
        }

        /// <summary>
        /// Internal stop
        /// </summary>
        public void StopInternal()
        {
            this.m_tracer.TraceInfo("The HelloWorld daemon has finished shutting down!");

            this.Stopping?.Invoke(this, EventArgs.Empty);

            // DO shutdown stuff here

            this.Stopped?.Invoke(this, EventArgs.Empty);
        }

        /// <summary>
        /// Start the daemon
        /// </summary>
        public bool Start()
        {
            // We don't want to start until the dependent service has started completely
            var helloService = ApplicationServiceContext.Current.GetService<HelloDaemon>();

            // Is the hello service already running?
            if (helloService == null)
            {
                this.m_tracer.TraceInfo("I don't want to start unless HelloDaemon is configured");
                return false;
            }
            else if (helloService.IsRunning) // already running
                this.StartInternal();
            else 
                helloService.Started += (o, e) => this.StartInternal();

            return true;
        }

        /// <summary>
        /// Stop the daemon
        /// </summary>
        public bool Stop()
        {
            // We don't want to start until the dependent service has started completely
            var helloService = ApplicationServiceContext.Current.GetService<HelloDaemon>();
            if (helloService == null)
            {
                this.m_tracer.TraceError("Hmmm, I shouldn't have been started!");
                return false;
            }
            else if (helloService.IsRunning) // Wait until the hello world daemon stops
                helloService.Stopped += (o, e) => this.StopInternal();
            else // Hello world has stopped already
                this.StopInternal();

            return true;
        }
    }
}
