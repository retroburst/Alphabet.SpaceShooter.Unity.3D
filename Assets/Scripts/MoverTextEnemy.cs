using UnityEngine;
using System.Collections;

/// <summary>
/// Moves the special case text based enemy.
/// These are the letters that float through space in the game.
/// </summary>
public class MoverTextEnemy : MonoBehaviour
{
	public float speed;
	
	/// <summary>
	/// Start this instance.
	/// </summary>
	private void Start ()
	{
		GetComponent<Rigidbody>().velocity = transform.up * speed;
	}
}
