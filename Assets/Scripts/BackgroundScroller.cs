using UnityEngine;
using System.Collections;

/// <summary>
/// Background scroller.
/// Responsible for scrolling the background image
/// for the game.
/// </summary>
public class BackgroundScroller : MonoBehaviour
{
	public float scrollSpeed;
	public float tileSizeZ;
	private Vector3 startPosition;
	
	/// <summary>
	/// Start this instance.
	/// </summary>
	private void Start ()
	{
		startPosition = transform.position;
	}
	
	/// <summary>
	/// Update this instance.
	/// </summary>
	private void Update ()
	{
		float newPosition = Mathf.Repeat (Time.time * scrollSpeed, tileSizeZ);
		transform.position = startPosition + Vector3.forward * newPosition;
	}
	
}