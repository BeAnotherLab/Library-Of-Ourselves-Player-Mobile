using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Security.Cryptography;
using System.Text;
using System;

public class Hashing {
	
	public static string Sha256(string str){
		SHA256Managed crypt = new SHA256Managed();
		StringBuilder hash = new StringBuilder();
		byte[] crypto = crypt.ComputeHash(Encoding.UTF8.GetBytes(str), 0, Encoding.UTF8.GetByteCount(str));
		foreach(byte bit in crypto){
			hash.Append(bit.ToString("x2"));
		}
		return hash.ToString().ToLower();
	}
	
	public static string Md5(string str){
		ASCIIEncoding encoding = new ASCIIEncoding();
		byte[] bytes = encoding.GetBytes(str);
		var crypt = new MD5CryptoServiceProvider();
		return BitConverter.ToString(crypt.ComputeHash(bytes));
	}
	
	/* Short, non-secure hashes */
	public static string Adler32(string str){
		const int mod = 65521;
		uint a = 1, b = 0;
		foreach (char c in str){
			a = (a + c) % mod;
			b = (b + a) % mod;
		}
		return "" + ((b << 16) | a);
	}
	
}
