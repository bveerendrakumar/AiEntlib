// -----------------------------------------------------------------------
// <copyright file="Tracer.cs" company="Microsoft">
// ==============================================================================
// Copyright © Microsoft Corporation.  All rights reserved. 2014
// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY
// OF ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT
// LIMITED TO THE IMPLIED WARRANTIES OF MERCHANTABILITY AND
// FITNESS FOR A PARTICULAR PURPOSE.
// ==============================================================================
// </copyright>
// -----------------------------------------------------------------------

namespace Microsoft.ApplicationInsights.Tracing
{
    using System;
    using System.Globalization;

    using Microsoft.ApplicationInsights;
    using Microsoft.ApplicationInsights.Tracing.Serialization;

    using Newtonsoft.Json;

    /// <summary>
    /// Implementing track telemetry trace .
    /// </summary>
    internal class Tracer
    {
        #region Fields

        /// <summary>
        /// Message maximum length.
        /// </summary>
        private const int MessageMaxLength = 32768;

        
        #endregion

        #region C'tor

        /// <summary>
        /// Initializes a new instance of the <see cref="Tracer"/> class.
        /// </summary>
        /// <param name="instrumentationKey">Application insights Instrumentation Key.</param>
        /// <exception cref="ArgumentNullException">If <paramref name="instrumentationKey"/> is null or empty.</exception>
        public Tracer(string instrumentationKey)
        {
            this.InstrumentationKey = instrumentationKey;
            this.telemetryContext = new TelemetryContext(TelemetryConfiguration.Active);
        }

        #endregion

        #region Public methods

        /// <summary>
        /// Application insights instrumentationKey.
        /// </summary>
        internal string InstrumentationKey { get; set; }

        /// <summary>
        /// Application insights telemetry context.
        /// </summary>
        internal TelemetryContext Context
        {
            get
            {
                return this.telemetryContext;
            }
        }

        /// <summary>
        /// Log simple message with no metadata.
        /// </summary>
        /// <param name="message">A meaningful human readable message.</param>
        public virtual void LogMessage(string message)
        {
            this.SendMessage(message);
        }

        /// <summary>
        /// Log message with additional metadata object.
        /// </summary>
        /// <param name="message">A meaningful human readable message.</param>
        /// <param name="data">Additional metadata about the event.</param>
        public virtual void LogMessageWithData(string message, object data)
        {
            this.SendMessage(message, data);
        }

        /// <summary>
        /// Log message with additional metadata object with custom <see cref="JsonConverter"/>s.
        /// </summary>
        /// <param name="message">A meaningful human readable message.</param>
        /// <param name="data">Additional metadata about the event.</param>
        /// <param name="dataConverters">Custom <seealso cref="JsonConverter"/> for customizing how the data 
        /// object is converted to JSON.</param>
        public virtual void LogMessageWithData(string message, object data, params JsonConverter[] dataConverters)
        {
            this.SendMessage(message, data, dataConverters);
        }

        #endregion

        #region Private methods

        /// <summary>
        /// Sending the message using the Telemetry context.
        /// </summary>
        private void SendMessage(string message, object data = null, params JsonConverter[] dataConverters)
        {
            if (message == null)
            {
                throw new ArgumentNullException(
                    "message",
                    "Message has not been specified. Please provide a value for message.");
            }
            if (message.Length == 0)
            {
                throw new ArgumentException(
                    "Message has not been specified. Please provide a value for message.",
                    "message");
            }
            if (message.Length > MessageMaxLength)
            {
                message = string.Format(
                    CultureInfo.CurrentCulture,
                    "{0}...",
                    message.Substring(0, MessageMaxLength));
            }

            TraceTelemetry trace = this.CreateTraceTelemetry(message, data, dataConverters);
            this.telemetryContext.Track(trace);
        }

        /// <summary>
        /// Creating the Trace Telemetry event.
        /// </summary>
        private TraceTelemetry CreateTraceTelemetry(string message, object data = null, params JsonConverter[] dataConverters)
        {
            var traceTelemetry = new TraceTelemetry()
            {
                // Remaining properties should be set by Core SDK property providers.
                InstrumentationKey = this.InstrumentationKey,
                Timestamp = DateTimeOffset.UtcNow,
                Message = message
            };

            if (data != null)
            {
                Type dataType = data.GetType();
                traceTelemetry.Properties["SourceType"] = dataType.AssemblyQualifiedName;

                string serializedData = Serializer.SerializeData(data, dataConverters);
                traceTelemetry.Properties[dataType.Name] = serializedData;
            }

            return traceTelemetry;
        }

        #endregion
    }
}
