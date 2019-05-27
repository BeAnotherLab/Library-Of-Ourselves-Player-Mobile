/*
	This script is a wrapper around Unity's Post Processing Stack.
	Allows to set values at runtime for post processing when HAZE_POSTPROCESSING
	is defined. If not, these calls won't do anything significant.
	
	This allows Haze scripts (and other assets) to support the Unity Post Processing
	Stack whether or not it's included in  project.
	
	As it is, I'm providing this file as a helper for you. You can modify it, and
	unlike other files from Haze assets, you're free to pass this one along.
	
	Keep this header if you want to~
	
	Jonathan Kings from Haze
*/

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if HAZE_POSTPROCESSING
using UnityEngine.PostProcessing;
#endif

namespace Haze{
	public static class PostProcessingBridge {
		
		///Whether the PostProcessingBridge will have any effect at all.
		public static bool Available{
			get{
				#if HAZE_POSTPROCESSING
				return true;
				#else
				return false;
				#endif
			}
		}
		
		///Call this anywhere to change the Focus Distance of the Depth of Field effect.
		public static void SetDepthOfField_FocusDistance(float val, Camera cam = null){
			#if HAZE_POSTPROCESSING
			if(cam == null) cam = Camera.main;
			PostProcessingBehaviour ppBehaviour = cam.GetComponent<PostProcessingBehaviour>();
			if(ppBehaviour){
				PostProcessingProfile profile = ppBehaviour.profile;
				if(profile != null){
					DepthOfFieldModel dof = profile.depthOfField;
					if(dof != null && dof.enabled){
						DepthOfFieldModel.Settings settings = dof.settings;
						
						//Set focus distance.
						settings.focusDistance = val;
						
						dof.settings = settings;
					}
				}
			}
			#endif
		}
		
	}
}