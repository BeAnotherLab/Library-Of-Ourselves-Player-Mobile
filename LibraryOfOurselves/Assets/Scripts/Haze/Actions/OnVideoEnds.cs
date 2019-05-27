using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;
using UnityEngine.Events;

public class OnVideoEnds : MonoBehaviour {
	
	[SerializeField] VideoPlayer player;//can be null
	[SerializeField] UnityEvent onEnd;
	[SerializeField] float delayAfterStart = -1;//if this is positive, will invoke event when player reaches this time rather than end
	[SerializeField] bool verbose;
	
	void Start(){
		if(!player)
			player = GetComponent<VideoPlayer>();
		if(delayAfterStart >= 0){
			StartCoroutine(checkTime());
		}else{
            /*player.loopPointReached += delegate{
				if(verbose)
					print(name + ": Video ended");
				onEnd.Invoke();
			};*/
            StartCoroutine(waitForEnd());
		}
	}

    IEnumerator waitForEnd(){
        while(player.isPlaying || Time.timeScale == 0)
        {
            yield return null;
        }
        if (verbose)
            print(name + ": Video ended");
        onEnd.Invoke();
    }
	
	IEnumerator checkTime(){
		while(true){
			yield return null;
			if(player.time >= delayAfterStart){
				if(verbose)
					print(name + ": Video reached "+player.time);
				onEnd.Invoke();
				yield break;
			}
		}
	}
	
}
