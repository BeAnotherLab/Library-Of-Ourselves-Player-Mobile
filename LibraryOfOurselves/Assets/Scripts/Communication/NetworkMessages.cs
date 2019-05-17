using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Text;

public static class NetworkMessages {

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
			data.WriteChar(s[i]);
		}
		data.WriteChar('#');
	}

	public static string ReadString(this List<byte> data) {
		char c;
		string s = "";
		while(true) {
			c = data.ReadChar();
			if(c == '#') return s;
			s += c;
		}
	}

}
