using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Text;

public static class NetworkMessages {

	public static void WriteBool(this List<byte> data, bool b) {
		data.Add(b ? (byte)1 : (byte)0);
	}

	public static bool ReadBool(this List<byte> data) {
		byte b = data[0];
		data.Remove(0);
		return b == 0 ? false : true;
	}

	public static void WriteChar(this List<byte> data, char c) {
		data.Add((byte)c);
	}

	public static char ReadChar(this List<byte> data) {
		byte b = data[0];
		data.RemoveAt(0);
		return (char)b;
	}

	public static void WriteString(this List<byte> data, string s) {
		for(int i = 0; i<s.Length; ++i) {
			if(s[i] != '\0')
				data.WriteChar(s[i]);
		}
		data.WriteChar('\0');
	}

	public static string ReadString(this List<byte> data) {
		char c;
		string s = "";
		while(true) {
			c = data.ReadChar();
			if(c == '\0') return s;
			s += c;
		}
	}

	public static void WriteByte(this List<byte> data, byte b) {
		data.Add(b);
	}

	public static byte ReadByte(this List<byte> data) {
		byte b = data[0];
		data.Remove(0);
		return b;
	}

	public static void WriteShort(this List<byte> data, short s) {
		byte byte1, byte2;
		byte2 = (byte)(s >> 8);
		byte1 = (byte)(s & 255);
		data.WriteByte(byte1);
		data.WriteByte(byte2);
	}

	public static short ReadShort(this List<byte> data) {
		byte byte1, byte2;
		byte1 = data.ReadByte();
		byte2 = data.ReadByte();
		return (short)((byte2 << 8) + byte1);
	}

	public static void WriteInt(this List<byte> data, int i) {
		short short1, short2;
		short2 = (short)(i >> 16);
		short1 = (short)(i & 0xFFFF);
		data.WriteShort(short1);
		data.WriteShort(short2);
	}

	public static int ReadInt(this List<byte> data) {
		short short1, short2;
		short1 = data.ReadShort();
		short2 = data.ReadShort();
		return (int)((short2 << 16) + short1);
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
		double seconds = d.Subtract(new DateTime(1970, 1, 1)).TotalSeconds;
		data.WriteDouble(seconds);
	}

	public static DateTime ReadTimestamp(this List<byte> data) {
		double seconds = data.ReadDouble();
		int milliseconds = (int)(seconds*1000) - (int)seconds*1000;
		TimeSpan span = new TimeSpan(0, (int)seconds, milliseconds);
		return new DateTime(1970, 1, 1).Add(span);
	}

}
