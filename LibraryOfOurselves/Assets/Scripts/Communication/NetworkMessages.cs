using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Text;

public static class NetworkMessages {

	public static void WriteByte(this List<byte> data, byte b) {
		data.Add(b);
	}

	public static byte ReadByte(this List<byte> data) {
		byte b = data[0];
		data.RemoveAt(0);
		return b;
	}

	public static void WriteBool(this List<byte> data, bool b) {
		data.WriteByte(b ? (byte)1 : (byte)0);
	}

	public static bool ReadBool(this List<byte> data) {
		byte b = data.ReadByte();
		return b == 0 ? false : true;
	}

	public static void WriteChar(this List<byte> data, char c) {
		data.WriteByte((byte)c);
	}

	public static char ReadChar(this List<byte> data) {
		byte b = data.ReadByte();
		return (char)b;
	}

	public static void WriteString(this List<byte> data, string s) {
		byte[] bytes = Encoding.UTF8.GetBytes(s);
		data.WriteShort((short)bytes.Length);
		data.AddRange(bytes);
	}

	public static string ReadString(this List<byte> data) {
		short strLength = data.ReadShort();
		if(strLength > data.Count)
			strLength = (short)data.Count;
		List<byte> readBytes = data.GetRange(0, strLength);
		data.RemoveRange(0, strLength);
		return Encoding.UTF8.GetString(readBytes.ToArray());
	}

	public static void WriteShort(this List<byte> data, short s) {
		data.AddRange(BitConverter.GetBytes(s));
	}

	public static short ReadShort(this List<byte> data) {
		List<byte> bytes = data.GetRange(0, 2);
		data.RemoveRange(0, 2);
		return BitConverter.ToInt16(bytes.ToArray(), 0);
	}

	public static void WriteInt(this List<byte> data, int i) {
		data.AddRange(BitConverter.GetBytes(i));
	}

	public static int ReadInt(this List<byte> data) {
		List<byte> bytes = data.GetRange(0, 4);
		data.RemoveRange(0, 4);
		return BitConverter.ToInt32(bytes.ToArray(), 0);
	}

	public static void WriteFloat(this List<byte> data, float f) {
		data.AddRange(BitConverter.GetBytes(f));
	}

	public static float ReadFloat(this List<byte> data) {
		List<byte> bytes = data.GetRange(0, 4);
		data.RemoveRange(0, 4);
		return BitConverter.ToSingle(bytes.ToArray(), 0);
	}

	public static void WriteDouble(this List<byte> data, double d) {
		data.AddRange(BitConverter.GetBytes(d));
	}

	public static double ReadDouble(this List<byte> data) {
		List<byte> bytes = data.GetRange(0, 8);
		data.RemoveRange(0, 8);
		return BitConverter.ToDouble(bytes.ToArray(), 0);
	}

	public static void WriteTimestamp(this List<byte> data, DateTime d) {
		int min = d.Minute;
		int sec = d.Second;
		int millis = d.Millisecond;
		data.WriteByte((byte)min);
		data.WriteByte((byte)sec);
		data.WriteShort((short)millis);
	}

	public static DateTime ReadTimestamp(this List<byte> data) {
		int min = data.ReadByte();
		int sec = data.ReadByte();
		int millis = data.ReadShort();
		DateTime now = DateTime.Now;
		int nowHr = now.Hour;
		int nowDay = now.Day;
		int nowMonth = now.Month;
		int nowYear = now.Year;
		if(now.Minute < min) {//sending a time one hour and receiving it the next
			--nowHr;
			if(nowHr < 0) {//sending a time one day and receiving it the next
				nowHr = 24;
				--nowDay;
				if(nowDay < 0) {//sending a time one month and receiving it the next (lol)
					--nowMonth;
					if(nowMonth < 0) {//sending a time one year and receiving it the next (yeah no one is ever going to fire this code lmao)
						nowMonth = 11;
						--nowYear;
					}
					nowDay = DateTime.DaysInMonth(nowYear, nowMonth) - 1;
				}
			}
		}
		try {
			return new DateTime(nowYear, nowMonth, nowDay, nowHr, min, sec, millis);
		}catch(ArgumentOutOfRangeException aoore) {
			Debug.LogError("Failed to read timestamp from data stream. Details:\nmin: " + min + "\nsec: " + sec + "\nmillis: " + millis + "\nnowHr: " + nowHr + "\nnowDay: " + nowDay + "\nnowMonth: " + nowMonth + "\nnowYear:" + nowYear);
			Debug.LogError(aoore);
			return DateTime.Now;//upon failing, simply return a timestamp for right now instead.
		}
	}

	public static void WriteVector3(this List<byte> data, Vector3 v) {
		data.WriteFloat(v.x);
		data.WriteFloat(v.y);
		data.WriteFloat(v.z);
	}

	public static Vector3 ReadVector3(this List<byte> data) {
		Vector3 ret = new Vector3();
		ret.x = data.ReadFloat();
		ret.y = data.ReadFloat();
		ret.z = data.ReadFloat();
		return ret;
	}

}
