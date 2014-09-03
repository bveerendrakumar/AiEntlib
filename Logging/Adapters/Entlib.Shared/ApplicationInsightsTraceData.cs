// -----------------------------------------------------------------------
// <copyright file="ApplicationInsightsTraceData.cs" company="Microsoft">
// ==============================================================================
// Copyright © Microsoft Corporation.  All rights reserved. 2014
// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY
// OF ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT
// LIMITED TO THE IMPLIED WARRANTIES OF MERCHANTABILITY AND
// FITNESS FOR A PARTICULAR PURPOSE.
// ==============================================================================
// </copyright>
// -----------------------------------------------------------------------

namespace Microsoft.ApplicationInsights.EntlibTraceListener
{
    using Newtonsoft.Json;
    using Newtonsoft.Json.Converters;
    using Newtonsoft.Json.Linq;
    using System;
    using System.Collections;
    using System.Diagnostics;
    using System.Diagnostics.CodeAnalysis;
    using System.Runtime.Serialization;
    
    /// <summary>
    /// Wrapper class for trace related data.
    /// </summary>
    [Serializable]
    internal class ApplicationInsightsTraceData : 
        ISerializable
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ApplicationInsightsTraceData"/> class.
        /// </summary>
        public ApplicationInsightsTraceData()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ApplicationInsightsTraceData"/> class.
        /// </summary>
        /// <param name="info">The information.</param>
        /// <param name="context">The context.</param>
        [SuppressMessage("Microsoft.Performance", "CA1800:DoNotCastUnnecessarily", Justification = "Never casted twice on the same code path")]
        public ApplicationInsightsTraceData(SerializationInfo info, StreamingContext context)
        {
            foreach (SerializationEntry entry in info)
            {
                object value = entry.Value;

                // If the serialization constructor is called by JSON.NET we will be given JValue objects we need to convert to object.
                JValue jvalue = entry.Value as JValue;
                JArray array;
                if (jvalue != null)
                {
                    value = jvalue.Value;
                }

                switch (entry.Name)
                {
                    case "TraceEventType":
                        this.TraceEventType = (TraceEventType)Enum.Parse(typeof(TraceEventType), (string)value);
                        break;
                    case "EventId":
                        this.EventId = (long)value;
                        break;
                    case "Timestamp":
                        this.Timestamp = (long)value;
                        break;
                    case "LogicalOperationStack":
                        array = value as JArray;
                        if (array != null)
                        {
                            this.LogicalOperationStack = new Stack(JsonConvert.DeserializeObject<object[]>(array.ToString()));
                        }
                        else
                        {
                            this.LogicalOperationStack = (Stack)value;    
                        }
                        
                        break;
                    case "Callstack":
                        this.Callstack = (string)value;
                        break;
                    case "AdditionalData":
                        array = value as JArray;
                        if (array != null)
                        {
                            this.AdditionalData = JsonConvert.DeserializeObject<object[]>(array.ToString());
                        }
                        else
                        {
                            this.AdditionalData = (object[])value;    
                        }
                        
                        break;
                }
            }
        }

        /// <summary>
        /// Gets or sets the type of the trace event.
        /// </summary>
        [JsonConverter(typeof(StringEnumConverter))]
        [JsonProperty(Required = Required.Always)]
        public TraceEventType TraceEventType { get; set; }

        /// <summary>
        /// Gets or sets the event identifier.
        /// </summary>
        [JsonProperty(Required = Required.Always)]
        public long EventId { get; set; }

        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Ignore)]
        public long Timestamp { get; set; }

        /// <summary>
        /// Gets or sets the logical operation stack.
        /// </summary>
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public Stack LogicalOperationStack { get; set; }

        /// <summary>
        /// Gets or sets the call stack.
        /// </summary>
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string Callstack { get; set; }

        /// <summary>
        /// Gets or sets the additional data.
        /// </summary>
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public object[] AdditionalData { get; set; }

        /// <summary>
        /// Populates a <see cref="T:System.Runtime.Serialization.SerializationInfo" /> with the data needed to serialize the target object.
        /// </summary>
        /// <param name="info">The <see cref="T:System.Runtime.Serialization.SerializationInfo" /> to populate with data.</param>
        /// <param name="context">The destination (see <see cref="T:System.Runtime.Serialization.StreamingContext" />) for this serialization.</param>
        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            if (info == null)
            {
                throw new ArgumentNullException("info");
            }

            info.AddValue("TraceEventType", this.TraceEventType.ToString(), typeof(string));
            info.AddValue("EventId", this.EventId);
            if (this.Timestamp != 0)
            {
                info.AddValue("Timestamp", this.Timestamp);
            }
            if (this.LogicalOperationStack != null)
            {
                info.AddValue("LogicalOperationStack", this.LogicalOperationStack, typeof(Stack));
            }
            if (this.Callstack != null)
            {
                info.AddValue("Callstack", this.Callstack, typeof(string));
            }
            if (this.AdditionalData != null)
            {
                info.AddValue("AdditionalData", this.AdditionalData, typeof(object[]));
            }
        }
    }
}
