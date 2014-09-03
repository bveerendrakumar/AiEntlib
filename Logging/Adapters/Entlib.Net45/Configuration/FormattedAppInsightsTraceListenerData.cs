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

using System.Diagnostics;
using Microsoft.Practices.EnterpriseLibrary.Logging.Configuration;

namespace Microsoft.ApplicationInsights.EntlibTraceListener.Configuration
{
    /// <summary>
    /// Configuration Data type for the FormattedAppInsightsTraceListener.
    /// </summary>
    public partial class FormattedAppInsightsTraceListenerData
    {
        /// <summary>
        /// Builds the <see cref="TraceListener" /> object represented by this configuration object.
        /// </summary>
        /// <param name="settings">The logging configuration settings.</param>
        /// <returns>
        /// An <see cref="FormattedAppInsightsTraceListener"/>.
        /// </returns>
        protected override TraceListener CoreBuildTraceListener(LoggingSettings settings)
        {
            var formatter = this.BuildFormatterSafe(settings, this.Formatter);

            return new FormattedAppInsightsTraceListener(this.InstrumentationKey, formatter);           
        }
    }
}
