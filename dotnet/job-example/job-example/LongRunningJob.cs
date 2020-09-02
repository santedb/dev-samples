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
using SanteDB.Core.Jobs;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace job_example
{
    /// <summary>
    /// A Long Running Job
    /// </summary>
    /// <remarks>
    /// This job illustrates creating a cancellable job that runs for a very long time
    /// </remarks>
    public class LongRunningJob : IJob
    {

        // Tracer
        private Tracer m_tracer = Tracer.GetTracer(typeof(LongRunningJob));

        // Cancel flag
        private bool m_cancelFlag = false;

        /// <summary>
        /// Get the name of the job
        /// </summary>
        public string Name => "A really long running job";

        /// <summary>
        /// Can cancel the job
        /// </summary>
        public bool CanCancel => true;

        /// <summary>
        /// Gets the current state
        /// </summary>
        public JobStateType CurrentState { get; private set; }

        /// <summary>
        /// Get the parameters
        /// </summary>
        public IDictionary<string, Type> Parameters => null;

        /// <summary>
        /// Time the job last started
        /// </summary>
        public DateTime? LastStarted { get; private set; }

        /// <summary>
        /// Time the job last finished
        /// </summary>
        public DateTime? LastFinished { get; private set; }

        /// <summary>
        /// Cancel this job
        /// </summary>
        public void Cancel()
        {
            this.m_cancelFlag = true;
        }

        /// <summary>
        /// Run the long running job
        /// </summary>
        /// <param name="sender">The job manager that send the request to start the job</param>
        /// <param name="e">The event arguments sent by the job manager</param>
        /// <param name="parameters">The parameters to the job</param>
        public void Run(object sender, EventArgs e, object[] parameters)
        {
            try
            {
                this.m_cancelFlag = false;
                this.LastStarted = DateTime.Now;
                this.CurrentState = JobStateType.Running;

                while(true)
                {
                    Thread.Sleep(1000);
                    this.m_tracer.TraceInfo("I just wasted another second... ");
                    if (this.m_cancelFlag)
                    {
                        this.CurrentState = JobStateType.Cancelled;
                        break;
                    }
                }

                this.LastFinished = DateTime.Now;

            }
            catch (Exception ex)
            {
                // Error handling
                this.m_tracer.TraceError("Error sending message - {0}", ex);
                this.CurrentState = JobStateType.Aborted;
                throw new Exception("Error running my Hello World Job!", ex);
            }
            finally
            {
                this.LastFinished = DateTime.Now;
            }
        }
    }
}
