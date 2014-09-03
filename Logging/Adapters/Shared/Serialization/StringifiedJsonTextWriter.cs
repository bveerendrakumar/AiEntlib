//------------------------------------------------------------------------------
// <copyright file='StringifiedJsonTextWriter.cs' company='Microsoft Corporation'>
// ==============================================================================
// Copyright © Microsoft Corporation.  All rights reserved. 2014
// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY
// OF ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT
// LIMITED TO THE IMPLIED WARRANTIES OF MERCHANTABILITY AND
// FITNESS FOR A PARTICULAR PURPOSE.
// ==============================================================================
// </copyright>
// <summary>Represents content that is stringified.</summary>
//------------------------------------------------------------------------------

using Newtonsoft.Json;
using System.IO;

namespace Microsoft.ApplicationInsights.Tracing.Serialization
{
    /// <summary>
    /// Simply wraps a <see cref="JsonTextWriter"/> type.
    /// This class just exists to let our other converters know that the contents they are converting has already been stringified at a higher level.
    /// </summary>
    internal class StringifiedJsonTextWriter : JsonTextWriter
    {
        /// <summary>
        /// Creates an instance of the StringifiedJsonTextWriter class using the specified <see cref="System.IO.TextWriter"/>.
        /// </summary>
        public StringifiedJsonTextWriter(TextWriter textWriter)
            : base(textWriter)
        {
        }
    }
}
