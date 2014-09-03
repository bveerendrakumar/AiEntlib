// -----------------------------------------------------------------------
// <copyright file="ILoggingConfigurationSendToAppInsightsTraceListener.cs" company="Microsoft">
// ==============================================================================
// Copyright © Microsoft Corporation.  All rights reserved. 2014
// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY
// OF ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT
// LIMITED TO THE IMPLIED WARRANTIES OF MERCHANTABILITY AND
// FITNESS FOR A PARTICULAR PURPOSE.
// ==============================================================================
// </copyright>
// -----------------------------------------------------------------------

using System.Diagnostics;
using Microsoft.Practices.EnterpriseLibrary.Common.Configuration;
using Microsoft.Practices.EnterpriseLibrary.Common.Configuration.Fluent;
using Microsoft.Practices.EnterpriseLibrary.Logging.Configuration;
using Microsoft.Practices.EnterpriseLibrary.Logging.Formatters;

namespace Microsoft.ApplicationInsights.EntlibTraceListener.Configuration.Fluent
{ 
    /// <summary>
    /// Fluent interface used to configure a <see cref="FormattedAppInsightsTraceListener"/> instance.
    /// </summary>
    /// <seealso cref="FormattedAppInsightsTraceListener"/>
    /// <seealso cref="FormattedAppInsightsTraceListenerData"/>
    public interface ILoggingConfigurationSendToAppInsightsTraceListener : ILoggingConfigurationCategoryContd
    {
        /// <summary>
        /// Specifies the formatter used to format email messages send by this <see cref="FormattedAppInsightsTraceListener"/>.<br/>
        /// </summary>
        /// <param name="formatBuilder">The <see cref="FormatterBuilder"/> used to create an <see cref="LogFormatter"/> .</param>
        /// <returns>Fluent interface that can be used to further configure the created <see cref="FormattedAppInsightsTraceListenerData"/>. </returns>
        /// <seealso cref="FormattedAppInsightsTraceListener"/>
        /// <seealso cref="FormattedAppInsightsTraceListenerData"/>
        ILoggingConfigurationSendToAppInsightsTraceListener FormatWith(IFormatterBuilder formatBuilder);

        /// <summary>
        /// Specifies the formatter used to format email messages send by this <see cref="FormattedAppInsightsTraceListener"/>.<br/>
        /// </summary>
        /// <param name="formatterName">The name of a <see cref="FormatterData"/> configured elsewhere in this section.</param>
        /// <returns>Fluent interface that can be used to further configure the created <see cref="FormattedAppInsightsTraceListenerData"/>. </returns>
        /// <seealso cref="FormattedAppInsightsTraceListener"/>
        /// <seealso cref="FormattedAppInsightsTraceListenerData"/>
        ILoggingConfigurationSendToAppInsightsTraceListener FormatWithSharedFormatter(string formatterName);

        /// <summary>
        /// Specifies the <see cref="SourceLevels"/> that should be used to filter trace output by this <see cref="FormattedAppInsightsTraceListener"/>.
        /// </summary>
        /// <param name="sourceLevel">The <see cref="SourceLevels"/> that should be used to filter trace output .</param>
        /// <returns>Fluent interface that can be used to further configure the created <see cref="FormattedAppInsightsTraceListenerData"/>. </returns>
        /// <seealso cref="FormattedAppInsightsTraceListener"/>
        /// <seealso cref="FormattedAppInsightsTraceListenerData"/>
        /// <seealso cref="SourceLevels"/>
        ILoggingConfigurationSendToAppInsightsTraceListener Filter(SourceLevels sourceLevel);

        /// <summary>
        /// Specifies which options, or elements, should be included in messages send by this <see cref="FormattedAppInsightsTraceListener"/>.<br/>
        /// </summary>
        /// <param name="traceOptions">The options that should be included in the trace output.</param>
        /// <returns>Fluent interface that can be used to further configure the created <see cref="FormattedAppInsightsTraceListenerData"/>. </returns>
        /// <seealso cref="FormattedAppInsightsTraceListener"/>
        /// <seealso cref="FormattedAppInsightsTraceListenerData"/>
        /// <seealso cref="TraceOptions"/>
        ILoggingConfigurationSendToAppInsightsTraceListener WithTraceOptions(TraceOptions traceOptions);

        /// <summary>
        /// Specifies the component id (registered in app insights portal) that should be used to log the data.
        /// </summary>        
        /// <param name="instrumentationKey">Instrumentation key of application.</param>
        /// <returns>Fluent interface that can be used to further configure the current <see cref="FormattedAppInsightsTraceListener"/> instance. </returns>
        /// <seealso cref="FormattedAppInsightsTraceListener"/>
        /// <seealso cref="FormattedAppInsightsTraceListenerData"/>
        ILoggingConfigurationSendToAppInsightsTraceListener WithInstrumentationKey(string instrumentationKey);
    }
}
