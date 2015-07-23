using UnityEngine;
using System.Collections;

/// <summary>
/// Random rotator.
/// Responsible for rotating asteroids in the game.
/// </summary>
public class RandomRotator : MonoBehaviour 
{
	public float tumble;
	
	/// <summary>
	/// Start this instance.
	/// </summary>
	private void Start ()
	{
		GetComponent<Rigidbody>().angularVelocity = Random.insideUnitSphere * tumble;
	}
	
}