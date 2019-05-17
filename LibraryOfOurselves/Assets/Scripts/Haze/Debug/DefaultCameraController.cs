using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DefaultCameraController : MonoBehaviour {
	
	[SerializeField] float speed = 10;
	[SerializeField] float aspeed = 90;
	
	void Update () {
		
		float horizontal = Input.GetAxis("Horizontal");
		float vertical = Input.GetAxis("Vertical");
		float other = Input.GetKey(KeyCode.E) ? 1 : Input.GetKey(KeyCode.Q) ? -1 : 0;
		transform.Translate(horizontal*speed*Time.deltaTime, other*speed*Time.deltaTime, vertical*speed*Time.deltaTime);
		
		float mouseX = Input.GetAxis("Mouse X");
		float mouseY = -Input.GetAxis("Mouse Y");
		transform.Rotate(mouseY*aspeed*Time.deltaTime, mouseX*aspeed*Time.deltaTime, 0);
		transform.eulerAngles = new Vector3(transform.eulerAngles.x, transform.eulerAngles.y, 0);
		
	}
}
