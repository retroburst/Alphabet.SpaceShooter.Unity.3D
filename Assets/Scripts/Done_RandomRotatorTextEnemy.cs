using UnityEngine;
using System.Collections;

public class Done_RandomRotatorTextEnemy : MonoBehaviour 
{
	public float tumble;
	
	void Start ()
	{
		Vector3 rotation = Random.insideUnitSphere * tumble;
		GetComponent<Rigidbody>().angularVelocity = new Vector3(0, rotation.y, 0);
	}
}