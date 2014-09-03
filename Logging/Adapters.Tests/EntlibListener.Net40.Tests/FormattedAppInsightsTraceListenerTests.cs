// <copyright file="FormattedAppInsightsTraceListenerTests.cs" company="Microsoft">
// ==============================================================================
// Copyright © Microsoft Corporation.  All rights reserved. 2014
// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY
// OF ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT
// LIMITED TO THE IMPLIED WARRANTIES OF MERCHANTABILITY AND
// FITNESS FOR A PARTICULAR PURPOSE.
// ==============================================================================d.
// </copyright>

using System.Diagnostics;
using Microsoft.Practices.EnterpriseLibrary.Common.Configuration;
using Microsoft.Practices.EnterpriseLibrary.Common.Utility;

namespace Microsoft.ApplicationInsights.EntlibTraceListener.Tests
{
    /// <summary>
    /// Partial class.
    /// </summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1001:TypesThatOwnDisposableFieldsShouldBeDisposable", Justification = "Disposing the object on the TestCleanup method")]
    public partial class FormattedAppInsightsTraceListenerTests
    {            
        #region private methods

        private static void VerifyInitializationWithKeyLocal()
        {
            var listener = EnterpriseLibraryContainer.Current.GetInstance<FormattedAppInsightsTraceListener>("AppInsights TraceListener‌implementation");
            VerifyListenerConfigurationAgainstAppConfig(listener);            
        }

        private static TraceListener GetListener(string name, IConfigurationSource configurationSource)
        {
            var container = EnterpriseLibraryContainer.CreateDefaultContainer(configurationSource);
            var listener = container.GetInstance<TraceListener>(name);
            container.Dispose();
            return listener;
        }
        #endregion private methods
    }
}
