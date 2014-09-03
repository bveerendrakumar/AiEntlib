// <copyright file="SendToAppInsightsTraceListenerExtensionTests.cs" company="Microsoft">
// ==============================================================================
// Copyright © Microsoft Corporation.  All rights reserved. 2014
// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY
// OF ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT
// LIMITED TO THE IMPLIED WARRANTIES OF MERCHANTABILITY AND
// FITNESS FOR A PARTICULAR PURPOSE.
// ==============================================================================
// </copyright>

using System;
using System.Diagnostics;
using System.Linq;
using Microsoft.ApplicationInsights.EntlibTraceListener;
using Microsoft.ApplicationInsights.EntlibTraceListener.Configuration;
using Microsoft.ApplicationInsights.EntlibTraceListener.Configuration.Fluent;
using Microsoft.Practices.EnterpriseLibrary.Common.Configuration;
using Microsoft.Practices.EnterpriseLibrary.Common.Configuration.Fluent;
using Microsoft.Practices.EnterpriseLibrary.Logging.Configuration;
using Microsoft.VisualStudio.TestTools.UnitTesting;

[module: System.Diagnostics.CodeAnalysis.SuppressMessage("StyleCop.CSharp.MaintainabilityRules", "SA1401:FieldsMustBePrivate", Justification = "Reviewed. Suppression is OK here.")]
[module: System.Diagnostics.CodeAnalysis.SuppressMessage("StyleCop.CSharp.MaintainabilityRules", "SA1402:FileMayOnlyContainASingleClass", Justification = "Reviewed. Suppression is OK here.")]

namespace Microsoft.ApplicationInsights.EntlibTraceListener.Tests.Configuration
{
    public abstract class GivenLoggingCategorySourceInConfigurationSourceBuilder : ArrangeActAssert
    {      
        protected ILoggingConfigurationCustomCategoryStart categorySourceBuilder;
        protected ILoggingConfigurationStart configureLogging;
        protected ConfigurationSourceBuilder configurationSourceBuilder;
        private const string CategoryName = "category";

        protected override void Arrange()
        {
            this.configurationSourceBuilder = new ConfigurationSourceBuilder();
            this.configureLogging = this.configurationSourceBuilder.ConfigureLogging();
            this.categorySourceBuilder = this.configureLogging.LogToCategoryNamed(CategoryName);
        }

        protected LoggingSettings GetLoggingConfiguration()
        {
            var configurationSource = this.GetConfigurationSource();
            var loggingSettings = (LoggingSettings)configurationSource.GetSection(LoggingSettings.SectionName);

            return loggingSettings;
        }

        protected IConfigurationSource GetConfigurationSource()
        {
            var configSource = new DictionaryConfigurationSource();
            this.configurationSourceBuilder.UpdateConfigurationWithReplace(configSource);

            return configSource;
        }

        protected TraceSourceData GetTraceSourceData()
        {
            return this.GetLoggingConfiguration().TraceSources.First(x => x.Name == CategoryName);
        }
    }

    public abstract class GivenAppInsightsListenerInConfigurationSourceBuilder : GivenLoggingCategorySourceInConfigurationSourceBuilder
    {
        protected ILoggingConfigurationSendToAppInsightsTraceListener appInsightsListenerBuilder;
        private const string AppinsightsListenerName = "AppInsights listener";

        protected override void Arrange()
        {
            base.Arrange();

            this.appInsightsListenerBuilder = categorySourceBuilder.SendTo.AppInsights(AppinsightsListenerName);
        }

        protected FormattedAppInsightsTraceListenerData GetAppInsightsTraceListenerData()
        {
            return this.GetLoggingConfiguration().TraceListeners.OfType<FormattedAppInsightsTraceListenerData>().First(x => x.Name == AppinsightsListenerName);
        }
    }
  
    [TestClass]
    public class WhenCallingSendToAppInsightsListenerOnLogToCategoryConfigurationBuilder : GivenAppInsightsListenerInConfigurationSourceBuilder
    {
        [TestMethod]
        [TestCategory("EntlibTraceListener")]
        public void ThenTraceOptionsIsNone()
        {
            Assert.AreEqual(TraceOptions.None, GetAppInsightsTraceListenerData().TraceOutputOptions);
        }

        [TestMethod]
        [TestCategory("EntlibTraceListener")]
        public void ThenFilterIsAll()
        {
            Assert.AreEqual(SourceLevels.All, GetAppInsightsTraceListenerData().Filter);
        }

        [TestMethod]
        [TestCategory("EntlibTraceListener")]
        public void ThenAppInsightsTraceListenerDataHasAppropriateType()
        {
            Assert.AreEqual(typeof(FormattedAppInsightsTraceListener), GetAppInsightsTraceListenerData().Type);
        }

        [TestMethod]
        [TestCategory("EntlibTraceListener")]
        public void ThenCategortyContainsTraceListenerReference()
        {
            Assert.AreEqual(GetAppInsightsTraceListenerData().Name, GetTraceSourceData().TraceListeners.First().Name);
        }

        [TestMethod]
        [TestCategory("EntlibTraceListener")]
        public void ThenLoggingConfigurationContainsTraceListener()
        {
            Assert.IsTrue(GetLoggingConfiguration().TraceListeners.OfType<FormattedAppInsightsTraceListenerData>().Any());
        }

        [TestMethod]
        [TestCategory("EntlibTraceListener")]
        public void ThenAddInstrumentationKey()
        {
            Assert.AreEqual(string.Empty, GetAppInsightsTraceListenerData().InstrumentationKey);
        }
    }

    [TestClass]
    public class WhenSettingTraceOptionForAppInsightsTraceListener : GivenAppInsightsListenerInConfigurationSourceBuilder
    {
        private TraceOptions trOption;

        [TestMethod]
        [TestCategory("EntlibTraceListener")]
        public void ThenConfigurationReflectsTraceOption()
        {
            Assert.AreEqual(this.trOption, this.GetAppInsightsTraceListenerData().TraceOutputOptions);
        }

        protected override void Act()
        {
            this.trOption = TraceOptions.Callstack | TraceOptions.DateTime;
            this.appInsightsListenerBuilder.WithTraceOptions(this.trOption);
        }
    }

    [TestClass]
    public class WhenSettingFilterForAppInsightsTraceListener : GivenAppInsightsListenerInConfigurationSourceBuilder
    {              
        [TestMethod]
        [TestCategory("EntlibTraceListener")]
        public void ThenConfigurationReflectsTraceOption()
        {
            Assert.AreEqual(SourceLevels.Error, GetAppInsightsTraceListenerData().Filter);
        }

        protected override void Act()
        {
            appInsightsListenerBuilder.Filter(SourceLevels.Error);
        }
    }

    [TestClass]
    public class When_SettingInstrumentationKeyForAppInsightsTraceListener : GivenAppInsightsListenerInConfigurationSourceBuilder
    {
        [TestMethod]
        public void ThenConfigurationHasInstrumentationKey()
        {
            Assert.AreEqual(ConfigurationTestHelper.InstrumentaionKey, GetAppInsightsTraceListenerData().InstrumentationKey);
        }

        protected override void Act()
        {
            this.appInsightsListenerBuilder.WithInstrumentationKey(ConfigurationTestHelper.InstrumentaionKey);
        }
    }
    
    [TestClass]
    public class WhenSettingInstrumentationKeyForAppInsightsTraceListenerToNull : GivenAppInsightsListenerInConfigurationSourceBuilder
    {
        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        [TestCategory("EntlibTraceListener")]
        public void Then_WithInstrumentationKeyForAppInsights_ThrowsArgumentException()
        {
            appInsightsListenerBuilder.WithInstrumentationKey(null);
        }
    }

    [TestClass]
    public class WhenCallingFormatWithOnSendToAppInsightsListenerPassingNull : GivenAppInsightsListenerInConfigurationSourceBuilder
    {
        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        [TestCategory("EntlibTraceListener")]
        public void Then_FormatWith_ThrowsArgumentNullException()
        {
            appInsightsListenerBuilder.FormatWith(null);
        }
    }
}
