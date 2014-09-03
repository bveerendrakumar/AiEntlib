//------------------------------------------------------------------------------
// <copyright file='FormatProviderConverter.cs' company='Microsoft Corporation'>
// ==============================================================================
// Copyright © Microsoft Corporation.  All rights reserved. 2014
// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY
// OF ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT
// LIMITED TO THE IMPLIED WARRANTIES OF MERCHANTABILITY AND
// FITNESS FOR A PARTICULAR PURPOSE.
// ==============================================================================
// </copyright>
// <summary>Serializes IFormatProvider to JSON.</summary>
//------------------------------------------------------------------------------

using Newtonsoft.Json;
using System;

namespace Microsoft.ApplicationInsights.Tracing.Serialization
{
    /// <summary>
    /// Converts IFormatProvider to JSON. Stringifies all complex types of IFormatProvider to avoid type conflicts with Elastic search.
    /// </summary>
    internal class FormatProviderConverter : JsonConverter
    {
        private static readonly FormatProviderConverter instance = new FormatProviderConverter();

        private FormatProviderConverter()
        {
        }

        /// <summary>
        /// <see cref="FormatProviderConverter"/> is a Singleton with no state. This returns the only instance of the class.
        /// </summary>
        public static FormatProviderConverter Instance
        {
            get { return FormatProviderConverter.instance; }
        }

        /// <summary>
        /// Determines if this converter to convert the given type.
        /// </summary>
        public override bool CanConvert(Type objectType)
        {
            return typeof(IFormatProvider).IsAssignableFrom(objectType);
        }

        /// <summary>
        /// Reads in JSON written by this converter. Currently not implemented.
        /// </summary>
        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            throw new NotImplementedException("Exception de-serialization is not implemented.");
        }

        /// <summary>
        /// Writes JSON for objects of type IFormatProvider this converter supports.
        /// </summary>
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            if (writer == null)
            {
                throw new ArgumentNullException("writer");
            }
            if (serializer == null)
            {
                throw new ArgumentNullException("serializer");
            }

            if (value != null)
            {
                serializer.Serialize(writer, value.ToString());
            }
            else
            {
                writer.WriteValue(string.Empty);
            }
        }
    }
}