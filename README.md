# Library-Of-Ourselves-Player-Mobile
Mobile version of Library of ourselves app

### Current version: 0.3.5-beta

## Video codec and export settings
For smooth playback of the videos, the following export settings from Adobe Premiere Pro are recommended:
#### Guide videos:
* Codec: H.264 (.mp4 format)
* Resolution: At most 1024 on either axis
* Frame rate: 25 to 30 (try to keep this number a round integer for best quality)
* Field order: Progressive
* Render at maximum depth + Use maximum render quality both checked
* Target bitrate: 0.19Mbps for tablets, up to 20Mbps for desktops
* Maximum bitrate: 0.19Mbps for tablets, up to 20Mbps for desktops
* Audio bitrate: at most 160kbps
* Precedence (audio): Bitrate over Sample rate
* Use FFMpeg to render videos with a keyframe every frame for [optimal skipping performance](https://www.renderheads.com/content/docs/AVProVideo/articles/feature-seeking-playbackrate.html)


## Usage guidelines
To use these guidelines, replace any "XXX" within filenames with the video's name you wish - note that these must be exactly the same for every file, with the same capitalisation.


#### Guide app:
* Use pin 000101 to unlock app initially and to access advanced admin features (pin can be modified in Settings).

* Guide videos should be named "XXXGuide.mp4", "XXXguide.mp4" or "XXXGUIDE.mp4" and placed *in any folder you like*. For content downloaded through the content downloader, check below for instructins. Upon opening the app for the first time, a dialog will appear asking you to set the content folder. The folder can be changed later in the settings menu.

* Video settings will be saved as "XXX_Settings.json" and placed in the win64 folder on desktop, in the application directory on Android (although these can be copied and read from the same folder as the videos, they can't be written there)

* Video thumbnails should be named "XXX.png" (the extension can also be PNG, jpg, JPG, jpeg, JPEG). Their aspect ratio will be kept, but square images will look nicer.

* Sync settings can temporarily be modified within the Settings for single-user playback:
	- Allowed error is how much time difference there can be before the playback speed starts changing (for example if set to 0.5s, as long as the time difference doesn't go above half a second the playback speed will remain at 1).
	- Maximum error is how much time difference there can be before the playback time automatically jumps to catch up to the other device (for example if set to 1s, whenever the time difference goes above that, playback speed will go back to 1 and instead the time will jump to whichever time the user device is currently on).
	- Max time dilation is the maximum allowed playback speed when the guide app needs to catch up (for example if set to x2, the playback speed will increase from x1 when the time difference == Allowed error and approach x2 as the time difference gets closer to Maximum error; the minimum playback speed is calculated as 1/(max playback speed), so in this case it would be 0.5).

#### VR app:
* Videos should be saved as "XXX.mp4", with an optional left and right audio tracks saved as "XXX-l.mp3" and "XXX-r.mp3". These files must reside in the `Application.PersistentDataPath` 

* The background colour displayed in the VR app is unique to each device, and can be used to identify which device is which on the guide - these devices can also be renamed later on each guide device (with admin features access).

### Content download:
To download content from the app on both tablet and VR headset, use the content download scene.
On Quest headsets, press all buttons on the right controller at once to open it.
On Android tabletes, you can open it from the menu settings. Make sure you press the "user persistent data path" button to make the content show up.

## Content Server Setup

This guide explains how to set up a local content server for Android devices using **Caddy**. The server hosts video content and a manifest that the Unity application uses to download and synchronize files.

### 1. Install Caddy

Download Caddy from the official GitHub releases page:

https://github.com/caddyserver/caddy/releases

Download the following release:

```
caddy_2.11.4_windows_amd64.zip
```

Extract the executable somewhere convenient, for example:

```text
C:\caddy\
```

### 2. Create the Deployment Folder

Follow this structure:

```text
Deploy/
│
├── manifest.json
├── generate_manifest.sh
└── Content/
    ├── Guide/
    │   └── JonahGuide.mp4
    └── User/
        └── Jonah.mp4
```

This folder contains all content that will be served to the Quest headsets and tablets.

### 3. Generate the Manifest

Whenever content is added, removed, or updated, regenerate the manifest.

Run the manifest generation script: [`generate_manifest.sh`](./LibraryOfOurselves/Utils/generate_manifest.sh)

This updates `manifest.json` with the latest:

* File sizes
* SHA-256 hashes

The Unity application uses this manifest to determine which files need to be downloaded.

### 4. Start the Server

Open PowerShell in the deployment folder.

```powershell
cd C:\caddy\Deploy
```

Start the Caddy file server:

```powershell
caddy file-server --listen :8080
```

If successful, Caddy will report that it is listening on port **8080**.

### 5. Test the Server Locally

Open a browser on the server PC and navigate to:

```
http://localhost:8080/
```

Verify that you can access the manifest:

```
http://localhost:8080/manifest.json
```

Verify that content is accessible, for example:

```
http://localhost:8080/Content/Guide/JonahGuide.mp4
```

### Updating Content

Whenever new content is published:

1. Add or replace files inside the `Content/` directory.

2. Run:

   ```bash
   ./generate_manifest.sh
   ```

3. Ensure Caddy is running (restart it if necessary).

4. Launch the application on the Quest headsets.

Each headset compares the latest `manifest.json` against its local files and downloads **only** content that is missing or has changed.


