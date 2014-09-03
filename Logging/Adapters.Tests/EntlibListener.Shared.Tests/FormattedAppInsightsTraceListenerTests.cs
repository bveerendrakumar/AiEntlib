// <copyright file="FormattedAppInsightsTraceListenerTests.cs" company="Microsoft">
// ==============================================================================
// Copyright © Microsoft Corporation.  All rights reserved. 2014
// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY
// OF ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT
// LIMITED TO THE IMPLIED WARRANTIES OF MERCHANTABILITY AND
// FITNESS FOR A PARTICULAR PURPOSE.
// ==============================================================================
// </copyright>

using Microsoft.ApplicationInsights.EntlibTraceListener.Tests.Configuration;
using Microsoft.Practices.EnterpriseLibrary.Common.Configuration;
using Microsoft.Practices.EnterpriseLibrary.Logging;
using Microsoft.Practices.EnterpriseLibrary.Logging.Configuration;
using Microsoft.Practices.EnterpriseLibrary.Logging.Formatters;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Configuration;
using System.Diagnostics;
using System.Linq;

namespace Microsoft.ApplicationInsights.EntlibTraceListener.Tests
{
    using Tracing.Tests;

    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1001:TypesThatOwnDisposableFieldsShouldBeDisposable", Justification = "Disposing the object on the TestCleanup method")]
    [System.Diagnostics.CodeAnalysis.SuppressMessage("StyleCop.CSharp.ReadabilityRules", "SA1101:PrefixLocalCallsWithThis", Justification = "Reviewed. Suppression is OK here.")]

    /// <summary>
    /// Partial class.
    /// </summary>
    [TestClass]
    public partial class FormattedAppInsightsTraceListenerTests
    {
        #region private variables
        
        private AdapterHelper adapterHelper;
        private LogWriter writer;

        #endregion private variables

        #region Test setup

        [TestInitialize]
        public void Initialize()
        {
            var factory = new LogWriterFactory();
            this.writer = factory.Create();
            this.adapterHelper = new AdapterHelper();
        }

        [TestCleanup]
        public void Cleanup()
        {
            this.adapterHelper.Dispose();
            this.writer.Dispose();
        }

        #endregion Test setup

        #region Test cases

        [TestMethod]
        [TestCategory("EntlibTraceListener")]
        public void VerifyInitializationWithKey()
        {
            VerifyInitializationWithKeyLocal();
        }

        [TestMethod]
        [TestCategory("EntlibTraceListener")]
        public void LogByApplyingFilter()
        {
            var listener = new FormattedAppInsightsTraceListener(this.adapterHelper.InstrumentationKey, new TextFormatter("TEST{newline}TEST"))
            {
                Filter = new EventTypeFilter(SourceLevels.Information)
            };
            var channel = this.adapterHelper.Channel;
            listener.Tracer.Context.TelemetryChannel = channel;

            listener.TraceData(new TraceEventCache(), "MockCateogry", TraceEventType.Error, 0, new LogEntry("message", "MockCateogry", 0, 0, TraceEventType.Error, "title", null));

            Assert.AreEqual(true, this.adapterHelper.Channel.SentItems.Length == 1);
        }

        [TestMethod]
        [TestCategory("EntlibTraceListener")]
        public void LogWithConfigValuesInAppConfiguration()
        {
            var listener = this.GetListenerFromConfig();
            this.ModifyListenerChannelFromAdapterHelper(listener);
            listener.Write("test message");
            Assert.AreEqual(true, this.adapterHelper.Channel.SentItems.Length == 1);
        }

        [TestMethod]
        [TestCategory("EntlibTraceListener")]
        public void ValidateWithMultipleCategories()
        {
            var listener = this.GetListenerFromConfig();
            this.ModifyListenerChannelFromAdapterHelper(listener);
            var logEntry = new LogEntry { Message = "message" };
            logEntry.Categories.Add("General");
            logEntry.Categories.Add("MockCategory");
            logEntry.EventId = 123;
            logEntry.Priority = 11;
            logEntry.Severity = TraceEventType.Error;
            logEntry.Title = "Test Case";

            this.TraceMessagesUsingListener(listener, logEntry);

            Assert.AreEqual(true, this.adapterHelper.Channel.SentItems.Length == 2);
        }
       
        [TestMethod]
        [TestCategory("EntlibTraceListener")]
        public void ValidateWithThresholdCount()
        {
            const int Threshold = 499;
            var listener = this.GetListenerFromConfig();
            this.ModifyListenerChannelFromAdapterHelper(listener);
            var logEntry = new LogEntry { Message = "message" };
            logEntry.Categories.Add("General");
            logEntry.EventId = 123;
            logEntry.Priority = 11;
            logEntry.Severity = TraceEventType.Error;
            logEntry.Title = "Test Case";

            for (var i = 0; i < Threshold; i++)
            {
                logEntry.Message = "Trace Debug" + i + DateTime.Now;
                this.TraceMessagesUsingListener(listener, logEntry);
            }

            Assert.AreEqual(true, this.adapterHelper.Channel.SentItems.Length == Threshold);
        }

        #endregion Test cases

        #region private methods

        private static void VerifyListenerConfigurationAgainstAppConfig(FormattedAppInsightsTraceListener listenerConfigured)
        {
            Assert.IsNotNull(listenerConfigured);

            Assert.AreEqual(ConfigurationTestHelper.ConditionalListenerName, listenerConfigured.Name);

            var key = listenerConfigured.InstrumentationKey;

            Assert.AreEqual(ConfigurationTestHelper.InstrumentaionKey, key);
        }

        private void TraceMessagesUsingListener(FormattedAppInsightsTraceListener listener, LogEntry logEntry)
        {
            Assert.IsNotNull(listener);

            foreach (var categorySource in logEntry.Categories)
            {
                listener.TraceData(new TraceEventCache(), categorySource, logEntry.Severity, logEntry.EventId, logEntry);
            }                                
        }

        private FormattedAppInsightsTraceListener GetListenerFromConfig()
        {
            var listener = (FormattedAppInsightsTraceListener)GetListener(ConfigurationTestHelper.ConditionalListenerName, ConfigurationTestHelper.AppConfigurationLoggSource);
            Assert.IsNotNull(listener);
            return listener;
        }

        private void ModifyListenerChannelFromAdapterHelper(FormattedAppInsightsTraceListener listener)
        {
            if (listener == null)
            {
                return;
            }
            var channel = this.adapterHelper.Channel;
            listener.Tracer.Context.TelemetryChannel = channel;

            LogSource logSource;
            this.writer.TraceSources.TryGetValue("General", out logSource);
        }        

        private string GetConditionalListenerName()
        {
            var listenerName = string.Empty;
#if NET40
            listenerName = "listener\u200cimplementation";
#else
            listenerName = "listener";
#endif
            return listenerName;
        }
        #endregion private methods
    }
}
