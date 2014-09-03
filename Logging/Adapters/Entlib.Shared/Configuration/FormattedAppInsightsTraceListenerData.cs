// -----------------------------------------------------------------------
// <copyright file="FormattedAppInsightsTraceListenerData.cs" company="Microsoft">
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
using System.Configuration;
using System.Diagnostics;
using System.Linq.Expressions;
using Microsoft.Practices.EnterpriseLibrary.Common.Configuration;
using Microsoft.Practices.EnterpriseLibrary.Common.Configuration.Design;
using Microsoft.Practices.EnterpriseLibrary.Logging.Configuration;
using Microsoft.Practices.EnterpriseLibrary.Logging.Formatters;

namespace Microsoft.ApplicationInsights.EntlibTraceListener.Configuration
{
    /// <summary>
    /// Configuration object for a <see cref="FormattedAppInsightsTraceListener"/>.
    /// </summary>
    public partial class FormattedAppInsightsTraceListenerData : TraceListenerData
    {
        private const string InstrumentationKeyProperty = "componentId";
        private const string FormatterNameProperty = "formatter";

        /// <summary>
        /// Initializes a <see cref="FormattedAppInsightsTraceListenerData"/>.
        /// </summary>
        public FormattedAppInsightsTraceListenerData()
            : base(typeof(FormattedAppInsightsTraceListener))
        {
            this.ListenerDataType = typeof(FormattedAppInsightsTraceListener);
        }

        /// <summary>
        /// Initializes a <see cref="FormattedAppInsightsTraceListenerData"/>.
        /// </summary>
        /// <param name="instrumentationKey">Component id used to send the logs which identifies component uniquely.</param>
        public FormattedAppInsightsTraceListenerData(string instrumentationKey)
            : this("unnamed", instrumentationKey)
        {
        }

        /// <summary>
        /// Initializes a <see cref="FormattedAppInsightsTraceListenerData"/>.
        /// </summary>
        /// <param name="name">Name of the listener.</param>
        /// <param name="instrumentationKey">Component id used to send the logs which identifies component uniquely.</param>
        public FormattedAppInsightsTraceListenerData(string name, string instrumentationKey)
            : this(name, instrumentationKey, string.Empty)
        {
        }

        /// <summary>
        /// Initializes a <see cref="FormattedAppInsightsTraceListenerData"/>.
        /// </summary>
        /// <param name="name">Name of the listener.</param>
        /// <param name="instrumentationKey">Component id used to send the logs which identifies component uniquely.</param>
        /// <param name="formatter">Formatter name to be used to format the message.</param>
        public FormattedAppInsightsTraceListenerData(string name, string instrumentationKey, string formatter)
            : this(name, typeof(FormattedAppInsightsTraceListener), instrumentationKey, TraceOptions.None, formatter)
        {
        }

        /// <summary>
        /// Initializes a <see cref="FormattedAppInsightsTraceListenerData"/>.
        /// </summary>
        /// <param name="name">Name of the listener.</param>
        /// <param name="instrumentationKey">Component id used to send the logs which identifies component uniquely.</param>
        /// <param name="formatter">Formatter name to be used to format the message.</param>
        /// <param name="traceOutputOptions">Specifies trace data options to be written to the trace output.</param>
        public FormattedAppInsightsTraceListenerData(string name, string instrumentationKey, string formatter, TraceOptions traceOutputOptions)
            : this(name, typeof(FormattedAppInsightsTraceListener), instrumentationKey, traceOutputOptions, formatter)
        {
        }

        /// <summary>
        /// Initializes a <see cref="FormattedAppInsightsTraceListenerData"/>.
        /// </summary>
        /// <param name="name">Name of the listener.</param>
        /// <param name="listenerType">Type of the listener.</param>
        /// <param name="instrumentationKey">Component id used to send the logs which identifies component uniquely.</param>
        /// <param name="traceOutputOptions">Specifies trace data options to be written to the trace output.</param>
        /// <param name="formatterName">Formatter name to be used to format the message.</param>
        private FormattedAppInsightsTraceListenerData(string name, Type listenerType, string instrumentationKey, TraceOptions traceOutputOptions, string formatterName)
            : base(name, listenerType, traceOutputOptions)
        {
            this.InstrumentationKey = instrumentationKey;
            this.Formatter = formatterName;
        }

        /// <summary>
        /// Gets or sets the InstrumentationKey.
        /// </summary>
        [ConfigurationProperty(InstrumentationKeyProperty, IsRequired = false)]
        public string InstrumentationKey
        {
            get { return (string)base[InstrumentationKeyProperty]; }
            set { base[InstrumentationKeyProperty] = value; }
        }

        /// <summary>
        /// Gets and sets the formatter name.
        /// </summary>
        [ConfigurationProperty(FormatterNameProperty, IsRequired = false)]        
        [Reference(typeof(NameTypeConfigurationElementCollection<FormatterData, CustomFormatterData>), typeof(FormatterData))]
        public string Formatter
        {
            get { return (string)base[FormatterNameProperty]; }
            set { base[FormatterNameProperty] = value; }
        }
    }
}
