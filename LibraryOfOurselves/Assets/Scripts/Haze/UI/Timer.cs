using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Haze{
    public class Timer : MonoBehaviour{

        [SerializeField] bool onStart = true;
        [SerializeField] string format = "H:M:S'm";//H for hours, M for minutes, S for seconds, m for milliseconds
        [SerializeField] float stopAt = -1;
        [SerializeField] UnityEvent onStop;

        float elapsed = 0;
        bool started = false;
        Text display;

        void Start(){
            display = GetComponent<Text>();
            if (onStart)
                StartTimer();
        }

        public void StartTimer(){
            started = true;
        }

        public void StopTimer()
        {
            started = false;
            onStop.Invoke();
        }

        void Update(){
            if (started){
                elapsed += Time.deltaTime;
                if (display)
                    updateDisplay();
                if(stopAt >= 0 && elapsed >= stopAt){
                    StopTimer();
                }
            }
        }

        void updateDisplay(){
            int milliseconds = (int)(elapsed * 1000) % 1000;
            int seconds = (int)(elapsed) % 60;
            int minutes = (int)(elapsed / 60) % 60;
            int hours = (int)(elapsed / 3600);
            string m = (milliseconds < 100 ? "00" : milliseconds < 10 ? "0" : "") + milliseconds;
            string S = (seconds < 10 ? "0" : "") + seconds;
            string M = (minutes < 10 ? "0" : "") + minutes;
            string H = (hours < 10 ? "0" : "") + hours;
            display.text = format.Replace("m", m).Replace("S", S).Replace("M", M).Replace("H", H);
        }

    }
}