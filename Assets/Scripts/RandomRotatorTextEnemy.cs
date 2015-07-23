using UnityEngine;
using System.Collections;

/// <summary>
/// Random rotator of the text enemy.
/// </summary>
public class RandomRotatorTextEnemy : MonoBehaviour 
{
	public float tumble;
	
	/// <summary>
	/// Start this instance.
	/// </summary>
	private void Start ()
	{
		Vector3 rotation = Random.insideUnitSphere * tumble;
		GetComponent<Rigidbody>().angularVelocity = new Vector3(0, rotation.y, 0);
	}
	
}