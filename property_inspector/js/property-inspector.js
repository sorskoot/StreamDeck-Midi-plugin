// global websocket, used to communicate from/to Stream Deck software
// as well as some info about our plugin, as sent by Stream Deck software 
var websocket = null,
    uuid = null,
    inInfo = null,
    actionInfo = {},
    currentAction,
    settingsModel =
    {
        Channel: 1,
        Note: 57,
        Value:0,
        Control:0,
        SelectedDevice: "0",
        Devices: []
    };

function connectElgatoStreamDeckSocket(inPort, inUUID, inRegisterEvent, inInfo, inActionInfo) {
    uuid = inUUID;
    actionInfo = JSON.parse(inActionInfo);

    inInfo = JSON.parse(inInfo);
    websocket = new WebSocket('ws://localhost:' + inPort);
    if (actionInfo.payload.settings.settingsModel) {
        settingsModel.Devices = actionInfo.payload.settings.settingsModel.Devices;
        settingsModel.SelectedDevice = actionInfo.payload.settings.settingsModel.SelectedDevice;
        settingsModel.Channel = actionInfo.payload.settings.settingsModel.Channel;            
    }
    if (actionInfo.action === "com.sorskoot.midi.action") {
        //initialize values
        if (actionInfo.payload.settings.settingsModel) {
            settingsModel.Note = actionInfo.payload.settings.settingsModel.Note;
            currentAction = "note";
        }
    } else if (actionInfo.action === "com.sorskoot.midicc.action") {
        if (actionInfo.payload.settings.settingsModel) {
            settingsModel.Value = actionInfo.payload.settings.settingsModel.Value;
            settingsModel.Control = actionInfo.payload.settings.settingsModel.Control;
            currentAction = "cc";
        }
    }

    

    updateUI();

    websocket.onopen = function () {
        var json = { event: inRegisterEvent, uuid: inUUID };
        // register property inspector to Stream Deck
        websocket.send(JSON.stringify(json));

        requestGlobalSettings(inUUID);
    };

    websocket.onmessage = function (evt) {
        // Received message from Stream Deck
        var jsonObj = JSON.parse(evt.data);
        var sdEvent = jsonObj['event'];
        switch (sdEvent) {
            case "didReceiveSettings":
                if (jsonObj.payload.settings.settingsModel.Note) {
                    settingsModel.Note = jsonObj.payload.settings.settingsModel.Note;
                }
                if (jsonObj.payload.settings.settingsModel.Control) {
                    settingsModel.Control = jsonObj.payload.settings.settingsModel.Control;
                }
                if (jsonObj.payload.settings.settingsModel.Value) {
                    settingsModel.Value = jsonObj.payload.settings.settingsModel.Value;
                }
                if (jsonObj.payload.settings.settingsModel.Channel) {
                    settingsModel.Channel = jsonObj.payload.settings.settingsModel.Channel;
                }
                if (jsonObj.payload.settings.settingsModel.Devices) {
                    settingsModel.Devices = jsonObj.payload.settings.settingsModel.Devices;
                }
                if (jsonObj.payload.settings.settingsModel.SelectedDevice) {
                    settingsModel.SelectedDevice = jsonObj.payload.settings.settingsModel.SelectedDevice;
                }

                updateUI();
                break;
            default:
                break;
        }
    };
}

const setSettings = (value, param) => {
    if (websocket) {
        settingsModel[param] = value;
        let settings;
        if (currentAction === "cc") {
            settings = {
                Channel: settingsModel.Channel,
                Value: settingsModel.Value,
                Control: settingsModel.Control,
                SelectedDevice: settingsModel.SelectedDevice,
                Devices: settingsModel.Devices
            };
        } else if (currentAction === "note") {
            settings = {
                Channel: settingsModel.Channel,
                Note: settingsModel.Note,
                SelectedDevice: settingsModel.SelectedDevice, 
                Devices: settingsModel.Devices
            };
        }
        var json = {
            "event": "setSettings",
            "context": uuid,
            "payload": {
                "settingsModel": settings
            }
        };
        websocket.send(JSON.stringify(json));
    }
};

function updateUI() {
    if (currentAction === "note") {
        let x = ['<div class="sdpi-item" id="note_value">',
            '<div class="sdpi-item-label"> Midi Note</div>',
            '<input id="txtNote" class="sdpi-item-value" type="number" inputmode="numeric" pattern="[0-9]*"',
            'placeholder="Enter midi note" onchange="setSettings(event.target.value, \'Note\')" />',
            '</div>'].join('');

        document.getElementById('placeholder').innerHTML = x;
        document.getElementById('txtNote').value = settingsModel.Note;

    } else if (currentAction === "cc") {
        let x=[
        '<div class="sdpi-item" id="control_value">',
        '    <div class="sdpi-item-label">Control</div>',
        '    <input id="txtControl" class="sdpi-item-value" type="number" inputmode="numeric" pattern="[0-9]*"',
        '        placeholder="Enter control value" onchange="setSettings(event.target.value, \'Control\')" />',
        '</div>',
        '<div class="sdpi-item" id="value_value">',
        '    <div class="sdpi-item-label">Value</div>',
        '    <input id="txtValue" class="sdpi-item-value" type="number" inputmode="numeric" pattern="[0-9]*"',
        '        placeholder="Enter value" onchange="setSettings(event.target.value, \'Value\')" />',
            '</div>'].join('');
        document.getElementById('placeholder').innerHTML = x;
        document.getElementById('txtControl').value = settingsModel.Control;
        document.getElementById('txtValue').value = settingsModel.Value;
    }

    document.getElementById('txtChannel').value = settingsModel.Channel;
    let newSelect = document.getElementById('selDevices');
    newSelect.innerHTML = '';
    for (let i = 0; i < settingsModel.Devices.length; i++) {
        const element = settingsModel.Devices[i];
        var opt = document.createElement("option");
        opt.value = i;
        opt.innerHTML = element;
        newSelect.appendChild(opt);
        console.log(element);
    }
    newSelect.value = settingsModel.SelectedDevice;
}

function requestGlobalSettings(inUUID) {
    if (websocket) {
        var json = {
            "event": "getGlobalSettings",
            "context": inUUID
        };

        websocket.send(JSON.stringify(json));
    }
}
