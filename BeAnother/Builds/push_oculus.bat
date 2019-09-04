echo "Make sure the file 'oculus.apk' is available in the current folder, then press enter to upload to the OculusGo or Samsung S8 currently plugged into the computer."
pause
c:/users/%username%/appdata/local/android/sdk/platform-tools/adb -d uninstall sco.forgotten.beanother
c:/users/%username%/appdata/local/android/sdk/platform-tools/adb -d install -r oculus.apk
c:/users/%username%/appdata/local/android/sdk/platform-tools/adb -d shell pm grant sco.forgotten.beanother android.permission.READ_EXTERNAL_STORAGE
c:/users/%username%/appdata/local/android/sdk/platform-tools/adb -d shell pm grant sco.forgotten.beanother android.permission.WRITE_EXTERNAL_STORAGE
c:/users/%username%/appdata/local/android/sdk/platform-tools/adb -d shell dumpsys package sco.forgotten.beanother
pause