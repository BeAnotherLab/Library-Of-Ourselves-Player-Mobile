using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rotate : MonoBehaviour {
	
	[SerializeField] float speed = 100.0f;
    [SerializeField] Axis axis = Axis.Z;

    enum Axis
    {
        X, Y, Z
    }

    void Update() {

        switch (axis) {
            case Axis.X:
                transform.Rotate(speed * Time.deltaTime, 0, 0);
                break;
            case Axis.Y:
                transform.Rotate(0, speed * Time.deltaTime, 0);
                break;
            case Axis.Z:
                transform.Rotate(0, 0, speed * Time.deltaTime);
                break;
        }
		
	}
	
}
