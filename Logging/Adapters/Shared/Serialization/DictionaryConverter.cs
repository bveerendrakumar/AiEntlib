// -----------------------------------------------------------------------
// <copyright file="DictionaryConverter.cs" company="Microsoft">
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
    /// Converts object of type Dictionary to JSON to avoid type conflicts with Elastic search.
    /// </summary>
    internal class DictionaryConverter : JsonConverter
    {
        private static readonly DictionaryConverter instance = new DictionaryConverter();

        private DictionaryConverter()
        {
        }

        /// <summary>
        /// <see cref="DictionaryConverter"/> is a Singleton with no state. This returns the only instance of the class.
        /// </summary>
        public static DictionaryConverter Instance
        {
            get { return DictionaryConverter.instance; }
        }

        /// <summary>
        /// Determines if this converter to convert the given type.
        /// </summary>
        public override bool CanConvert(Type objectType)
        {
            return typeof(IDictionary).IsAssignableFrom(objectType);
        }

        /// <summary>
        /// Reads in JSON written by this converter. Currently not implemented.
        /// </summary>
        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            throw new NotImplementedException("Dictionary de-serialization is not implemented.");
        }

        /// <summary>
        /// Writes JSON for objects of type Dictionary this converter supports.
        /// </summary>
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            if (writer == null)
            {
                throw new ArgumentNullException("writer");
            }

            IDictionaryEnumerator dictionaryEnumerator = ((IDictionary)value).GetEnumerator();

            writer.WriteStartObject();

            while (dictionaryEnumerator.MoveNext())
            {
                writer.WritePropertyName(dictionaryEnumerator.Key.ToString());
                ObjectConverter.Instance.WriteJson(writer, dictionaryEnumerator.Value, serializer);
            }

            writer.WriteEndObject();
        }
    }
}
