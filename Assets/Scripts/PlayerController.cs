using UnityEngine;
using System.Collections;

[System.Serializable]
public class Boundary{
	public float xMin;
	public float xMax;
	public float zMin;
	public float zMax;
}

public class PlayerController : MonoBehaviour {
	private Rigidbody rb = null;
	private AudioSource audioWeaponFire = null;
	public float Speed = 0.0f;
	public Boundary boundary = null;
	public float tilt = 0.0f;
	public GameObject shot = null;
	public Transform shotSpawn = null;
	private float nextFire = 0.0f;
	public float fireRate = 0.5f;

	void Start()
	{
		rb = GetComponent<Rigidbody> ();
		audioWeaponFire = GetComponent<AudioSource> ();
	}

	void Update()
	{
		if (Input.GetButton ("Fire1") && Time.time > nextFire) {
			nextFire = Time.time + fireRate;
			Instantiate (shot, shotSpawn.transform.position, shotSpawn.transform.rotation);
			audioWeaponFire.Play ();
		}
	}

	void FixedUpdate () {
		float moveHorizontal = Input.GetAxis ("Horizontal");
		float moveVertical = Input.GetAxis ("Vertical");
		rb.velocity = (new Vector3 (moveHorizontal, 0.0f, moveVertical) * Speed);
		float newX = Mathf.Clamp (rb.position.x, boundary.xMin, boundary.xMax);
		float newY = Mathf.Clamp (rb.position.z, boundary.zMin, boundary.zMax);
		rb.position = new Vector3 (newX, 0.0f, newY);
		rb.rotation = Quaternion.Euler(new Vector3(0.0f, 0.0f, rb.velocity.x * -tilt));

	}
}
