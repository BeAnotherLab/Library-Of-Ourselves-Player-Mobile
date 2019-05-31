package sco.haze.vpb;

import android.Manifest;
import android.annotation.TargetApi;
import android.app.Activity;
import android.app.Fragment;
import android.app.FragmentManager;
import android.app.FragmentTransaction;
import android.content.Context;
import android.content.pm.PackageManager;
import android.hardware.*;
import android.os.Build;
import android.os.Environment;
import android.support.v4.content.ContextCompat;

import java.io.BufferedReader;
import java.io.File;
import java.io.InputStreamReader;

import com.unity3d.player.UnityPlayer;

public class VrPlayerBindings implements SensorEventListener {

    public static VrPlayerBindings instance = null;

    public static VrPlayerBindings getInstance(Context context){
        if(instance == null)
            instance = new VrPlayerBindings(context);
        return instance;
    }






    Context context;
    float temperatureFromSensor = Float.NEGATIVE_INFINITY;

    public String message = "";
    public String callbackGameObject;
    public String callbackMethod;





    public float getTemperature(){
        if(temperatureFromSensor == Float.NEGATIVE_INFINITY)
            return getCpuTemp();
        return temperatureFromSensor;
    }

    public String getSDCardPath(){
        File[] externalFilesDirs = context.getExternalFilesDirs(null);
        File emulated = null, sd = null;
        if(Build.VERSION.SDK_INT > 21) {
            for (int i = 0; i < externalFilesDirs.length; ++i) {
                File dir = externalFilesDirs[i];
                if (Environment.isExternalStorageEmulated(dir)) {
                    emulated = dir;
                } else if (Environment.isExternalStorageRemovable(dir)) {
                    sd = dir;
                }
            }
            if(sd != null) return sd.getAbsolutePath();
            if(emulated != null) return emulated.getAbsolutePath();
        }
        if(externalFilesDirs.length > 0)//fallback on just sending the first one
            return externalFilesDirs[0].getAbsolutePath();
        return null;
    }

    public boolean isExternalStoragePermissionEnabled(){
        if(ContextCompat.checkSelfPermission(context, Manifest.permission.WRITE_EXTERNAL_STORAGE) == PackageManager.PERMISSION_GRANTED){
            return true;
        }
        return false;
    }

    public void requestExternalStoragePermission(String callbackGameObject, String callbackMethod) {
        this.callbackGameObject = callbackGameObject;
        this.callbackMethod = callbackMethod;
        if (Build.VERSION.SDK_INT >= 23) {
            //ActivityCompat.requestPermissions(currentActivity, new String[]{Manifest.permission.WRITE_EXTERNAL_STORAGE}, 0);
            try {
                final FragmentManager fragmentManager = UnityPlayer.currentActivity.getFragmentManager();
                final Fragment request = new RequestFragment();
                FragmentTransaction fragmentTransaction = fragmentManager.beginTransaction();
                fragmentTransaction.add(0, request);
                fragmentTransaction.commit();
            } catch (Exception e) {
                UnityPlayer.UnitySendMessage(callbackGameObject, callbackMethod, "Could not request external storage permissions: " + e.getMessage());
            }
        }else{
            UnityPlayer.UnitySendMessage(callbackGameObject, callbackMethod, "Cannot request permissions on Android versions < 23");
        }
    }





    public VrPlayerBindings(Context context){
        this.context = context;
        try {
            SensorManager sensorManager = (SensorManager) context.getSystemService(Context.SENSOR_SERVICE);
            Sensor temperatureSensor = sensorManager.getDefaultSensor(Sensor.TYPE_AMBIENT_TEMPERATURE);
            if(temperatureSensor == null){
                temperatureSensor = sensorManager.getDefaultSensor(Sensor.TYPE_TEMPERATURE);
            }
            if(temperatureSensor != null)
                sensorManager.registerListener(this, temperatureSensor, SensorManager.SENSOR_DELAY_FASTEST);
            else
                message += "[VPB] No temperature sensor.\n";
        }catch(Exception e){
            message += "[VPB] Error: " + e.getMessage() + "\n";
        }
    }

    @Override
    public void onSensorChanged(SensorEvent event) {
        temperatureFromSensor = event.values[0];
    }

    @Override
    public void onAccuracyChanged(Sensor sensor, int accuracy) {
        message += "[VPB] Accuracy changed: " + accuracy + "\n";
    }

    private float getCpuTemp() {
        Process p;
        try {
            p = Runtime.getRuntime().exec("cat sys/class/thermal/thermal_zone0/temp");
            p.waitFor();
            BufferedReader reader = new BufferedReader(new InputStreamReader(p.getInputStream()));

            String line = reader.readLine();
            float temp = Float.parseFloat(line) / 1000.0f;

            return temp;

        } catch (Exception e) {
            e.printStackTrace();
            return 0.0f;
        }
    }

    @TargetApi(23)
    public static class RequestFragment extends Fragment{
        @Override
        public void onStart() {
            super.onStart();
            requestPermissions(new String[]{Manifest.permission.WRITE_EXTERNAL_STORAGE}, 0);
        }

        @Override
        public void onRequestPermissionsResult(int requestCode, String[] permissions, int[] grantResults) {
            String result = "PERMISSION:";
            if (grantResults.length > 0 && grantResults[0] == PackageManager.PERMISSION_GRANTED){
                result+="GRANTED";
            }else{
                result+="DENIED";
            }
            UnityPlayer.UnitySendMessage(VrPlayerBindings.instance.callbackGameObject, VrPlayerBindings.instance.callbackMethod, result);
            FragmentTransaction fragmentTransaction = getFragmentManager().beginTransaction();
            fragmentTransaction.remove(this);
            fragmentTransaction.commit();
        }
    }
}
