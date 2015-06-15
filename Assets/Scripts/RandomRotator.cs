using UnityEngine;
using System.Collections;

public class RandomRotator : MonoBehaviour {
	private Rigidbody rb = null;
	public float tumble = 1.0f;

	// Use this for initialization
	void Start () {
		rb = GetComponent<Rigidbody> ();
		rb.angularVelocity = Random.insideUnitSphere * tumble;

	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
