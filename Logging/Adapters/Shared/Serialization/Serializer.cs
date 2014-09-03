//------------------------------------------------------------------------------
// <copyright file='Serializer.cs' company='Microsoft Corporation'>
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
using System.Linq;

namespace Microsoft.ApplicationInsights.Tracing.Serialization
{
    internal class Serializer
    {
        private static readonly JsonConverter[] converters =
            new JsonConverter[] 
            { 
                ObjectArrayConverter.Instance,
                StackConverter.Instance,
                ExceptionConverter.Instance,
                FormatProviderConverter.Instance,
            };

        private static readonly JsonSerializerSettings defaultSettings = Serializer.CreateSettings(Serializer.converters);

        internal static string SerializeData(object data, params JsonConverter[] dataConverters)
        {
            if (data == null)
            {
                throw new ArgumentNullException("data");
            }

            JsonSerializerSettings settingsToUse = null;
            if (dataConverters != null && dataConverters.Length > 0)
            {
                JsonConverter[] convertersToUse = Serializer.converters.Concat(dataConverters).ToArray();

                // I have to create this every time because JSON.NET does not allow me to specify 
                // both an settings object and a collection of converters in the SerializeObject API.
                settingsToUse = Serializer.CreateSettings(convertersToUse);
            }
            else
            {
                settingsToUse = Serializer.defaultSettings;
            }

            string serializedData = JsonConvert.SerializeObject(data, settingsToUse);
            return serializedData;
        }

        internal static TType DeserializeData<TType>(string data, params JsonConverter[] dataConverters)
        {
            if (data == null)
            {
                throw new ArgumentNullException("data");
            }

            JsonSerializerSettings settingsToUse = null;
            if (dataConverters != null && dataConverters.Length > 0)
            {
                JsonConverter[] convertersToUse = Serializer.converters.Concat(dataConverters).ToArray();

                // I have to create this every time because JSON.NET does not allow me to specify 
                // both an settings object and a collection of converters in the SerializeObject API.
                settingsToUse = Serializer.CreateSettings(convertersToUse);
            }
            else
            {
                settingsToUse = Serializer.defaultSettings;
            }

            TType deserializedData = JsonConvert.DeserializeObject<TType>(data, settingsToUse);
            return deserializedData;
        }

        private static JsonSerializerSettings CreateSettings(params JsonConverter[] dataConverters)
        {
            JsonSerializerSettings settings = new JsonSerializerSettings()
            {
                // The first time an object is encountered it will be serialized as usual but if the object is encountered as a child object of itself the serializer will skip serializing it.
                // see http://james.newtonking.com/json/help/index.html?topic=html/SerializationSettings.htm for more info.
                // needed because some adapters have self referential objects.
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
                Converters = dataConverters,
                ContractResolver = ApplicationInsightsContractResolver.Instance,
            };
            return settings;
        }
    }
}
