//------------------------------------------------------------------------------
// <copyright file='ObjectArrayConverter.cs' company='Microsoft Corporation'>
// ==============================================================================
// Copyright © Microsoft Corporation.  All rights reserved. 2014
// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY
// OF ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT
// LIMITED TO THE IMPLIED WARRANTIES OF MERCHANTABILITY AND
// FITNESS FOR A PARTICULAR PURPOSE.
// ==============================================================================
// </copyright>
// <summary>Serializes object arrays to JSON.</summary>
//------------------------------------------------------------------------------

using Newtonsoft.Json;
using System;

namespace Microsoft.ApplicationInsights.Tracing.Serialization
{
    /// <summary>
    /// Rather than converting object arrays to JSON, this converter converts each individual object to JSON and then wraps serialized object JSON in a string.
    /// </summary>
    internal class ObjectArrayConverter : EnumerableConverter
    {
        private static readonly ObjectArrayConverter instance = new ObjectArrayConverter();

        private ObjectArrayConverter()
        {
        }

        /// <summary>
        /// <see cref="ObjectArrayConverter"/> is a Singleton with no state. This returns the only instance of the class.
        /// </summary>
        public static ObjectArrayConverter Instance
        {
            get { return ObjectArrayConverter.instance; }
        }

        /// <summary>
        /// Determines if this converter to convert the given type.
        /// </summary>
        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(object[]);
        }

        /// <summary>
        /// Reads in JSON written by this converter. Currently not implemented.
        /// </summary>
        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            throw new NotImplementedException("Object Array de-serialization is not implemented.");
        }
    }
}
