// -----------------------------------------------------------------------
// <copyright file="SendToAppInsightsTraceListenerExtension.cs" company="Microsoft">
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
using System.Diagnostics;
using Microsoft.Practices.EnterpriseLibrary.Common.Configuration.Fluent;
using Microsoft.Practices.EnterpriseLibrary.Common.Properties;
using Microsoft.Practices.EnterpriseLibrary.Logging.Configuration;

namespace Microsoft.ApplicationInsights.EntlibTraceListener.Configuration.Fluent
{
    /// <summary>
    /// Extension methods to support configuration of <see cref="FormattedAppInsightsTraceListener"/>.
    /// </summary>
    /// <seealso cref="FormattedAppInsightsTraceListener"/>
    /// <seealso cref="FormattedAppInsightsTraceListenerData"/>
    public static class SendToAppInsightsTraceListenerExtension
    {
        /// <summary>
        /// Adds a new <see cref="FormattedAppInsightsTraceListener"/> to the logging settings and creates
        /// a reference to this Trace Listener for the current category source.
        /// </summary>
        /// <param name="context">Fluent interface extension point.</param>
        /// <param name="listenerName">The name of the <see cref="FormattedAppInsightsTraceListener"/>.</param>
        /// <returns>Fluent interface that can be used to further configure the created <see cref="FormattedAppInsightsTraceListenerData"/>. </returns>
        /// <seealso cref="FormattedAppInsightsTraceListener"/>
        /// <seealso cref="FormattedAppInsightsTraceListenerData"/>
        public static ILoggingConfigurationSendToAppInsightsTraceListener AppInsights(
            this ILoggingConfigurationSendTo context, string listenerName)
        {
            if (string.IsNullOrEmpty(listenerName))
            {
                throw new ArgumentException(Resources.ExceptionStringNullOrEmpty, "listenerName");
            }

            return new SendToAppInsightsTraceListenerBuilder(context, listenerName);
        }

        private class SendToAppInsightsTraceListenerBuilder : SendToTraceListenerExtension, ILoggingConfigurationSendToAppInsightsTraceListener
        {
            private readonly FormattedAppInsightsTraceListenerData appInsightsTraceListenerData;

            public SendToAppInsightsTraceListenerBuilder(ILoggingConfigurationSendTo context, string listenerName)
                : base(context)
            {
                this.appInsightsTraceListenerData = new FormattedAppInsightsTraceListenerData { Name = listenerName };
                this.AddTraceListenerToSettingsAndCategory(this.appInsightsTraceListenerData);
            }

            #region interface members
            public ILoggingConfigurationSendToAppInsightsTraceListener FormatWith(IFormatterBuilder formatBuilder)
            {
                if (formatBuilder == null)
                {
                    throw new ArgumentNullException("formatBuilder");
                }

                FormatterData formatter = formatBuilder.GetFormatterData();
                this.appInsightsTraceListenerData.Formatter = formatter.Name;
                LoggingSettings.Formatters.Add(formatter);

                return this;
            }

            public ILoggingConfigurationSendToAppInsightsTraceListener FormatWithSharedFormatter(string formatterName)
            {
                this.appInsightsTraceListenerData.Formatter = formatterName;
                return this;
            }

            public ILoggingConfigurationSendToAppInsightsTraceListener Filter(SourceLevels sourceLevel)
            {
                this.appInsightsTraceListenerData.Filter = sourceLevel;
                return this;
            }

            public ILoggingConfigurationSendToAppInsightsTraceListener WithTraceOptions(TraceOptions traceOptions)
            {
                this.appInsightsTraceListenerData.TraceOutputOptions = traceOptions;

                return this;
            }

            public ILoggingConfigurationSendToAppInsightsTraceListener WithInstrumentationKey(string instrumentationKey)
            {
                if (string.IsNullOrEmpty(instrumentationKey))
                {
                    throw new ArgumentException(Resources.ExceptionStringNullOrEmpty, "instrumentationKey");
                }

                this.appInsightsTraceListenerData.InstrumentationKey = instrumentationKey;
                return this;
            }
            #endregion interface members
        }
    }
}
