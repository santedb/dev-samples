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
using SanteDB.Core.Diagnostics;
using SanteDB.Core.Services;
using System;
using System.Threading;

namespace daemon_example
{
    /// <summary>
    /// The HelloDaemon is a simple Daemon that prints "HELLO WORLD" on the trace log
    /// </summary>
    public class HelloDaemon : IDaemonService
    {

        // Tracer
        private Tracer m_tracer = Tracer.GetTracer(typeof(HelloDaemon));

        // A simple timer 
        private Timer m_helloTimer;

        /// <summary>
        /// True when running
        /// </summary>
        public bool IsRunning => this.m_helloTimer != null;

        /// <summary>
        /// Service name
        /// </summary>
        public string ServiceName => "HelloDaemon";

        /// <summary>
        /// Fired when the daemon is starting
        /// </summary>
        /// <remarks>This event is sometimes used by other services to get notified when your service starts (see the chaining example)</remarks>
        public event EventHandler Starting;
        /// <summary>
        /// Fired after this daemon has started
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
        /// Start the hello daemon
        /// </summary>
        public bool Start()
        {
            // Notify other modules we're starting
            this.Starting?.Invoke(this, EventArgs.Empty);

            // Fire up the timer
            this.m_helloTimer = new Timer((_) => this.m_tracer.TraceInfo("Hello World!"), null, 1000, 1000);

            // Notify of start
            this.Started?.Invoke(this, EventArgs.Empty);
            return true;
        }

        /// <summary>
        /// Stop the hello daemon
        /// </summary>
        public bool Stop()
        {
            // Notify other modules we're stopping
            this.Stopping?.Invoke(this, EventArgs.Empty);

            // Dispose and get rid of the timer
            this.m_helloTimer?.Dispose();
            this.m_helloTimer = null;

            this.Stopped?.Invoke(this, EventArgs.Empty);
            return true;
        }
    }
}
