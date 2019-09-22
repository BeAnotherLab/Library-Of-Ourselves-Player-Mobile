echo "Make sure the file 'guide0.apk' is available in the current folder, then press enter to upload to the Android device currently plugged into the computer."
pause
c:/users/%username%/appdata/local/android/sdk/platform-tools/adb -d uninstall sco.Haze.LibraryOfOurselvesGuide0
c:/users/%username%/appdata/local/android/sdk/platform-tools/adb -d install -r guide0.apk
c:/users/%username%/appdata/local/android/sdk/platform-tools/adb -d shell pm grant sco.Haze.LibraryOfOurselvesGuide0 android.permission.READ_EXTERNAL_STORAGE
c:/users/%username%/appdata/local/android/sdk/platform-tools/adb -d shell pm grant sco.Haze.LibraryOfOurselvesGuide0 android.permission.WRITE_EXTERNAL_STORAGE
c:/users/%username%/appdata/local/android/sdk/platform-tools/adb -d shell dumpsys package sco.Haze.LibraryOfOurselvesGuide0
pause