c:/users/%username%/appdata/local/android/sdk/platform-tools/adb -d uninstall sco.forgotten.beanother
c:/users/%username%/appdata/local/android/sdk/platform-tools/adb -d install -r vr.apk
c:/users/%username%/appdata/local/android/sdk/platform-tools/adb -d shell pm grant sco.forgotten.beanother android.permission.WRITE_EXTERNAL_STORAGE
pause