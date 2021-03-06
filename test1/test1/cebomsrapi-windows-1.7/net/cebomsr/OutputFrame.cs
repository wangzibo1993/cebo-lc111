﻿/* 
  CESYS Software License - Version 1.0 - January 1st, 2016

  Permission is hereby granted, free of charge, to any person or organization
  obtaining a copy of the software and accompanying documentation covered by
  this license (the "Software") to use, reproduce, display, distribute,
  execute, and transmit the Software, and to prepare derivative works of the
  Software, and to permit third-parties to whom the Software is furnished to
  do so, all subject to the following:

  The software must only be used to operate hardware manufactured by 
  CESYS GmbH, Herzogenaurach.

  The copyright notices in the Software and this entire statement, including
  the above license grant, this restriction and the following disclaimer,
  must be included in all copies of the Software, in whole or in part, and
  all derivative works of the Software, unless such copies or derivative
  works are solely in the form of machine-executable object code generated by
  a source language processor.

  THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
  IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
  FITNESS FOR A PARTICULAR PURPOSE, TITLE AND NON-INFRINGEMENT. IN NO EVENT
  SHALL THE COPYRIGHT HOLDERS OR ANYONE DISTRIBUTING THE SOFTWARE BE LIABLE
  FOR ANY DAMAGES OR OTHER LIABILITY, WHETHER IN CONTRACT, TORT OR OTHERWISE,
  ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER
  DEALINGS IN THE SOFTWARE.
  
  Written by Thomas Hoppe <thomas.hoppe@cesys.com>, 2016
*/  

using System;
using System.Collections.Generic;
using System.Text;

namespace CeboMsrNet
{
    /// <summary>
    /// Class to use when setting multiple outputs simultaneous. Direct construction is prohibited,
    /// as the count of the the various elements varies from device type to device type. Instances
    /// must be generated calling Device.CreateOutputFrame().
    /// </summary>
    public sealed class OutputFrame
    {
        private float[] _vvalues;
        private int[] _values;

        internal float[] VValues { get { return _vvalues; } }
        internal int[] Values { get { return _values; } }

        internal OutputFrame(int vcount, int count)
        {
            _vvalues = new float[vcount];
            _values = new int[count];
        }

        /// <summary>
        /// Define value that should be set on the specified digital port.
        /// </summary>
        /// <param name="index">Zero based index of port to specify.</param>
        /// <param name="value">Value to set at this port.</param>
        public void SetDigitalPort(int index, int value)
        {
            _values[index] = value;
        }

        /// <summary>
        /// Define value that should be set on the specified analog output.
        /// </summary>
        /// <param name="index">Zero based index of output to specify.</param>
        /// <param name="value">Value to set at this output.</param>
        public void SetAnalogOutput(int index, float value)
        {
            _vvalues[index] = value;
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("OutputFrame: \n");
            for (int i = 0; i < _vvalues.Length; ++i)
                sb.Append(string.Format("AnalogOutput #{0} = {1:0.00} V\n", i, _vvalues[i]));
            for (int i = 0; i < _values.Length; ++i)
                sb.Append(string.Format("DigitalPort #{0} = {1} (0x{1:x8})\n", i, _values[i]));
            return sb.ToString();
        }
    }
}
