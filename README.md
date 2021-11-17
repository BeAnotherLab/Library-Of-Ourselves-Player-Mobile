# Library-Of-Ourselves-Player-Mobile
Mobile version of Library of ourselves app

### Current version: 0.3.0

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

* Guide videos should be named "XXXGuide.mp4", "XXXguide.mp4" or "XXXGUIDE.mp4" and placed in the `Application.PersistentDataPath `folder (that folder will be */Android/data/sco.Haze.LibraryOfOurselvesGuide/files/* on Android)

* Video settings will be saved as "XXX_Settings.json" and placed in the win64 folder on desktop, in the application directory on Android (although these can be copied and read from the same folder as the videos, they can't be written there)

* Video thumbnails should be named "XXX.png" (the extension can also be PNG, jpg, JPG, jpeg, JPEG). Their aspect ratio will be kept, but square images will look nicer.

* Sync settings can temporarily be modified within the Settings for single-user playback:
	- Allowed error is how much time difference there can be before the playback speed starts changing (for example if set to 0.5s, as long as the time difference doesn't go above half a second the playback speed will remain at 1).
	- Maximum error is how much time difference there can be before the playback time automatically jumps to catch up to the other device (for example if set to 1s, whenever the time difference goes above that, playback speed will go back to 1 and instead the time will jump to whichever time the user device is currently on).
	- Max time dilation is the maximum allowed playback speed when the guide app needs to catch up (for example if set to x2, the playback speed will increase from x1 when the time difference == Allowed error and approach x2 as the time difference gets closer to Maximum error; the minimum playback speed is calculated as 1/(max playback speed), so in this case it would be 0.5).
	
* Log files for the app will be generated at runtime in the application directory (*/Android/data/sco.Haze.LibraryOfOurselvesGuide/files/loo_log.txt*).


#### VR app:
* Videos should be saved as "XXX.mp4", with an optional left and right audio tracks saved as "XXX-l.mp3" and "XXX-r.mp3". These files must reside in the `Application.PersistentDataPath `folder (that folder will be */Android/data/sco.Haze.LibraryOfOurselvesGuide/files/* on Android)

* The background colour displayed in the VR app is unique to each device, and can be used to identify which device is which on the guide - these devices can also be renamed later on each guide device (with admin features access).

* Log files for the app will be generated at runtime in the application directory (*/Android/data/sco.Haze.LibraryOfOurselves/files/loo_old_log.txt*).
