// Set an off-center projection, where perspective's vanishing
// point is not necessarily in the center of the screen.
//
// adapted by Henrique Debarba to dinamically correct pMatrix
// according to scrren size/pos and camera pos


using UnityEngine;
using System.Collections;

[ExecuteInEditMode]
public class OffCenterPerspective : MonoBehaviour {
	
	// position and scale of the tgt screen is used to set the projection
	// scale in x defines horizontal size and scale in y defines vertical size
	public Transform tgtScreen;

		
	void LateUpdate() {
		// screen parameters must be defined in a gameObject transform in the scene (make input more intuitive) 
		if (tgtScreen!=null){

			// screen size as defined by tgtScreen transform
			float horScreenSize = tgtScreen.localScale.x;
			float verScreenSize = tgtScreen.localScale.y;

			Quaternion resetOrient = Quaternion.Inverse(transform.rotation);

			// allign screen and camera with world coord system
			Vector3 axisAllignedCamPos = resetOrient*transform.position;
			Vector3 axisAllignedScreenPos = resetOrient*tgtScreen.position;

			// distance between screen and camera along screen normal direction (z axis)
			float distance2Screen = Mathf.Abs(axisAllignedCamPos.z - axisAllignedScreenPos.z);

			// normilized position of the camera in a plane parallel to the screen
			// values < 0 and > 1 means the camera is beyond the borders of the screen
			float xZero = (axisAllignedCamPos.x - axisAllignedScreenPos.x)/horScreenSize + 0.5f;
			float yZero = (axisAllignedCamPos.y - axisAllignedScreenPos.y)/verScreenSize + 0.5f;


	   		// this script is ment to be attached to a camera
			if (camera!=null){
				float dist2NearPlane = camera.nearClipPlane;
				if (distance2Screen < dist2NearPlane)
					distance2Screen = dist2NearPlane;

				// define boundaries of the projection in terms of camera near plane (plane of projection)
				float top = (((1.0f-yZero) * verScreenSize)/distance2Screen) * dist2NearPlane;
				float bottom = (((-yZero) * verScreenSize)/distance2Screen) * dist2NearPlane;		
				float right = (((1.0f-xZero) * horScreenSize)/distance2Screen) * dist2NearPlane;
				float left = (((-xZero) * horScreenSize)/distance2Screen) * dist2NearPlane;

				// create new projection matrix
				Matrix4x4 m = OffCenter(left, right, bottom, top, camera.nearClipPlane, camera.farClipPlane);
				camera.projectionMatrix = m;
			} else
				Debug.Log("OffCenterPerspective: Behavior must be attached to a camera");
		} else
			Debug.Log("OffCenterPerspective: you must set a screen transform in the scene");
    }
	
    static Matrix4x4 OffCenter(float left, float right, float bottom, float top, float near, float far) {
        float x = 2.0F * near / (right - left);
        float y = 2.0F * near / (top - bottom);
        float a = (right + left) / (right - left);
        float b = (top + bottom) / (top - bottom);
        float c = -(far + near) / (far - near);
        float d = -(2.0F * far * near) / (far - near);
        float e = -1.0F;
        Matrix4x4 m = new Matrix4x4();
        m[0, 0] = x;
        m[0, 1] = 0;
        m[0, 2] = a;
        m[0, 3] = 0;
        m[1, 0] = 0;
        m[1, 1] = y;
        m[1, 2] = b;
        m[1, 3] = 0;
        m[2, 0] = 0;
        m[2, 1] = 0;
        m[2, 2] = c;
        m[2, 3] = d;
        m[3, 0] = 0;
        m[3, 1] = 0;
        m[3, 2] = e;
        m[3, 3] = 0;
        return m;
    }
}