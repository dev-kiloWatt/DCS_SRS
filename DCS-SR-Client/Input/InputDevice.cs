﻿using System;
using Ciribob.DCS.SimpleRadio.Standalone.Client.Settings;

namespace Ciribob.DCS.SimpleRadio.Standalone.Client.Input
{
    public class InputDevice
    {
        public InputBinding InputBind { get; set; }

        public string DeviceName { get; set; }

        public int Button { get; set; }
        public Guid InstanceGuid { get; internal set; }
        public int ButtonValue { get; internal set; }

        public bool IsSameBind(InputDevice compare)
        {
            return Button == compare.Button &&
                   compare.InstanceGuid == InstanceGuid &&
                   ButtonValue == compare.ButtonValue;
        }
    }
}