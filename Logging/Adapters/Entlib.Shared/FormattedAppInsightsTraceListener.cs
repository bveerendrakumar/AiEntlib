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
using Microsoft.ApplicationInsights.DataContracts;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using Microsoft.Practices.EnterpriseLibrary.Common.Configuration;
using Microsoft.Practices.EnterpriseLibrary.Logging;
using Microsoft.Practices.EnterpriseLibrary.Logging.Formatters;
using Microsoft.Practices.EnterpriseLibrary.Logging.TraceListeners;
using System.Collections.Generic;
using Newtonsoft.Json;

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
        private TelemetryClient _telemetryClient;

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
               } 
        }

        /// <summary>
        /// The logging controller we will be using.
        /// </summary>
        internal TelemetryClient TelemetryClient
        {
            get { return this._telemetryClient; }
            set { this._telemetryClient = value; }
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
            var trace = new TraceTelemetry(message);
            this.CreateTraceData(null, trace, TraceEventType.Verbose, new TraceEventCache());
            this.TelemetryClient.Track(trace);
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
            var log = data as LogEntry;
            TraceTelemetry trace = null;
            if (log != null)
            {
                formattedMessage = this.Formatter != null ? this.Formatter.Format(log) : log.Message;                
                trace = new TraceTelemetry(formattedMessage);
                this.CreateTraceData(log, trace, eventType, eventCache);
            }                        
            else if (data is string)
            {
                trace = new TraceTelemetry((string)data);
                this.CreateTraceData(null, trace, TraceEventType.Verbose, eventCache);                
            }

            this.TelemetryClient.Track(trace);
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
            this._telemetryClient = new TelemetryClient();
            if (!string.IsNullOrEmpty(this.InstrumentationKey))
            {
                this.TelemetryClient.Context.InstrumentationKey = this.InstrumentationKey;
            }
        }

        private void CreateTraceData(LogEntry logEntry, TraceTelemetry trace, TraceEventType eventType, TraceEventCache eventCache)
        {
            IDictionary<string, string> propertyBag = trace.Context.Properties;
            if (null != logEntry)
            {
                propertyBag.Add("SourceType", string.Format("{0}: {1}","SourceType", logEntry.GetType().ToString()));
                propertyBag.Add("LoggedSeverity", string.Format("{0}: {1}", "LoggedSeverity", logEntry.LoggedSeverity));
         if (null != logEntry.ActivityId)
                {
                    propertyBag.Add("ActivityId", string.Format("{0}: {1}", "ActivityId", logEntry.ActivityId.ToString()));
                }
                if (null != logEntry.CategoriesStrings)
                {
                    propertyBag.Add("Categories", string.Format("{0}: {1}", "Categories", string.Join(",", logEntry.CategoriesStrings)));
                }

                propertyBag.Add("Priority", string.Format("{0}: {1}", "Priority", logEntry.Priority.ToString()));
                propertyBag.Add("ProcessId", string.Format("{0}: {1}", "ProcessId", logEntry.ProcessId));
                propertyBag.Add("ProcessName", string.Format("{0}: {1}", "ProcessName", logEntry.ProcessName));
                if (null != logEntry.RelatedActivityId)
                {
                    propertyBag.Add("RelatedActivityId", string.Format("{0}: {1}", "RelatedActivityId", logEntry.RelatedActivityId.ToString()));
                }
                propertyBag.Add("Title", string.Format("{0}: {1}", "Title", logEntry.Title));
                propertyBag.Add("ErrorMessages", string.Format("{0}: {1}", "ErrorMessages", logEntry.ErrorMessages));
                propertyBag.Add("AppDomainName", string.Format("{0}: {1}", "AppDomainName", logEntry.AppDomainName));
                propertyBag.Add("ManagedThreadName", string.Format("{0}: {1}", "ManagedThreadName", logEntry.ManagedThreadName));
                propertyBag.Add("MachineName", string.Format("{0}: {1}", "MachineName", logEntry.MachineName));
                if (null != logEntry.ExtendedProperties)
                {
                    foreach (var extendedProperty in logEntry.ExtendedProperties)
                    {
                        propertyBag.Add(string.Format("ExtendedProperties.{0}", extendedProperty.Key), JsonConvert.SerializeObject(extendedProperty.Value));
                    }
                }
            }
            if ((this.TraceOutputOptions & TraceOptions.Timestamp) == TraceOptions.Timestamp)
            {
                propertyBag.Add("Timestamp", string.Format("{0}: {1}", "Timestamp", eventCache.Timestamp.ToString()));
            }

     }

        
        #endregion private methods
    }
}
