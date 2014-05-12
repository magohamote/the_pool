using UnityEngine;
using System.Collections;

public class CameraController : MonoBehaviour {

	
	public GameObject player;
	private Vector3 offset;
	private Quaternion offsetRot;
	
	void Start () {
		offset =  new Vector3 (0f,2.42f,0f);
		offsetRot = player.transform.rotation;
	}
	
	// Update is called once per frame
	void LateUpdate () {
		transform.position = player.transform.position + offset;
		transform.rotation = player.transform.rotation * offsetRot;
	}
}
