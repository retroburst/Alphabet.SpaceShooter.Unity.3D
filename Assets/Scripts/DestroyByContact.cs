using UnityEngine;
using System.Collections;
using System;

/// <summary>
/// Destroy by contact.
/// </summary>
public class DestroyByContact : MonoBehaviour
{
	public GameObject explosion;
	public GameObject playerExplosion;
	public int scoreValue;
	private GameController gameController;
	private EnemyType type;
	public event Action<EnemyType, int> EnemyDestroyed;
	public event Action PlayerDestoryed;
	
	/// <summary>
	/// Start this instance.
	/// </summary>
	private void Start ()
	{
		EnemyTypeTag tag = this.gameObject.GetComponent<EnemyTypeTag> ();
		type = tag.EnemyType;
		GameObject gameControllerObject = GameObject.FindGameObjectWithTag (Constants.TAG_GAME_CONTROLLER);
		if (gameControllerObject != null) {
			gameController = gameControllerObject.GetComponent <GameController> ();
		}
		if (gameController == null) {
			Debug.LogError ("Cannot find 'GameController' script");
		}
		gameController.RegisterDestroyByContactForObservation (this);
	}
	
	/// <summary>
	/// Handles the trigger enter event.
	/// </summary>
	/// <param name="other">Other.</param>
	private void OnTriggerEnter (Collider other)
	{
		if (other.tag == Constants.TAG_BOUNDARY || other.tag == Constants.TAG_ENEMY) {
			return;
		}
		if (explosion != null) {
			Instantiate (explosion, transform.position, transform.rotation);
		}
		if (other.tag == Constants.TAG_PLAYER) {
			Instantiate (playerExplosion, other.transform.position, other.transform.rotation);
			if (PlayerDestoryed != null) {
				PlayerDestoryed ();
			}
		}
		if (EnemyDestroyed != null) {
			EnemyDestroyed (type, scoreValue);
		}
		Destroy (other.gameObject);
		Destroy (gameObject);
	}
	
}