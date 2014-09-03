//------------------------------------------------------------------------------
// <copyright file='EnumerableConverter.cs' company='Microsoft Corporation'>
// ==============================================================================
// Copyright © Microsoft Corporation.  All rights reserved. 2014
// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY
// OF ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT
// LIMITED TO THE IMPLIED WARRANTIES OF MERCHANTABILITY AND
// FITNESS FOR A PARTICULAR PURPOSE.
// ==============================================================================
// </copyright>
// <summary>Serializes IEnumerable to JSON.</summary>
//------------------------------------------------------------------------------

using Newtonsoft.Json;
using System;
using System.Collections;

namespace Microsoft.ApplicationInsights.Tracing.Serialization
{
    /// <summary>
    /// This converter converts each individual object in the <see cref="IEnumerable"/> to JSON and then wraps serialized object JSON in a string.
    /// </summary>
    internal abstract class EnumerableConverter : JsonConverter
    {
        /// <summary>
        /// Writes JSON for the types of object types this converter supports.
        /// </summary>
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            if (writer == null)
            {
                throw new ArgumentNullException("writer");
            }

            IEnumerable values = (IEnumerable)value;

            writer.WriteStartArray();   // write: [
            foreach (object obj in values)
            {
                // Wrap each value in a string
                ObjectConverter.Instance.WriteJson(writer, obj, serializer);
            }
            writer.WriteEndArray();     // write: ]
        }
    }
}
