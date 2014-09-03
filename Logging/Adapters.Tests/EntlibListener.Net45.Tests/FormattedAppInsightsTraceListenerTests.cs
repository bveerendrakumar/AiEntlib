// <copyright file="FormattedAppInsightsTraceListenerTests.cs" company="Microsoft">
// ==============================================================================
// Copyright © Microsoft Corporation.  All rights reserved. 2014
// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY
// OF ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT
// LIMITED TO THE IMPLIED WARRANTIES OF MERCHANTABILITY AND
// FITNESS FOR A PARTICULAR PURPOSE.
// ==============================================================================.
// </copyright>

using System.Diagnostics;
using System.Linq;
using Microsoft.Practices.EnterpriseLibrary.Common.Configuration;
using Microsoft.Practices.EnterpriseLibrary.Logging;
using Microsoft.Practices.EnterpriseLibrary.Logging.Configuration;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Microsoft.ApplicationInsights.EntlibTraceListener.Tests
{
    /// <summary>
    /// Partial class.
    /// </summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1001:TypesThatOwnDisposableFieldsShouldBeDisposable", Justification = "Disposing the object on the TestCleanup method")]
    public partial class FormattedAppInsightsTraceListenerTests
    {
        #region private methods

        private static FormattedAppInsightsTraceListener GetConfiguredAppInsightsTraceListener(LoggingConfiguration config)
        {          
            return
              (FormattedAppInsightsTraceListener)config.AllTraceListeners.Single(
                        listener => listener is FormattedAppInsightsTraceListener);
        }

        private static void VerifyInitializationSuccess(LoggingConfiguration config)
        {
            Assert.IsNotNull(config);
            var listenerConfigured =
                GetConfiguredAppInsightsTraceListener(config);

            VerifyListenerConfigurationAgainstAppConfig(listenerConfigured);
        }

        private static TraceListener GetListener(string name, IConfigurationSource configurationSource)
        {
            var settings = LoggingSettings.GetLoggingSettings(configurationSource);
            return settings.TraceListeners.Get(name).BuildTraceListener(settings);
        }

        private void VerifyInitializationWithKeyLocal()
        {
            this.writer.Configure(VerifyInitializationSuccess);
        }
        #endregion private methods
    }
}
