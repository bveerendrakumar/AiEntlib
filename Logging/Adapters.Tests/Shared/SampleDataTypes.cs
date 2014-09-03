//-----------------------------------------------------------------------------------
// <copyright file='SampleDataTypes.cs' company='Microsoft Corporation'>
// ==============================================================================
// Copyright © Microsoft Corporation.  All rights reserved. 2014
// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY
// OF ANY KIND, EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT
// LIMITED TO THE IMPLIED WARRANTIES OF MERCHANTABILITY AND
// FITNESS FOR A PARTICULAR PURPOSE.
// ==============================================================================
// </copyright>
//-----------------------------------------------------------------------------------

using System;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.Serialization;

namespace Microsoft.ApplicationInsights.Tracing.Tests
{
    #region Dummy types used for testing

    [Serializable]
    [SuppressMessage("Microsoft.StyleCop.CSharp.MaintainabilityRules", "SA1402:FileMayOnlyContainASingleClass", Justification = "Keep all sample data together.")]
    public enum SampleEnum1
    {
        On,
        Off,
    }

    [Serializable]
    [SuppressMessage("Microsoft.StyleCop.CSharp.MaintainabilityRules", "SA1402:FileMayOnlyContainASingleClass", Justification = "Keep all sample data together.")]
    public struct SampleDataStruct1
    {
        private int magicNumber;
        private SampleDataClass2 data2;

        public SampleDataStruct1(int magicNumber, SampleDataClass2 data2)
        {
            this.magicNumber = magicNumber;
            this.data2 = data2;
        }

        public int MagicNumber
        {
            get { return this.magicNumber; }
        }

        public SampleDataClass2 SampleData2
        {
            get { return this.data2; }
        }
    }

    [Serializable]
    [SuppressMessage("Microsoft.StyleCop.CSharp.MaintainabilityRules", "SA1402:FileMayOnlyContainASingleClass", Justification = "Keep all sample data together.")]
    public class SampleDataClass1 : ISerializable
    {
        private string name;
        private SampleDataStruct1 dataStruct;

        public SampleDataClass1()
        {
            this.name = string.Empty;
            this.dataStruct = new SampleDataStruct1();
        }

        public SampleDataClass1(string name, SampleDataStruct1 dataStruct)
        {
            this.name = name;
            this.dataStruct = dataStruct;
        }

        protected SampleDataClass1(SerializationInfo information, StreamingContext context)
        {
            this.name = (string)information.GetValue("name", typeof(string));
            this.dataStruct = (SampleDataStruct1)information.GetValue("dataStruct", typeof(SampleDataStruct1));
        }

        [SuppressMessage("Microsoft.Design", "CA1024:UsePropertiesWhereAppropriate", Justification = "Turning this into a property would affect serialization logic.")]
        public string GetName()
        {
            return this.name;
        }

        public virtual void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("name", this.name);
            info.AddValue("dataStruct", this.dataStruct, typeof(SampleDataStruct1));
        }
    }

    [Serializable]
    [SuppressMessage("Microsoft.StyleCop.CSharp.MaintainabilityRules", "SA1402:FileMayOnlyContainASingleClass", Justification = "Keep all sample data together.")]
    public class SampleDataClass2 : ISerializable, IEquatable<SampleDataClass2>
    {
        private SampleEnum1 state;
        private double[] values;

        public SampleDataClass2()
        {
            this.state = default(SampleEnum1);
            this.values = new double[] { };
        }

        public SampleDataClass2(SampleEnum1 state, double[] values)
        {
            this.state = state;
            this.values = values;
        }

        protected SampleDataClass2(SerializationInfo information, StreamingContext context)
        {
            this.state = (SampleEnum1)information.GetValue("state", typeof(SampleEnum1));
            this.values = (double[])information.GetValue("values", typeof(double[]));
        }

        [SuppressMessage("Microsoft.Design", "CA1024:UsePropertiesWhereAppropriate", Justification = "Turning this into a property would affect serialization logic.")]
        public SampleEnum1 GetState()
        {
            return this.state;
        }

        public double GetValue(int index)
        {
            return this.values[index];
        }

        public virtual void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("state", this.state, typeof(SampleEnum1));
            info.AddValue("values", this.values, typeof(double[]));
        }

        public bool Equals(SampleDataClass2 other)
        {
            if (ReferenceEquals(other, null))
            {
                return false;
            }
            if (other.state != this.state)
            {
                return false;
            }
            if (other.values != this.values)
            {
                return false;
            }
            return true;
        }
    }

    [Serializable]
    [SuppressMessage("Microsoft.StyleCop.CSharp.MaintainabilityRules", "SA1402:FileMayOnlyContainASingleClass", Justification = "Keep all sample data together.")]
    public class SampleNode
    {
        public string NodeName { get; set; }

        public SampleNode NextNode { get; set; }
    }

    #endregion Dummy types used for testing
}
