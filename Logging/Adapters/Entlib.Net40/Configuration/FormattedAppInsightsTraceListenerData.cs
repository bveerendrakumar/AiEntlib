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
using System.Diagnostics;
using System.Linq.Expressions;
using Microsoft.Practices.EnterpriseLibrary.Common.Configuration.ContainerModel;
using Microsoft.Practices.EnterpriseLibrary.Logging.Formatters;

namespace Microsoft.ApplicationInsights.EntlibTraceListener.Configuration
{
    /// <summary>
    /// Configuration Data type for the FormattedAppInsightsTraceListener.
    /// </summary>
    public partial class FormattedAppInsightsTraceListenerData
    {
        /// <summary>
        /// Returns a lambda expression that represents the creation of the trace listener described by this
        /// configuration object.
        /// </summary>
        /// <returns>A lambda expression to create a trace listener.</returns>
        protected override Expression<Func<TraceListener>> GetCreationExpression()
        {
            return () =>
                      new FormattedAppInsightsTraceListener(this.InstrumentationKey, Container.ResolvedIfNotNull<ILogFormatter>(this.Formatter));
        }        
    }
}
