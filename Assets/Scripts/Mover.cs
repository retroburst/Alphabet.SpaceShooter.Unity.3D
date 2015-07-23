using UnityEngine;
using System.Collections;

/// <summary>
/// Moves game objects with rigid bodies.
/// </summary>
public class Mover : MonoBehaviour
{
	public float speed;
	
	/// <summary>
	/// Start this instance.
	/// </summary>
	private void Start ()
	{
		GetComponent<Rigidbody>().velocity = transform.forward * speed;
	}
	
}
