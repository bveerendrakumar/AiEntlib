// <copyright file="ConfigurationTestHelper.cs" company="Microsoft">
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
using System.IO;
using Microsoft.Practices.EnterpriseLibrary.Common.Configuration;
using Microsoft.Practices.EnterpriseLibrary.Logging.Configuration;

[module: System.Diagnostics.CodeAnalysis.SuppressMessage("StyleCop.CSharp.MaintainabilityRules", "SA1401:FieldsMustBePrivate", Justification = "Reviewed. Suppression is OK here.")]
[module: System.Diagnostics.CodeAnalysis.SuppressMessage("StyleCop.CSharp.MaintainabilityRules", "SA1402:FileMayOnlyContainASingleClass", Justification = "Reviewed. Suppression is OK here.")]

namespace Microsoft.ApplicationInsights.EntlibTraceListener.Tests.Configuration
{
    public static class ConfigurationTestHelper
    {
        public static string ListenerName = "AppInsights TraceListener";
        public static string InstrumentaionKey = "93d9c2b7-e633-4571-8520-d391511a1df5";
        public static string ConfigurationFileName = "test.exe.config";
        private static LoggingSettings settings = null;
        private static DictionaryConfigurationSource configurationSource = null;

        public static DictionaryConfigurationSource AppConfigurationLoggSource
        {
            get
            {
                if (settings == null)
                {
                    settings = (LoggingSettings)ConfigurationManager.GetSection(LoggingSettings.SectionName);
                }
                if (configurationSource == null)
                {
                    configurationSource = new DictionaryConfigurationSource();
                    configurationSource.Add(LoggingSettings.SectionName, settings);
                }
                return configurationSource;
            }
        }

        public static string ConditionalListenerName
        {
            get
            {
                return GetConditionalListenerName();
            }
        }

        public static IConfigurationSource SaveSectionsAndReturnConfigurationSource(IDictionary<string, ConfigurationSection> sections)
        {
            System.Configuration.Configuration configuration
                = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);

            SaveSections(configuration, sections);

            return new SystemConfigurationSource(false);
        }

        public static IConfigurationSource SaveSectionsInFileAndReturnConfigurationSource(IDictionary<string, ConfigurationSection> sections)
        {
            System.Configuration.Configuration configuration
                = GetConfigurationForCustomFile(ConfigurationFileName);

            SaveSections(configuration, sections);

            return GetConfigurationSourceForCustomFile(ConfigurationFileName);
        }

        public static IConfigurationSource GetConfigurationSourceForCustomFile(string fileName)
        {
            return new FileConfigurationSource(fileName, false);
        }

        public static System.Configuration.Configuration GetConfigurationForCustomFile(string fileName)
        {
            var fileMap = new ExeConfigurationFileMap { ExeConfigFilename = fileName };
            File.SetAttributes(fileMap.ExeConfigFilename, FileAttributes.Normal);
            return ConfigurationManager.OpenMappedExeConfiguration(fileMap, ConfigurationUserLevel.None);
        }

        public static IConfigurationSource SaveSectionsAndGetConfigurationSource(LoggingSettings loggingSettings)
        {
            var fileMap = new ExeConfigurationFileMap
            {
                ExeConfigFilename = ConfigurationFileName
            };
            System.Configuration.Configuration rwConfiguration =
                ConfigurationManager.OpenMappedExeConfiguration(fileMap, ConfigurationUserLevel.None);
            rwConfiguration.Sections.Remove(LoggingSettings.SectionName);
            rwConfiguration.Sections.Add(LoggingSettings.SectionName, loggingSettings);

            File.SetAttributes(fileMap.ExeConfigFilename, FileAttributes.Normal);
            rwConfiguration.Save();
            ConfigurationManager.OpenMappedExeConfiguration(fileMap, ConfigurationUserLevel.None);

            return new FileConfigurationSource(fileMap.ExeConfigFilename, false);
        }

        private static void SaveSections(System.Configuration.Configuration configuration, IDictionary<string, ConfigurationSection> sections)
        {
            foreach (string sectionName in sections.Keys)
            {
                configuration.Sections.Remove(sectionName);
                configuration.Sections.Add(sectionName, sections[sectionName]);
            }

            configuration.Save();
        }

        private static string GetConditionalListenerName()
        {
#if NET40
            return ListenerName + "\u200cimplementation";
#else
            return ListenerName;
#endif
        }
    }

    public class MockLogObjectsHelper : IDisposable
    {
        public MockLogObjectsHelper()
        {
            this.LoggingSettings = new LoggingSettings();
            this.ConfigurationSource = new DictionaryConfigurationSource();
            this.ConfigurationSource.Add(LoggingSettings.SectionName, LoggingSettings);
        }

        public DictionaryConfigurationSource ConfigurationSource
        {
            get;
            set;
        }

        public LoggingSettings LoggingSettings
        {
            get;
            set;
        }

        #region Implementation for IDisposable

        /// <summary>
        /// Releases all resources used by <see cref="Tracer"/>.
        /// </summary>
        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Releases unmanaged and - optionally - managed resources.
        /// </summary>
        /// <param name="dispossing"><c>true</c> to release both managed and unmanaged resources; 
        /// <c>false</c> to release only unmanaged resources.</param>
        private void Dispose(bool dispossing)
        {
            if (dispossing)
            {
                if (this.ConfigurationSource != null)
                {
                   ((IConfigurationSource)this.ConfigurationSource).Dispose();
                }
            }
        }

        #endregion 
    }
}
