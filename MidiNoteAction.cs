﻿using Newtonsoft.Json.Linq;
using RtMidi.Core;
using RtMidi.Core.Devices;
using RtMidi.Core.Devices.Infos;
using RtMidi.Core.Enums;
using RtMidi.Core.Messages;
using Serilog;
using StreamDeckLib;
using StreamDeckLib.Messages;
using System;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace StreamDeckMidiPlugin2
{
    [ActionUuid(Uuid = "com.sorskoot.midi.action")]
    public class MidiNoteAction : BaseStreamDeckActionWithSettingsModel<Models.MidiModel>
    {

        public override Task OnWillAppear(StreamDeckEventPayload args)
        {
            Log.Information("OnWillAppear");
            this.SetModelProperties(args);

            IMidiOutputDeviceInfo inputDeviceInfo =
               MidiDeviceManager.Default.OutputDevices.ElementAt(this.SettingsModel.SelectedDevice);

            if (MidiDevice.outputDevice == null)
            {
                MidiDevice.outputDevice = inputDeviceInfo.CreateDevice();
            }

            if (!MidiDevice.outputDevice.IsOpen)
            {
                MidiDevice.outputDevice.Open();
            }

            return Task.CompletedTask;
        }

        public override Task OnWillDisappear(StreamDeckEventPayload args)
        {
            MidiDevice.outputDevice.Close();
            return base.OnWillDisappear(args);
        }

        public override async Task OnPropertyInspectorDidAppear(StreamDeckEventPayload args)
        {
            Log.Information("OnPropertyInspectorDidAppear");
            await this.Manager.SetSettingsAsync(args.context, this.SettingsModel);
            await base.OnPropertyInspectorDidAppear(args);
        }

        public override Task OnKeyDown(StreamDeckEventPayload args)
        {
            Log.Information("OnKeyDown");
            this.MidiOn(this.SettingsModel.Channel, this.SettingsModel.Note);
            return Task.CompletedTask;
        }

        public override Task OnKeyUp(StreamDeckEventPayload args)
        {
            this.MidiOff(this.SettingsModel.Channel, this.SettingsModel.Note);
            return base.OnKeyUp(args);
        }

        public override Task OnDidReceiveSettings(StreamDeckEventPayload args)
        {
            Log.Information("OnDidReceiveSettings");

            MidiDevice.outputDevice.Close();

            return this.OnWillAppear(args);
        }

        public void MidiOn(int channel, int note)
        {
            var midiChannel = (Channel)(channel - 1);
            var midiNote = (Key)note;
            if (MidiDevice.outputDevice.IsOpen)
            {
                MidiDevice.outputDevice.Send(new NoteOnMessage(midiChannel, midiNote, 127));
            }
            else
            {
                Log.Warning("midi output device not connected");
            }
        }

        public void MidiOff(int channel, int note)
        {
            var midiChannel = (Channel)(channel - 1);
            var midiNote = (Key)note;
            if (MidiDevice.outputDevice.IsOpen)
            {
                MidiDevice.outputDevice.Send(new NoteOffMessage(midiChannel, midiNote, 127));
            }
            else
            {
                Log.Warning("midi output device not connected");
            }
        }

        // These methods are overriding the default methods because arrays are not supported 
        // in model yet.
        private new void SetModelProperties(StreamDeckEventPayload args)
        {
            PropertyInfo[] properties = typeof(Models.MidiModel).GetProperties();
            PropertyInfo[] array = properties;
            foreach (PropertyInfo propertyInfo in array)
            {
                if ((args.payload != null && args.payload.settings != null && args.payload.settings.settingsModel != null) && args.PayloadSettingsHasProperty(propertyInfo.Name))
                {
                    object payloadSettingsValue = this.GetPayloadSettingsValue(args, propertyInfo.Name);
                    object value = Convert.ChangeType(payloadSettingsValue, propertyInfo.PropertyType);
                    propertyInfo.SetValue(this.SettingsModel, value);
                }
            }
        }

        private object GetPayloadSettingsValue(StreamDeckEventPayload obj, string propertyName)
        {
            if (obj.PayloadSettingsHasProperty(propertyName))
            {
                if (obj.payload.settings.settingsModel[propertyName].Type != JTokenType.Array)
                {
                    return obj.payload.settings.settingsModel[propertyName].Value;
                }
                else
                {
                    return ((JArray)obj.payload.settings.settingsModel[propertyName]).ToObject(typeof(string[]));
                }
            }
            return null;
        }
    }
}
