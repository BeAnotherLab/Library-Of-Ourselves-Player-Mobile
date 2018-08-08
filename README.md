# WIP-Player
Mobile version of Library of ourselves app

## Setup
### Setting up Unity
+ Make sure Unity is ready for Android development; see [this page](https://docs.unity3d.com/Manual/android-GettingStarted.html) for more on this.

### Setting up the phone
+ Use a GearVR-compatible phone ([list here](https://www.unlockunit.com/blog/samsung-gear-vr-compatible-phones/)).
+ Make sure GearVR service is installed and enabled, as described [here](https://support.oculus.com/guides/gear-vr/latest/concepts/gsg-b-sw-software-setup/).
+ Place Oculus Signature file, which you can generate for your phone [here](https://dashboard.oculus.com/tools/osig-generator/), in BeAnother/Assets/Plugins/Android/Assets/ right next to the `PLACE_OSIG_HERE` file.
+ For each VR video XXX that you want to try with, place the 235- or 360-degrees video file XXX.mp4 on the phone's SD card's root. Note that if the phone has two SD cards, you should place it onto the first one.
+ In Unity's Player Settings (Edit/Project Settings/Player/Android), enable _Virtual Reality Supported_.
+ In Unity's Build Settings, select scenes _Scenes/VR/VRAutoconnect_ and _Scenes/VR/VR_.
+ Build the .apk and install it onto the phone.
+ On the phone, go to Settings/Apps/Gear VR Service/Storage/Manage Storage/ and tap on _VR Service Version_ several times until the developper options turn up; turn on _Developper mode_.
+ The first time running BeAnother, make sure to accept all requested permissions (should only be write and read external storage).

### Settings up the tablet
+ For each video XXX, place the guide video XXXGuide.mp4 or 360_XXXGuide.mp4 (if the video is 360-degrees), as well as a thumbnail XXX.jpg onto the tablet's SD card's root.
+ In Unity's Player Settings, disable _Virtual Reality Supported_.
+ In Unity's Build Settings, select scenes _Scenes/GuideAutoconnect_ and everything that starts with _Scenes/Guide/_.
+ Build the .apk and install it onto the tablet.
+ The first time running it, make sure to accept all requested permissions.

## Use
+ Connect both the phone and the tablet to the same Wi-Fi network.
+ Run both apps; the phone will first show pure white until the devices pair, at which point it will turn dark. The guide app will show the main menu then.
+ To try the autocalibration feature, keep the phone still, then write 'autocalibrate on' in the guide app's main menu's input field (bottom of the screen). Wait a few minutes, then type 'autocalibrate off', then 'logs' to see the results.
+ Tap a video to show its description. From there, you can either play it or enter its Advanced Settings (pin for now is __000000__).
+ The first time you save a video's advanced settings, it will poop out a .json file on the tablet's SD card. To modify those settings, you can either delete that file to save them again, or open the file in a text editor to edit it manually.
+ For now, the guide app is only in English; all of the different texts are in the en.json file on the tablet (generated upon first run of the guide app). You can switch languages by editting this file and saving it under a different name (for example, no.json for norwegian); then in Unity, head to Assets/Scenes/GuideAutoconnect.unity and change the GameObject _Language_ so that its _Lang_ component's _Language_ field is 'no' instead of 'en'.
