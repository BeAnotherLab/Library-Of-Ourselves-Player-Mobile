using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class DefaultController : MonoBehaviour {
	
	[SerializeField] float multiplier = 20;
	
	Rigidbody2D body;
	Joystick joystick;
	
	void Start(){
		body = GetComponent<Rigidbody2D>();
		joystick = (Joystick)FindObjectOfType(typeof(Joystick));
	}
	
	void FixedUpdate(){
		
		body.AddForce(new Vector2(joystick.Axes.x, joystick.Axes.y) * multiplier);
		
	}
}
