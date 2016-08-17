using UnityEngine;
using System.Collections;

public class FallOffProtection : MonoBehaviour
{
    public Vector3 respawnPosition;
    public float minYLimit;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
        if (transform.position.y < minYLimit) transform.position = respawnPosition;
	}
}
