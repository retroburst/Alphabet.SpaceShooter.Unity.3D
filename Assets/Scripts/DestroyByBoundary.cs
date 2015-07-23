using UnityEngine;
using System.Collections;

/// <summary>
/// Destroy by boundary.
/// </summary>
public class DestroyByBoundary : MonoBehaviour
{
	/// <summary>
	/// Handles the trigger exit event.
	/// </summary>
	/// <param name="other">Other.</param>
	private void OnTriggerExit (Collider other)
	{
		Destroy (other.gameObject);
	}
}