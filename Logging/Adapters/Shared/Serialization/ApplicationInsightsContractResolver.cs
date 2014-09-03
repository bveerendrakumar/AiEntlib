//------------------------------------------------------------------------------
// <copyright file='ApplicationInsightsContractResolver.cs' company='Microsoft Corporation'>
// ==============================================================================
// Copyright © Microsoft Corporation.  All rights reserved. 2014
// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY
// OF ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT
// LIMITED TO THE IMPLIED WARRANTIES OF MERCHANTABILITY AND
// FITNESS FOR A PARTICULAR PURPOSE.
// ==============================================================================
// </copyright>
// <summary>JSON.NET contract resolver that serializes objects as strings.</summary>
//------------------------------------------------------------------------------

using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System.Reflection;

namespace Microsoft.ApplicationInsights.Tracing.Serialization
{
    /// <summary>
    /// Custom contract resolver for Application Insights.
    /// Converts any type mutable fields (of type object or collection of object) to strings.
    /// </summary>
    internal sealed class ApplicationInsightsContractResolver : DefaultContractResolver
    {
        private static readonly ApplicationInsightsContractResolver instance = new ApplicationInsightsContractResolver();

        private ApplicationInsightsContractResolver()
        {
        }

        /// <summary>
        /// <see cref="ApplicationInsightsContractResolver"/> is a Singleton with no state. This returns the only instance of the class.
        /// </summary>
        public static ApplicationInsightsContractResolver Instance
        {
            get { return ApplicationInsightsContractResolver.instance; }
        }

        /// <summary>
        /// Creates a <see cref="JsonProperty"/> for the given <see cref="MemberInfo"/>.
        /// In the case of object fields, the property converter will be overridden to use the <see cref="ObjectConverter"/> that converts objects to strings.
        /// </summary>
        protected override JsonProperty CreateProperty(MemberInfo member, MemberSerialization memberSerialization)
        {
            JsonProperty property = base.CreateProperty(member, memberSerialization);

            if (property.PropertyType == typeof(object))
            {
                property.Converter = ObjectConverter.Instance;
            }

            return property;
        }
    }
}
