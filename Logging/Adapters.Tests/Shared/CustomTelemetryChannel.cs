//-----------------------------------------------------------------------------------
// <copyright file='CustomTelemetryChannel.cs' company='Microsoft Corporation'>
// ==============================================================================
// Copyright © Microsoft Corporation.  All rights reserved. 2014
// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY
// OF ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT
// LIMITED TO THE IMPLIED WARRANTIES OF MERCHANTABILITY AND
// FITNESS FOR A PARTICULAR PURPOSE.
// ==============================================================================
// </copyright>
//-----------------------------------------------------------------------------------

namespace Microsoft.ApplicationInsights
{
    using System.Collections.Generic;
    using Microsoft.ApplicationInsights.Channel;
    using Microsoft.ApplicationInsights.DataContracts;

    internal class CustomTelemetryChannel :
        ITelemetryChannel
    {
        public CustomTelemetryChannel()
        {
            this.SentItems = new ITelemetry[0];
        }

        public string EndpointAddress { get; set; }

        public ITelemetry[] SentItems { get; private set; }

        public void Send(ITelemetry item)
        {
            lock (this)
            {
                ITelemetry[] current = this.SentItems;
                List<ITelemetry> temp = new List<ITelemetry>(current);
                temp.Add(item);
                this.SentItems = temp.ToArray();
            }
        }

        public void Send(IEnumerable<ITelemetry> items)
        {
            lock (this)
            {
                ITelemetry[] current = this.SentItems;
                List<ITelemetry> temp = new List<ITelemetry>(current);
                temp.AddRange(items);
                this.SentItems = temp.ToArray();
            }
        }

        public void Dispose()
        {
        }

        public bool DeveloperMode
        {
            get; 
            set; 
        }
    }
}
