using UnityEngine;
using System.Collections;

public class Done_MoverTextEnemy : MonoBehaviour
{
	public float speed;

	void Start ()
	{
		GetComponent<Rigidbody>().velocity = transform.up * speed;
	}
}
