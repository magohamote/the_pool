using UnityEngine;
using System.Collections;


public class PlayerController : MonoBehaviour {

	
	
	public float speed;
	
	void FixedUpdate ()
	{
		
		float turn = Input.GetAxis("Horizontal");

   
		transform.Rotate(0, turn * 90 * Time.deltaTime, 0);
		
		
		Vector3 moveDirection = transform.forward * Input.GetAxis("Vertical") * -1 * speed;
        
    
		if (Input.GetKey ("space")) {
            moveDirection.y = 0;
        }
		// Move the controller
		rigidbody.AddForce(moveDirection * Time.deltaTime);
		
		
		
		
	}
	
	void OnTriggerEnter(Collider other) {
		if(other.gameObject.tag == "PickUp") {
			other.gameObject.SetActive(false);
		}
	}
}


