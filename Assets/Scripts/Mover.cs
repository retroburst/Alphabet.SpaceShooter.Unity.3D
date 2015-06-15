using UnityEngine;
using System.Collections;

public class Mover : MonoBehaviour {
	private Rigidbody rb = null;
	public float speed = 1.0f;

	// Use this for initialization
	void Start () {
		rb = GetComponent<Rigidbody> ();
		rb.velocity = transform.forward * speed;
	}
}

