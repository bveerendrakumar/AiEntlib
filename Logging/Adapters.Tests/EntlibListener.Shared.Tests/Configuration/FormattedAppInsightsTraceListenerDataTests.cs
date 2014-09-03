// <copyright file="FormattedAppInsightsTraceListenerDataTests.cs" company="Microsoft">
// ==============================================================================
// Copyright © Microsoft Corporation.  All rights reserved. 2014
// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY
// OF ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT
// LIMITED TO THE IMPLIED WARRANTIES OF MERCHANTABILITY AND
// FITNESS FOR A PARTICULAR PURPOSE.
// ==============================================================================
// </copyright>

using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using Microsoft.ApplicationInsights.EntlibTraceListener.Configuration;
using Microsoft.Practices.EnterpriseLibrary.Common.Configuration;
using Microsoft.Practices.EnterpriseLibrary.Logging.Configuration;
using Microsoft.Practices.EnterpriseLibrary.Logging.Formatters;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Microsoft.ApplicationInsights.EntlibTraceListener.Tests.Configuration
{
    /// <summary>
    /// Partial class.
    /// </summary>
    [TestClass]
    public partial class FormattedAppInsightsTraceListenerConfigurationTests
    {
        private string instrumentationKey = string.Empty;

        [TestInitialize]
        public void SetUp()
        {
            AppDomain.CurrentDomain.SetData("APPBASE", Environment.CurrentDirectory);
            this.instrumentationKey = ConfigurationTestHelper.InstrumentaionKey;
        }

        [TestCleanup]
        public void Cleanup()
        {           
        }

        [TestMethod]
        [TestCategory("EntlibTraceListener")]
        public void ListenerDataIsCreatedCorrectly()
        {
            var listenerData =
                new FormattedAppInsightsTraceListenerData("listener", "93d9c2b7-e633-4571-8520-d391511a1df5", "formatter");

            Assert.AreSame(typeof(FormattedAppInsightsTraceListener), listenerData.Type);
            Assert.AreSame(typeof(FormattedAppInsightsTraceListenerData), listenerData.ListenerDataType);
            Assert.AreEqual("listener", listenerData.Name);
            Assert.AreEqual(this.instrumentationKey, listenerData.InstrumentationKey);
            Assert.AreEqual(TraceOptions.None.ToString(), listenerData.TraceOutputOptions.ToString());
            Assert.AreEqual("formatter", listenerData.Formatter);
        }

        [TestMethod]
        [TestCategory("EntlibTraceListener")]
        public void CanDeserializeSerializedConfiguration()
        {
            var listenerName = "listener";
            var key = this.instrumentationKey;
            const string Formatter = "formatter";

            TraceListenerData data =
                new FormattedAppInsightsTraceListenerData(listenerName, key, Formatter, TraceOptions.Callstack);

            var settings = new LoggingSettings();
            settings.TraceListeners.Add(data);

            IDictionary<string, ConfigurationSection> sections = new Dictionary<string, ConfigurationSection>();
            sections[LoggingSettings.SectionName] = settings;
            IConfigurationSource configurationSource
                = ConfigurationTestHelper.SaveSectionsInFileAndReturnConfigurationSource(sections);

            var roSettigs = (LoggingSettings)configurationSource.GetSection(LoggingSettings.SectionName);

            Assert.AreEqual(1, roSettigs.TraceListeners.Count);
            Assert.IsNotNull(roSettigs.TraceListeners.Get(listenerName));
            Assert.AreEqual(TraceOptions.Callstack, roSettigs.TraceListeners.Get(listenerName).TraceOutputOptions);
            Assert.AreSame(typeof(FormattedAppInsightsTraceListenerData), roSettigs.TraceListeners.Get(listenerName).GetType());
            Assert.AreSame(typeof(FormattedAppInsightsTraceListenerData), roSettigs.TraceListeners.Get(listenerName).ListenerDataType);
            Assert.AreSame(typeof(FormattedAppInsightsTraceListener), roSettigs.TraceListeners.Get(listenerName).Type);
            Assert.AreEqual(key, ((FormattedAppInsightsTraceListenerData)roSettigs.TraceListeners.Get(listenerName)).InstrumentationKey);            
        }

        [TestMethod]
        [TestCategory("EntlibTraceListener")]
        public void CanDeserializeSerializedConfigurationWithDefaults()
        {
            var rwLoggingSettings = new LoggingSettings();
            rwLoggingSettings.TraceListeners.Add(
                new FormattedAppInsightsTraceListenerData("listener1", this.instrumentationKey, "formatter"));
            rwLoggingSettings.TraceListeners.Add(
                new FormattedAppInsightsTraceListenerData("listener2", this.instrumentationKey, "formatter"));

            IDictionary<string, ConfigurationSection> sections = new Dictionary<string, ConfigurationSection>();
            sections[LoggingSettings.SectionName] = rwLoggingSettings;
            var configurationSource
                = ConfigurationTestHelper.SaveSectionsInFileAndReturnConfigurationSource(sections);

            var roLoggingSettings = (LoggingSettings)configurationSource.GetSection(LoggingSettings.SectionName);

            Assert.AreEqual(2, roLoggingSettings.TraceListeners.Count);
            Assert.IsNotNull(roLoggingSettings.TraceListeners.Get("listener1"));
            Assert.IsNotNull(roLoggingSettings.TraceListeners.Get("listener2"));
        }

        [TestMethod]
        [TestCategory("EntlibTraceListener")]
        public void CanCreateInstanceFromGivenName()
        {
            var listenerData =
                new FormattedAppInsightsTraceListenerData(ConfigurationTestHelper.ListenerName, this.instrumentationKey, "formatter")
                {
                    TraceOutputOptions = TraceOptions.Callstack | TraceOptions.DateTime,
                    Filter = SourceLevels.Information
                };
            var helper = new MockLogObjectsHelper();
            helper.LoggingSettings.Formatters.Add(new TextFormatterData("formatter", "some template"));
            helper.LoggingSettings.TraceListeners.Add(listenerData);

            var listener = (FormattedAppInsightsTraceListener)GetListener(ConfigurationTestHelper.ConditionalListenerName, helper.ConfigurationSource);

            Assert.IsNotNull(listener);
            Assert.AreEqual(ConfigurationTestHelper.ConditionalListenerName, listener.Name);
            Assert.AreEqual(TraceOptions.Callstack | TraceOptions.DateTime, listener.TraceOutputOptions);
            Assert.IsNotNull(listener.Filter);
            Assert.AreEqual(SourceLevels.Information, ((EventTypeFilter)listener.Filter).EventType);
            Assert.AreEqual(this.instrumentationKey, listener.InstrumentationKey);
           Assert.IsNotNull(listener.Formatter);
            Assert.AreEqual(listener.Formatter.GetType(), typeof(TextFormatter));
            Assert.AreEqual("some template", ((TextFormatter)listener.Formatter).Template);
        }

        [TestMethod]
        [TestCategory("EntlibTraceListener")]
        public void CanCreateInstanceFromConfigurationFile()
        {
            var loggingSettings = new LoggingSettings();
            loggingSettings.Formatters.Add(new TextFormatterData("formatter", "some template"));
            loggingSettings.TraceListeners.Add(
                 new FormattedAppInsightsTraceListenerData(ConfigurationTestHelper.ListenerName, this.instrumentationKey, "formatter"));

            var listener = GetListener(ConfigurationTestHelper.ConditionalListenerName, ConfigurationTestHelper.SaveSectionsAndGetConfigurationSource(loggingSettings));
            
            Assert.IsNotNull(listener);
            Assert.AreEqual(listener.GetType(), typeof(FormattedAppInsightsTraceListener));
            Assert.IsNotNull(((FormattedAppInsightsTraceListener)listener).Formatter);
            Assert.AreEqual(((FormattedAppInsightsTraceListener)listener).Formatter.GetType(), typeof(TextFormatter));
            Assert.AreEqual("some template", ((TextFormatter)((FormattedAppInsightsTraceListener)listener).Formatter).Template);
        }

        [TestMethod]
        [TestCategory("EntlibTraceListener")]
        public void CanCreateInstanceWithNoFormatter()
        {
            var loggingSettings = new LoggingSettings();
            loggingSettings.TraceListeners.Add(
                new FormattedAppInsightsTraceListenerData(ConfigurationTestHelper.ListenerName, this.instrumentationKey));

            var listener = GetListener(ConfigurationTestHelper.ConditionalListenerName, ConfigurationTestHelper.SaveSectionsAndGetConfigurationSource(loggingSettings));
            Assert.IsNotNull(listener);
            Assert.AreEqual(listener.GetType(), typeof(FormattedAppInsightsTraceListener));
            Assert.IsNull(((FormattedAppInsightsTraceListener)listener).Formatter);
        }      
    }
}