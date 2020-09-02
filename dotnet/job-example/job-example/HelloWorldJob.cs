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
using SanteDB.Core.Jobs;
using SanteDB.Core.Mail;
using SanteDB.Core.Services;
using System;
using System.Collections.Generic;

namespace job_example
{
    /// <summary>
    /// This job prints "Hello World" to the messaging service
    /// </summary>
    /// <remarks>
    /// <para>This example is outlined on our wiki! This example illustrates:</para>
    /// <list type="number">
    ///     <item>How to implement an IJob</item>
    ///     <item>Calling the message service to send a notice to users</item>
    ///     <item>Indicating that the job was successful</item>
    /// </list>
    /// </remarks>
    public class HelloWorldJob : IJob
    {

        // Tracer for error logging
        private Tracer m_tracer = Tracer.GetTracer(typeof(HelloWorldJob));

        /// <summary>
        /// Get the name of the job
        /// </summary>
        public string Name => "Hello World Job!";

        /// <summary>
        /// Return true if the job can be cancelled while running
        /// </summary>
        public bool CanCancel => false;

        /// <summary>
        /// Use this property to return the current state of your job
        /// </summary>
        public JobStateType CurrentState { get; private set; }

        /// <summary>
        /// Get the type of parameters that this job supports. Here the job accepts a single string parameter called your_name
        /// </summary>
        public IDictionary<string, Type> Parameters => new Dictionary<String, Type>()
        {
            { "your_name", typeof(String) }
        };

        /// <summary>
        /// Get the date that the job was last started
        /// </summary>
        public DateTime? LastStarted { get; private set; }

        /// <summary>
        /// Get the date that the job was last completed
        /// </summary>
        public DateTime? LastFinished { get; private set; }

        /// <summary>
        /// Cancel the job
        /// </summary>
        /// <exception cref="NotSupportedException">When the cancel operation is not supported</exception>
        public void Cancel()
        {
            throw new NotSupportedException();
        }

        /// <summary>
        /// Run the specified job
        /// </summary>
        /// <param name="sender">The IJobManager that is initiating this request</param>
        /// <param name="e">Event arguments the IJobManager sent to the event</param>
        /// <param name="parameters">The parameters provided by the user (in order of the <see cref="Parameters"/> property</param>
        public void Run(object sender, EventArgs e, object[] parameters)
        {
            if (parameters.Length == 0 || !(parameters[0] is String yourName)) 
                yourName = "Mystery User";

            try
            {
                // Set the status
                this.CurrentState = JobStateType.Running;
                this.LastStarted = DateTime.Now;

                // The message repository is used to store / query for messages to / from users
                var messageService = ApplicationServiceContext.Current.GetService<IMailMessageRepositoryService>();

                // Create the message & broadcast it
                var message = new MailMessage("student", yourName, "Hello World!", $"Hello {yourName}! This is a message from my job!", MailMessageFlags.None);
                messageService.Broadcast(message);

                // Set the state
                this.CurrentState = JobStateType.Completed;
            }
            catch (Exception ex)
            {
                // Error handling
                this.m_tracer.TraceError("Error sending message - {0}", e);
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
