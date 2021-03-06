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
    /// Offers functionality for counter periphery. Instances can be retrieved from
    /// Counters of the respective Device instance.
    /// </summary>
    public sealed class Counter : IInput
    {
        /// <summary>
        /// The possible counter configuration values.
        /// </summary>
        public enum CounterConfig : uint
        {
            RisingEdge = SystemConnector.CeboMsrCounterConfigRisingEdge,
            FallingEdge = SystemConnector.CeboMsrCounterConfigFallingEdge,
            Alternating = SystemConnector.CeboMsrCounterConfigAlternating
        }

        private Device _device;
        private uint _interfaceId;
        private int _index;
        private string _name;

        internal uint InterfaceId { get { return _interfaceId; } }
        internal int Index { get { return _index; } }

        internal Counter(Device device, uint interfaceId, int index)
        {
            _device = device;
            _interfaceId = interfaceId;
            _index = index;
            _name = SystemConnector.GetPeripheralName(device.Handle, interfaceId);
        }

        /// <summary>
        /// Reset counter to 0.
        /// </summary>
        public void Reset()
        {
            SystemConnector.ResetCounter(_device.Handle, _interfaceId);
        }

        /// <summary>
        /// Define or retrieve the counters enabled state.
        /// </summary>
        public bool Enabled
        {
            get { return SystemConnector.GetCounterEnable(_device.Handle, _interfaceId); }
            set { SystemConnector.SetCounterEnable(_device.Handle, _interfaceId, value); }
        }

        /// <summary>
        /// Define and request behavior of the counter.
        /// </summary>
        public CounterConfig Config
        {
            get { return (CounterConfig)SystemConnector.GetCounterConfig(_device.Handle, _interfaceId); }
            set { SystemConnector.SetCounterConfig(_device.Handle, _interfaceId, (uint)value); }
        }

        /// <summary>
        /// Current counter value.
        /// </summary>
        public long Value
        {
            get { return SystemConnector.ReadCounter(_device.Handle, _interfaceId); }
        }

        /// <summary>
        /// Name of the component.
        /// </summary>
        public string Name { get { return _name; } }

        public override string ToString()
        {
            return "Counter" + _index;
        }
    }
}
