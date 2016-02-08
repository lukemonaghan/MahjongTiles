using UnityEngine;

public class MouseOrbit : MonoBehaviour
{
    public float speed;

	void Update () 
    {
        if (Input.GetKey(KeyCode.LeftAlt) && (Input.GetMouseButton(0) || Input.GetMouseButton(1)))
	    {
            transform.RotateAround(Vector3.zero, transform.up, Input.GetAxis("Mouse X") * speed);
            //transform.RotateAround(Vector3.zero, transform.right, Input.GetAxis("Mouse Y") * speed);
	        //if (transform.position.y < 0.0f)
	        //{
	        //    transform.position = new Vector3(transform.position.x, 1, transform.position.z);
	        //}
            transform.LookAt(Vector3.zero);
        }
	}

    float ClampAngle(float angle, float min,float max)
    {
        while (angle < 0)
            angle += 360;
        while (angle > 360)
            angle -= 360;
         angle = Mathf.Clamp(angle, min, max);
         return angle;
    }
}
