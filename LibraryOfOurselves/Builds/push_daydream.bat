echo "Make sure the file 'daydream.apk' is available in the current folder, then press enter to upload to the Lenovo Mirage Solo currently plugged into the computer."
pause
c:/users/%username%/appdata/local/android/sdk/platform-tools/adb -d uninstall sco.Haze.LibraryOfOurselves
c:/users/%username%/appdata/local/android/sdk/platform-tools/adb -d install -r daydream.apk
c:/users/%username%/appdata/local/android/sdk/platform-tools/adb -d shell pm grant sco.Haze.LibraryOfOurselves android.permission.READ_EXTERNAL_STORAGE
c:/users/%username%/appdata/local/android/sdk/platform-tools/adb -d shell pm grant sco.Haze.LibraryOfOurselves android.permission.WRITE_EXTERNAL_STORAGE
c:/users/%username%/appdata/local/android/sdk/platform-tools/adb -d shell dumpsys package sco.Haze.LibraryOfOurselves
pause