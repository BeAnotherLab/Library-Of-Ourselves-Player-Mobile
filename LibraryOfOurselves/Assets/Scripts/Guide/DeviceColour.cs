using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class DeviceColour {
	public static Color getDeviceColor(string uniqueId) {
		if(uniqueId.Length > 6) {
			string redString = uniqueId[0] + "" + uniqueId[1];
			string greenString = uniqueId[2] + "" + uniqueId[3];
			string blueString = uniqueId[4] + "" + uniqueId[5];
			try {
				float hue = int.Parse(redString, System.Globalization.NumberStyles.HexNumber) / 255.0f;
				float saturation = int.Parse(greenString, System.Globalization.NumberStyles.HexNumber) /255.0f;
				float value = int.Parse(blueString, System.Globalization.NumberStyles.HexNumber) / 255.0f;
				saturation = saturation * 0.3f + 0.5f;
				value = value * 0.1f + 0.85f;
				return ColorFromHSV(hue, saturation, value);
			} catch(Exception e) {
				Haze.Logger.LogWarning("Cannot infer colour from unique id: Error = " + e);
			}
		}
		Haze.Logger.LogWarning("Cannot infer colour from unique id: " + uniqueId);
		return new Color(1, 1, 1);
	}


	static Color ColorFromHSV(double hue, double saturation, double value) {
		hue *= 360.0;
		int hi = Convert.ToInt32(Math.Floor(hue / 60)) % 6;
		double f = hue / 60 - Math.Floor(hue / 60);

		value = value * 255;
		int v = Convert.ToInt32(value);
		int p = Convert.ToInt32(value * (1 - saturation));
		int q = Convert.ToInt32(value * (1 - f * saturation));
		int t = Convert.ToInt32(value * (1 - (1 - f) * saturation));

		if(hi == 0)
			return new Color(v/255.0f, t/255.0f, p/255.0f);
		else if(hi == 1)
			return new Color(q/255.0f, v/255.0f, p/255.0f);
		else if(hi == 2)
			return new Color(p/255.0f, v/255.0f, t/255.0f);
		else if(hi == 3)
			return new Color(p/255.0f, q/255.0f, v/255.0f);
		else if(hi == 4)
			return new Color(t/255.0f, p/255.0f, v/255.0f);
		else
			return new Color(v/255.0f, p/255.0f, q/255.0f);
	}
}
