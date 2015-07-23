using UnityEngine;
using System.Collections;

/// <summary>
/// Weapons controller for enemy ships.
/// </summary>
public class WeaponController : MonoBehaviour
{
	public GameObject shot;
	public Transform shotSpawn;
	public float fireRate;
	public float delay;
	
	/// <summary>
	/// Start this instance.
	/// </summary>
	private void Start ()
	{
		InvokeRepeating ("Fire", delay, fireRate);
	}

	/// <summary>
	/// Fire this instance.
	/// </summary>
	private void Fire ()
	{
		Instantiate (shot, shotSpawn.position, shotSpawn.rotation);
		GetComponent<AudioSource> ().Play ();
	}
	
}
