// -----------------------------------------------------------------------
// <copyright file="StackConverter.cs" company="Microsoft">
// ==============================================================================
// Copyright © Microsoft Corporation.  All rights reserved. 2014
// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY
// OF ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT
// LIMITED TO THE IMPLIED WARRANTIES OF MERCHANTABILITY AND
// FITNESS FOR A PARTICULAR PURPOSE.
// ==============================================================================
// </copyright>
// -----------------------------------------------------------------------

using Newtonsoft.Json;
using System;
using System.Collections;

namespace Microsoft.ApplicationInsights.Tracing.Serialization
{
    /// <summary>
    /// <see cref="JsonConverter"/> that wraps <see cref="Stack"/> contents in strings.
    /// </summary>
    internal class StackConverter : EnumerableConverter
    {
        private static readonly StackConverter instance = new StackConverter();

        private StackConverter()
        {
        }

        /// <summary>
        /// <see cref="StackConverter"/> is a Singleton with no state. This returns the only instance of the class.
        /// </summary>
        public static StackConverter Instance
        {
            get { return StackConverter.instance; }
        }

        /// <summary>
        /// Determines if this converter to convert the given type.
        /// </summary>
        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(Stack);
        }

        /// <summary>
        /// Reads in JSON written by this converter. Currently not implemented.
        /// </summary>
        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            throw new NotImplementedException("Stack de-serialization is not implemented.");
        }
    }
}
