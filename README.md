# WIP-Player
Mobile version of Library of ourselves app

## Setup
### Setting up the user device
+ Samsung GearVR:
  + Setup your computer to prepare for building:
    + Download the latest version of Unity - tested with Unity 2018.2.2f1.
    + Make sure Unity is ready for Android development; see [this page](https://docs.unity3d.com/Manual/android-sdksetup.html) for more on this.
      + You will need to download Android Studio and the latest version of the JDK from [Oracle's website](http://www.oracle.com/technetwork/java/javase/downloads/jdk8-downloads-2133151.html).
    + Open Android Studio, go to File/Settings and make sure that you have both of the SDK platforms for Android 7 (APIs level 24 and 25); if not, install them now.
    + If you run into some problems during any of the build processes:
      + If the program is frozen, give it some time as it might just be working very slowly. Otherwise, you can try to empty your %TEMP% folder and reinstalling a clean Unity.
      + If Unity displays an error, make sure that you've accepted the confirmation dialog on the phone or tablet; make sure that the JDK and Android SDK paths you've provided are correct. Try restarting Unity.
      + If the error still shows up, open your environment variables (on Windows: Control Panel/System and Security/System/Advanced System Settings/Environment Variables) and check that your system %PATH% has an entry pointing to your Java install's bin/ directory (which should be C:/Program Files/Java/jdkxxx_xxx/bin/).
      + If all else fails, try uninstalling Java and Android Studio and starting from scratch.
  + Use a GearVR-compatible phone ([list here](https://www.unlockunit.com/blog/samsung-gear-vr-compatible-phones/)).
  + Turn on Developper Mode on the phone ([instructions here](https://developer.android.com/studio/debug/dev-options#enable)).
  + Make sure GearVR service is installed and enabled, as described [here](https://support.oculus.com/guides/gear-vr/latest/concepts/gsg-b-sw-software-setup/).
  + Place Oculus Signature file, which you can generate for your phone [here](https://dashboard.oculus.com/tools/osig-generator/), in BeAnother/Assets/Plugins/Android/Assets/ right next to the `PLACE_OSIG_HERE` file.
  + For each VR video XXX that you want to try with, place the 235- or 360-degrees video file XXX.mp4 on the phone's SD card's root. Note that if the phone has two SD cards, you should place it onto the first one.
  + In Unity's Player Settings (Edit/Project Settings/Player/Android), enable _Virtual Reality Supported_.
  + In Unity's Build Settings, select scenes _Scenes/VR/VRAutoconnect_ and _Scenes/VR/VR_.
  + Build the .apk and install it onto the phone, or select the Build And Run option (Ctrl+B on Windows).
  + Go to Settings/Apps/Library of Ourselves/Permissions and enable Storage.
+ OculusGo:
  + Turn on Developper Mode on the OculusGo ([instructions here](https://developer.oculus.com/documentation/mobilesdk/latest/concepts/mobile-device-setup-go/)).
  + For each VR video XXX that you want to try with, place the 235- or 360-degrees video file XXX.mp4 on the OculusGo's internal storage root.
  + If you want to use the binaural audio feature for any video, place the .mp3 files XXX-l.mp3 and XXX-r.mp3 (left and right, respectively) in the same folder.
  + Tnstall _Builds/vr.apk_ onto the OculusGo.
  + The first time running the app, make sure to accept all requested permissions (should only be write and read external storage).
+ Lenovo Mirage Solo:
  + Turn on Developper Mode on the Mirage Solo ([instructions here](https://developer.android.com/studio/debug/dev-options#enable)).
  + For each VR video XXX that you want to try with, place the 235- or 360-degrees video file XXX.mp4 on a micro SD card.
  + Put the SD card into the Mirage Solo (unlike most Android devices, it does not have any MTP drivers).
  + Install _Builds/vr.apk_ onto the Mirage Solo.
  + The first time running the app, make sure to accept all requested permissions (should only be write and read external storage).

### Settings up the guide device
+ Tablet:
  + For each video XXX, place the guide video XXXGuide.mp4 or 360_XXXGuide.mp4 (if the video is 360-degrees), as well as a square thumbnail XXX.jpg, onto the tablet's SD card's root.
  + Install _Builds/guide.apk_ onto the tablet.
  + The first time running it, make sure to accept all requested permissions.
+ Standalone (PC)
  + For each video XXX, place the guide video XXXGuide.mp4 or 360_XXXGuide.mp4 (if the video is 360-degrees), as well as a square thumbnail XXX.jpg, into C:/Users/%USERNAME%/AppData/LocalLow/4gotten/Library of Ourselves/.
  + Run _Builds/Standalone/Library of Ourselves.exe_; recommended settings when launching are Resolution @ 1280x600 (Windowed), and Graphics Quality @ Ultra.

## Use
+ Connect both user and guide device to the same Wi-Fi network.
+ Run both apps; they will pair together shortly. The guide app will show the main menu then.
+ To try the autocalibration feature, in Unity:
  + In the scene _Scenes/Guide/GuideMainMenu_, head to the _Canvas_ GameObject (in hierarchy), and select its first 2 children _Input_ & _Console_; toggle their checkboxes (top left of the inspector tab) to turn them on.
  + Keep the phone still, then write 'autocalibrate on' in the guide app's main menu's input field (bottom of the screen). Wait a few minutes, then type 'autocalibrate off', then 'logs' to see the results.
+ Tap a video to show its description. From there, you can either play it or enter its Advanced Settings (pin is __000001__).
+ The first time you save a video's advanced settings, it will poop out a .json file on the tablet's SD card (or PC's Appdata), with the same name as the video. To modify those settings, you can either delete that file to save them again, or open the file in any text editor to edit it manually if you know how to use JSON.
+ Note that the sound of the video might not play if you connect your headphones while the video is already loaded; it should be preferred to connect headphones prior to running the app to avoid any problems.
+ To recenter the video, the user is provided with an input specific to their platform:
  + On GearVR, double-tap the touchpad on the right of the headset.
  + On OculusGo, press the controller's trigger.
  + On Mirage Solo, press the controller's main button.

## Translation
For now, the guide app is only available in English; all of the different texts are in the en.json file on the tablet (generated upon first run of the guide app). To switch languages:
+ Place a file called "LoO_language.txt" on the guide tablet's SD card root directory (or PC's Appdata); the file should contain exactly "language:X" (where X is the name of whatever language you need, for example Norwegian).
+ Run the guide app once, this will create a new .json file "X.json" (ie "Norwegian.json") on the SD card, filled with the default English text.
+ Locate that file, open it in any text editor and modify it to provide a translation (you might want to look up how to edit JSON data).
+ Once you have a translation file for a language, you can simply copy and paste it to use the same translation on a different tablet; note that it should remain in the same directory.
+ Reverting the contents of LoO_language.txt to "language:en" will turn the app back to English.
+ Note that any change in the language file or LoO_language.txt will require a restart of the guide app.
