// -----------------------------------------------------------------------
// <copyright file="FormattedAppInsightsTraceListener.cs" company="Microsoft">
// ==============================================================================
// Copyright © Microsoft Corporation.  All rights reserved. 2014
// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY
// OF ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT
// LIMITED TO THE IMPLIED WARRANTIES OF MERCHANTABILITY AND
// FITNESS FOR A PARTICULAR PURPOSE.
// ==============================================================================
// </copyright>
// -----------------------------------------------------------------------

using System;
using Microsoft.ApplicationInsights.EntlibTraceListener.Configuration;
using Microsoft.ApplicationInsights.Tracing;
using Microsoft.ApplicationInsights.Tracing.Serialization;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using Microsoft.Practices.EnterpriseLibrary.Common.Configuration;
using Microsoft.Practices.EnterpriseLibrary.Logging;
using Microsoft.Practices.EnterpriseLibrary.Logging.Formatters;
using Microsoft.Practices.EnterpriseLibrary.Logging.TraceListeners;
using Tracer = Microsoft.ApplicationInsights.Tracing.Tracer;

namespace Microsoft.ApplicationInsights.EntlibTraceListener
{
    /// <summary>
    /// AppInsightsTraceListener that routes all logging output to the Application Insights logging framework.
    /// The messages will be read by the Microsoft Monitoring Agent (MMA) if available or directly sends logging message to the Application Insights cloud service.
    /// </summary>
    [ConfigurationElementType(typeof(FormattedAppInsightsTraceListenerData))]
    [SuppressMessage("Microsoft.Design", "CA1001:TypesThatOwnDisposableFieldsShouldBeDisposable", Justification = "Releasing the resources on the close method")]
    public sealed class FormattedAppInsightsTraceListener : FormattedTraceListenerBase
    {
        /// <summary>
        /// The logging controller we will be using.
        /// </summary>
        private Tracer tracer;

        private string instrumentationKey;
        #region Constructors

        /// <summary>
        /// Initializes a new instance of <see cref="FormattedAppInsightsTraceListener"/>.
        /// </summary>        
        public FormattedAppInsightsTraceListener()
            : this(string.Empty)
        {            
        }

        /// <summary>
        /// Initializes a new instance of <see cref="FormattedAppInsightsTraceListener"/>, without specifying a
        /// instrumentation Key.
        /// </summary>
        /// <param name="instrumentationKey">InstrumentationKey given by app insights to log the data against.</param>        
        public FormattedAppInsightsTraceListener(string instrumentationKey)
            : this(instrumentationKey, null)
        {
        }

        /// <summary>
        /// Initializes a new instance of <see cref="FormattedAppInsightsTraceListener"/>.
        /// </summary>        
        /// <param name="formatter">The formatter.</param>        
        public FormattedAppInsightsTraceListener(
            ILogFormatter formatter)
            : this(string.Empty, formatter)
        {            
        }

        /// <summary>
        /// Initializes a new instance of <see cref="FormattedAppInsightsTraceListener"/>.
        /// </summary>
        /// <param name="instrumentationKey">InstrumentationKey given by app insights to log the data against.</param>
        /// <param name="formatter">The formatter.</param>        
        public FormattedAppInsightsTraceListener(
            string instrumentationKey,            
            ILogFormatter formatter)
            : base(formatter)
        {
            this.InitializeLogger();
            this.InstrumentationKey = instrumentationKey;            
        }
        #endregion Constructors
        
        #region properties
           /// <summary>
        /// The Application Insights instrumentationKey for your application. 
        /// </summary>
        public string InstrumentationKey 
        {
               get
               {
                   return this.instrumentationKey;
               }

               set
               {
                   this.instrumentationKey = value;
                   this.tracer.InstrumentationKey = value;
               } 
        }

        /// <summary>
        /// The logging controller we will be using.
        /// </summary>
        internal Tracer Tracer
        {
            get
            {
                return this.tracer;
            }
        }
        
        #endregion

        #region implementation of FormattedTraceListenerBase
        /// <summary>
        /// The Write method. 
        /// </summary>
        /// <param name="message">The message to log.</param>
        public override void Write(string message)
        {
            if (this.Filter != null && !this.Filter.ShouldTrace(null, string.Empty, TraceEventType.Verbose, 0, message, null, null, null))
            {
                return;
            }

            this.tracer.LogMessage(message);
        }

        /// <summary>
        /// The WriteLine method.
        /// </summary>
        /// <param name="message">The message to log.</param>
        public override void WriteLine(string message)
        {
            this.Write(message + Environment.NewLine);
        }

        #endregion implementation of FormattedTraceListenerBase

        #region extending the listener
        /// <summary>
        /// Delivers the trace data as an email message.
        /// </summary>
        /// <param name="eventCache">The context information provided by <see cref="System.Diagnostics"/>.</param>
        /// <param name="source">The name of the trace source that delivered the trace data.</param>
        /// <param name="eventType">The type of event.</param>
        /// <param name="id">The id of the event.</param>
        /// <param name="data">The data to trace.</param>
        public override void TraceData(TraceEventCache eventCache, string source, TraceEventType eventType, int id, object data)
        {
            if ((this.Filter != null) && !this.Filter.ShouldTrace(eventCache, source, eventType, id, null, null, data, null))
            {
                return;
            }

            var formattedMessage = string.Empty;
            ApplicationInsightsTraceData metaData = null;
            object[] additionalProperties = null;
            var log = data as LogEntry;
            if (log != null)
            {
                formattedMessage = this.Formatter != null ? this.Formatter.Format(log) : log.Message;
                additionalProperties = this.FormatLogEntryForAppInsights(log); 
            }                        
            else if (data is string)
            {
                metaData = new ApplicationInsightsTraceData { TraceEventType = TraceEventType.Verbose };
                this.tracer.LogMessageWithData((string)data, metaData);
                return;
            }

            metaData = this.CreateTraceData(eventCache, eventType, id);

            metaData.AdditionalData = additionalProperties; 
            string message = string.IsNullOrEmpty(formattedMessage) ? (data == null ? string.Empty : data.ToString()) : formattedMessage;
            this.tracer.LogMessageWithData(message, metaData);
        }

        /// <summary>
        /// Writes trace information, a message, and event information to the listener specific output.
        /// </summary>
        /// <param name="eventCache">A TraceEventCache object that contains the current process ID, thread ID, and stack trace information.</param>
        /// <param name="source">A name used to identify the output, typically the name of the application that generated the trace event.</param>
        /// <param name="eventType">One of the TraceEventType values specifying the type of event that has caused the trace.</param>
        /// <param name="id">A numeric identifier for the event.</param>
        /// <param name="message">A message to write.</param>
        public override void TraceEvent(TraceEventCache eventCache, string source, TraceEventType eventType, int id, string message)
        {
               this.TraceData(eventCache, source, eventType, id, message);                        
        }

        /// <summary>
        /// Writes trace information, a message, and event information to the listener specific output.
        /// </summary>
        /// <param name="eventCache">A TraceEventCache object that contains the current process ID, thread ID, and stack trace information.</param>
        /// <param name="source">A name used to identify the output, typically the name of the application that generated the trace event.</param>
        /// <param name="eventType">One of the TraceEventType values specifying the type of event that has caused the trace.</param>
        /// <param name="id">A numeric identifier for the event.</param>        
        public override void TraceEvent(TraceEventCache eventCache, string source, TraceEventType eventType, int id)
        {
            this.TraceEvent(eventCache, source, eventType, id, string.Empty);            
        }

        #endregion extending the listener

        
        #region private methods
        
        private void InitializeLogger()
        {
            this.tracer = new Tracer(this.InstrumentationKey);
        }

        private object[] FormatLogEntryForAppInsights(LogEntry logEntry)
        {
            return
                 new object[]
                {
                    new { Message = logEntry.Message },

                    new { ActivityId = logEntry.ActivityId },
                     
                    new { Categories = logEntry.Categories },

                     new { LoggedSeverity = logEntry.LoggedSeverity },

                     new { Priority = logEntry.Priority },

                     new { Severity = logEntry.Severity },

                     new { ProcessId = logEntry.ProcessId },

                     new { ProcessName = logEntry.ProcessName },

                     new { RelatedActivityId = logEntry.RelatedActivityId },

                     new { Title = logEntry.Title },
                      
                     new { ErrorMessages = logEntry.ErrorMessages },

                     new { AppDomainName = logEntry.AppDomainName },

                     new { MachineName = logEntry.MachineName },

                     new { ThreadId = logEntry.Win32ThreadId },
                      
                     new { ManagedThreadName = logEntry.ManagedThreadName },
                      
                     new { ExtendedProperties = logEntry.ExtendedProperties },                     
                };
        }

        /// <summary>
        /// Creates the trace data.
        /// </summary>
        /// <param name="eventCache">The event cache.</param>
        /// <param name="eventType">Type of the event.</param>
        /// <param name="id">The identifier.</param>
        /// <returns>The trace meta data.</returns>
        private ApplicationInsightsTraceData CreateTraceData(TraceEventCache eventCache, TraceEventType eventType, int id)
        {
            ApplicationInsightsTraceData metaData = new ApplicationInsightsTraceData
            {
                TraceEventType = eventType,
                EventId = id,
            };

            if ((this.TraceOutputOptions & TraceOptions.LogicalOperationStack) == TraceOptions.LogicalOperationStack)
            {
                metaData.LogicalOperationStack = eventCache.LogicalOperationStack;
            }

            if ((this.TraceOutputOptions & TraceOptions.Timestamp) == TraceOptions.Timestamp)
            {
                metaData.Timestamp = eventCache.Timestamp;
            }

            if ((this.TraceOutputOptions & TraceOptions.Callstack) == TraceOptions.Callstack)
            {
                metaData.Callstack = eventCache.Callstack;
            }

            return metaData;
        }

        #endregion private methods
    }
}
