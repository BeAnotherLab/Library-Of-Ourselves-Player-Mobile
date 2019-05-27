package sco.haze.vpb;

import android.content.Context;
import android.hardware.*;
import android.os.Build;
import android.os.Environment;
import android.os.StrictMode;

import java.io.BufferedReader;
import java.io.File;
import java.io.FileInputStream;
import java.io.InputStreamReader;

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
}
