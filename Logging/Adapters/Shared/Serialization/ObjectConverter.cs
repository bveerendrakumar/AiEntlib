//------------------------------------------------------------------------------
// <copyright file='ObjectConverter.cs' company='Microsoft Corporation'>
// ==============================================================================
// Copyright © Microsoft Corporation.  All rights reserved. 2014
// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY
// OF ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT
// LIMITED TO THE IMPLIED WARRANTIES OF MERCHANTABILITY AND
// FITNESS FOR A PARTICULAR PURPOSE.
// ==============================================================================
// </copyright>
// <summary>Serializes objects to JSON.</summary>
//------------------------------------------------------------------------------

using Newtonsoft.Json;
using System;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.IO;

namespace Microsoft.ApplicationInsights.Tracing.Serialization
{
    /// <summary>
    /// Rather than converting objects to JSON, this converter converts the object to JSON and then wraps serialized object JSON in a string.
    /// </summary>
    internal sealed class ObjectConverter : JsonConverter
    {
        private static readonly ObjectConverter instance = new ObjectConverter();

        private ObjectConverter()
        {
        }

        /// <summary>
        /// <see cref="ObjectConverter"/> is a Singleton with no state. This returns the only instance of the class.
        /// </summary>
        public static ObjectConverter Instance
        {
            get { return ObjectConverter.instance; }
        }

        /// <summary>
        /// Determines if this converter to convert the given type.
        /// </summary>
        public override bool CanConvert(Type objectType)
        {
            return true;
        }

        /// <summary>
        /// Reads in JSON written by this converter. Currently not implemented.
        /// </summary>
        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            throw new NotImplementedException("Object de-serialization is not implemented.");
        }

        /// <summary>
        /// Writes JSON for the types of object types this converter supports.
        /// </summary>
        [SuppressMessage("Microsoft.Usage", "CA2202:Do not dispose objects multiple times", Justification = "Benign for a StringWriter")]
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

            // Enclosing property is already being stringified, so don't stringify this property as it will create unnecessary nested strings.
            if (writer is StringifiedJsonTextWriter)
            {
                serializer.Serialize(writer, value);
            }
            else
            {
                if (value == null)
                {
                    writer.WriteValue(string.Empty);
                }
                else if (ObjectConverter.IsPrimitive(value))
                {
                    writer.WriteValue(value.ToString());
                }
                else
                {
                    // Stringify property using a StringifiedJsonTextWriter so nested properties know they have already been stringified.
                    using (StringWriter stringWriter = new StringWriter(CultureInfo.CurrentCulture))
                    using (JsonTextWriter customWriter = new StringifiedJsonTextWriter(stringWriter))
                    {
                        serializer.Serialize(customWriter, value);
                        writer.WriteValue(stringWriter.ToString());
                    }
                }
            }
        }

        [SuppressMessage("Microsoft.StyleCop.CSharp.ReadabilityRules", "SA1121:UseBuiltInTypeAlias", Justification = "Want to be explicit about the type")]
        private static bool IsPrimitive(object value)
        {
            return value is Byte ||
                   value is SByte ||
                   value is Int16 ||
                   value is Int32 ||
                   value is Int64 ||
                   value is UInt16 ||
                   value is UInt32 ||
                   value is UInt64 ||
                   value is Decimal ||
                   value is Double ||
                   value is Single ||
                   value is String ||
                   value is Enum;
        }
    }
}
