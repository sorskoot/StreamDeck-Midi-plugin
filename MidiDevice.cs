﻿using System.Collections.Generic;
using RtMidi.Core.Devices;

namespace StreamDeckMidiPlugin2
{
    static class MidiDevice
    {
        public static Dictionary<string, IMidiOutputDevice> OutputDevices = new Dictionary<string, IMidiOutputDevice>();
    }
}
