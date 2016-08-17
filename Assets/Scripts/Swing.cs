using UnityEngine;
using System.Collections;

// Swings via rotation and position slerp
public class Swing : MonoBehaviour {

    private float timeSwingStart;
    private Vector3 startPos, startRot;
    private bool swinging;
    private float swingDuration = .4f;
	// Use this for initialization
	void Start ()
    {
        startPos = transform.localPosition;
        startRot = transform.localRotation.eulerAngles;
    }
	
	// Update is called once per frame
	void Update ()
    {
        float time = Time.time;
	    if(Input.GetMouseButton(0))
        {
            if(!swinging)
            {
                swinging = true;
                timeSwingStart = time;
            }


            transform.localPosition = Vector3.Slerp(startPos, startPos + Vector3.forward * 1.3f, (time - timeSwingStart) % swingDuration);
            Vector3 rotAmt = new Vector3(-60, 40, -40);
            transform.localRotation = 
                Quaternion.Euler(Vector3.Slerp(startRot, startRot + rotAmt * 3, (time - timeSwingStart) % swingDuration));
        }
        else
        {
            transform.localPosition = startPos;
            transform.localRotation = Quaternion.Euler(startRot);
        }
	}
}
