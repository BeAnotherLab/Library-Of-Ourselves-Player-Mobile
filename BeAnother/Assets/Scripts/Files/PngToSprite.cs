using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class PngToSprite {
	
	public static Sprite LoadSprite(string path, float pixelsPerUnit = 100.0f){
		Texture2D spriteTexture = LoadTexture(path);
		if(spriteTexture == null) return null;
		return Sprite.Create(spriteTexture, new Rect(0, 0, spriteTexture.width, spriteTexture.height), Vector2.zero, pixelsPerUnit);
	}
	
	public static Texture2D LoadTexture(string path){
		if(File.Exists(path)){
			byte[] data = File.ReadAllBytes(path);
			Texture2D tex2d = new Texture2D(2, 2);
			if(tex2d.LoadImage(data))
				return tex2d;
		}
		Debug.Log("Cannot load texture from file " + path);
		return null;
	}
	
}
