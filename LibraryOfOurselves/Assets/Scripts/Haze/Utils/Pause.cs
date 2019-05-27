using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Haze{
	public class Pause : MonoBehaviour {
		
		static bool isPaused = false;
		
		public static bool Paused{
			get{ return isPaused; }
		}
		
		void OnEnable(){
			///Pause
			Time.timeScale = 0;
			isPaused = true;
		}
		
		void OnDisable(){
			///Play
			Time.timeScale = 1;
			isPaused = false;
		}
		
	}
}