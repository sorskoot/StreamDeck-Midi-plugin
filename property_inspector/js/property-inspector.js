// global websocket, used to communicate from/to Stream Deck software
// as well as some info about our plugin, as sent by Stream Deck software 
var websocket = null,
    uuid = null,
    inInfo = null,
    actionInfo = {},
    settingsModel =
    {
        Channel: 1,
        Note: 57,
        SelectedDevice: "0",
        Devices:[]
    };

function connectElgatoStreamDeckSocket(inPort, inUUID, inRegisterEvent, inInfo, inActionInfo) {
    uuid = inUUID;
    actionInfo = JSON.parse(inActionInfo);
    console.log(inActionInfo);

    inInfo = JSON.parse(inInfo);
    websocket = new WebSocket('ws://localhost:' + inPort);

    //initialize values
    if (actionInfo.payload.settings.settingsModel) {
        settingsModel.Note = actionInfo.payload.settings.settingsModel.Note;
        settingsModel.Channel = actionInfo.payload.settings.settingsModel.Channel;
        settingsModel.Devices = actionInfo.payload.settings.settingsModel.Devices;
        settingsModel.SelectedDevice = actionInfo.payload.settings.settingsModel.SelectedDevice;
    }

    updateUI();

    websocket.onopen = function () {
        var json = { event: inRegisterEvent, uuid: inUUID };
        // register property inspector to Stream Deck
        websocket.send(JSON.stringify(json));
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
        var json = {
            "event": "setSettings",
            "context": uuid,
            "payload": {
                "settingsModel": settingsModel
            }
        };
        websocket.send(JSON.stringify(json));
    }
};

function updateUI() {

    document.getElementById('txtNote').value = settingsModel.Note;
    document.getElementById('txtChannel').value = settingsModel.Channel;
    let newSelect = document.getElementById('selDevices');
    for (let i = 0; i < settingsModel.Devices.length; i++) {
        const element = settingsModel.Devices[i];
        var opt = document.createElement("option");
        opt.value = i;
        opt.innerHTML = element;
        newSelect.appendChild(opt);
    }
    newSelect.value = settingsModel.SelectedDevice;
}


