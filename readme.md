# Plex Web Hook

This is a simple application to turn on different Hue Light scenes in a room in response to a movie or show starting / stopping on Plex.  Before you start you need:

1. A Hue Bridge
2. A Plex Media server (and a Plex Pass to enable web hooks)

## Getting Started

### 1. You Settings

You will need to add your settings to the appsettings.json file.  You may want to do this by copying the file to 'appsettings.development.json' rather than modifying the base file.

### 2. Create a Hue User

Follow the instructions on the Hue website to create a new authorised user on your Hue Bridge: https://developers.meethue.com/documentation/getting-started

Paste your new user ID into the appsetting: `hue-user`

### 3. Populate Hue Scene and Room IDs

The following settings need to be populated with IDs (not names) from Hue:

* `scene-to-show-when-playing`: ID of the Scene to turn on when a movie or show starts
* `scene-to-show-when-stopped`: ID of the Scene to turn on when a movie or show stops
* `theatre-room`: ID of the Group (room) where the Scene should be used.  Note that this ID is required by the Hue API, but Scene will be turned on in whichever room it is located regardless of the setting.

You can use the form at: http://[hue bridge ip address]/debug/clip.html to query your Bridge for the Scene IDs and Room IDs.

### 4. Populate the Device UUID

While logged into Plex, navigate to https://plex.tv/devices.xml 

Find the device that should trigger the light scenes, and copy the `clientIdentifier` into the setting `device-uuid`.

### 5. Add a WebHook

On your Plex Media server, add a WebHook that points to the new service.  It is recommended to run the web hook service on the same device as the WebHook service, otherwise you will have to worry about firewall settings.

See: https://support.plex.tv/articles/115002267687-webhooks/

If you run on the same device as the server, the WebHook URL will be: `http://localhost:5050` by default.

### 6. Run

Using the command prompt in the `src\PlexWebHook` folder, type:

```
dotnet run
```

Using a browser, navigate to:

* http://localhost:5050/start to see the light scene for playing movies or shows turn on
* http://localhost:5050/stop to see the light scene for stopping movies or shows turn on

If everything is configured correctly, starting and stopping a movie or show on the desired device should also turn on the correct Hue Scenes.

## Deploying

1. Make sure you have the right settings in an appsettings.Release.json file.

2. Run `dotnet publish --configuration Release`

3. Follow the instructions to host as a Windows Service (if desired - Windows services can be used to run the service on PC start up without logging in): https://docs.microsoft.com/en-us/aspnet/core/host-and-deploy/windows-service?view=aspnetcore-2.1
 
Note: you may need to manually copy settings from an environment specific file to the appsettings.json file after it has been published when running as a service.
Note: I have included some commented out code that should work when running as a Windows Service.