using UnityEngine;
using System.Collections;

public class CameraController : MonoBehaviour {

	
	public GameObject player;
	private Vector3 offset;
	
	void Start () {
		offset =  new Vector3 (0f,2.42f,0f);
	}
	
	// Update is called once per frame
	void LateUpdate () {
		transform.position = player.transform.position + offset;
	}
}
