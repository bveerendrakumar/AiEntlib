//------------------------------------------------------------------------------
// <copyright file='ExceptionConverter.cs' company='Microsoft Corporation'>
// ==============================================================================
// Copyright © Microsoft Corporation.  All rights reserved. 2014
// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY
// OF ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT
// LIMITED TO THE IMPLIED WARRANTIES OF MERCHANTABILITY AND
// FITNESS FOR A PARTICULAR PURPOSE.
// ==============================================================================
// </copyright>
// <summary>Serializes exceptions to JSON.</summary>
//------------------------------------------------------------------------------

using Newtonsoft.Json;
using System;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Linq;
using System.Runtime.Serialization;

namespace Microsoft.ApplicationInsights.Tracing.Serialization
{
    /// <summary>
    /// Converts exceptions to JSON. Stringifies all complex types to avoid type conflicts with Elastic search.
    /// </summary>
    internal class ExceptionConverter : JsonConverter
    {
        private static readonly ExceptionConverter instance = new ExceptionConverter();

        private ExceptionConverter()
        {
        }

        /// <summary>
        /// <see cref="ExceptionConverter"/> is a Singleton with no state. This returns the only instance of the class.
        /// </summary>
        public static ExceptionConverter Instance
        {
            get { return ExceptionConverter.instance; }
        }

        /// <summary>
        /// Determines if this converter to convert the given type.
        /// </summary>
        public override bool CanConvert(Type objectType)
        {
            return typeof(Exception).IsAssignableFrom(objectType);
        }

        /// <summary>
        /// Reads in JSON written by this converter. Currently not implemented.
        /// </summary>
        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            throw new NotImplementedException("Exception de-serialization is not implemented.");
        }

        /// <summary>
        /// Writes JSON for the types of object types this converter supports.
        /// </summary>
        [SuppressMessage("Microsoft.Naming", "CA2204:Literals should be spelled correctly", MessageId = "ISerializable", Justification = "ISerializable is the name of a type I'm referring to")]
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

            ISerializable serializableException = value as ISerializable;
            if (serializableException == null)
            {
                throw new InvalidOperationException(string.Format(CultureInfo.CurrentCulture, "Expected an ISerializable type, got {0} instead.", value.GetType()));
            }
            else
            {
                writer.WriteStartObject();
                SerializationInfo serializationInfo = new SerializationInfo(value.GetType(), new FormatterConverter());
                serializableException.GetObjectData(serializationInfo, serializer.Context);
                SerializationInfoEnumerator enumerator = serializationInfo.GetEnumerator();
                while (enumerator.MoveNext())
                {
                    SerializationEntry current = enumerator.Current;
                    writer.WritePropertyName(current.Name);
                    if (ExceptionConverter.IsPrimitive(current.ObjectType)
                        || serializer.Converters.Where(c => c.GetType() != typeof(ExceptionConverter))
                                                .Any(c => c.CanConvert(current.ObjectType)))
                    {
                        // Do not stringify primitives
                        serializer.Serialize(writer, current.Value);
                    }
                    else
                    {
                        // Stringify all complex types.
                        ObjectConverter.Instance.WriteJson(writer, current.Value, serializer);
                    }
                }
                writer.WriteEndObject();
            }
        }

        [SuppressMessage("Microsoft.StyleCop.CSharp.ReadabilityRules", "SA1121:UseBuiltInTypeAlias", Justification = "Want to be explicit about the type")]
        private static bool IsPrimitive(Type serializedType)
        {
            return serializedType == typeof(Byte) ||
                   serializedType == typeof(SByte) ||
                   serializedType == typeof(Int16) ||
                   serializedType == typeof(Int32) ||
                   serializedType == typeof(Int64) ||
                   serializedType == typeof(UInt16) ||
                   serializedType == typeof(UInt32) ||
                   serializedType == typeof(UInt64) ||
                   serializedType == typeof(Decimal) ||
                   serializedType == typeof(Single) ||
                   serializedType == typeof(Double) ||
                   serializedType == typeof(String) ||
                   typeof(Enum).IsAssignableFrom(serializedType);
        }
    }
}
