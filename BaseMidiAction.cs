using System.Linq;
using RtMidi.Core;
using RtMidi.Core.Devices.Infos;
using Serilog;
using StreamDeckLib;
using StreamDeckLib.Messages;
using StreamDeckMidiPlugin2.Models;
using System.Threading.Tasks;
using System.Reflection;
using System;
using Newtonsoft.Json.Linq;

namespace StreamDeckMidiPlugin2
{
    public class BaseMidiAction<T> : BaseStreamDeckActionWithSettingsModel<T> where T : BaseMidiModel
    {
        public override Task OnWillAppear(StreamDeckEventPayload args)
        {
            Log.Information("OnWillAppear");
            this.SetModelProperties(args);

            // This might go wrong when the order of devices change or devices are added/removed
            // We need to refer to the devices by name instead of index.
            IMidiOutputDeviceInfo inputDeviceInfo =
               MidiDeviceManager.Default.OutputDevices.ElementAt(this.SettingsModel.SelectedDevice);

            if (!MidiDevice.OutputDevices.ContainsKey(this.SettingsModel.SelectedDevice))
            {
                MidiDevice.OutputDevices.Add(
                    this.SettingsModel.SelectedDevice, inputDeviceInfo.CreateDevice());
            }

            if (!MidiDevice.OutputDevices[this.SettingsModel.SelectedDevice].IsOpen)
            {
                MidiDevice.OutputDevices[this.SettingsModel.SelectedDevice].Open();
            }

            return Task.CompletedTask;
        }

        public override Task OnWillDisappear(StreamDeckEventPayload args)
        {
            // Check if the Midi device is initialized and open
            if (MidiDevice.OutputDevices.ContainsKey(this.SettingsModel.SelectedDevice) &&
                MidiDevice.OutputDevices[this.SettingsModel.SelectedDevice].IsOpen)
            {
                MidiDevice.OutputDevices[this.SettingsModel.SelectedDevice].Close();
            }
            return base.OnWillDisappear(args);
        }

        public override async Task OnPropertyInspectorDidAppear(StreamDeckEventPayload args)
        {
            Log.Information("OnPropertyInspectorDidAppear");
            await this.Manager.SetSettingsAsync(args.context, this.SettingsModel);
            await base.OnPropertyInspectorDidAppear(args);
        }

        // These methods are overriding the default methods because arrays are not supported 
        // in model yet.
        private new void SetModelProperties(StreamDeckEventPayload args)
        {
            PropertyInfo[] properties = typeof(T).GetProperties();
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
