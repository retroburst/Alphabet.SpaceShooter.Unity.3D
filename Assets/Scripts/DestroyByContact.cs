using UnityEngine;
using System.Collections;
using System;

public class DestroyByContact : MonoBehaviour
{
	public GameObject explosion;
	public GameObject playerExplosion;
	public int scoreValue;
	private GameController gameController;
	private EnemyType type;

	public event Action<EnemyType, int> EnemyDestroyed;
	public event Action PlayerDestoryed;

	void Start ()
	{
		EnemyTypeTag tag = this.gameObject.GetComponent<EnemyTypeTag> ();
		type = tag.EnemyType;
		GameObject gameControllerObject = GameObject.FindGameObjectWithTag ("GameController");
		if (gameControllerObject != null) {
			gameController = gameControllerObject.GetComponent <GameController> ();
		}
		if (gameController == null) {
			Debug.Log ("Cannot find 'GameController' script");
		}
		gameController.RegisterDestroyByContactForObservation (this);
	}

	void OnTriggerEnter (Collider other)
	{
		if (other.tag == "Boundary" || other.tag == "Enemy") {
			return;
		}

		if (explosion != null) {
			Instantiate (explosion, transform.position, transform.rotation);
		}

		if (other.tag == "Player") {
			Instantiate (playerExplosion, other.transform.position, other.transform.rotation);
			if (PlayerDestoryed != null)
				PlayerDestoryed ();
		}

		if (EnemyDestroyed != null)
			EnemyDestroyed (type, scoreValue);

		Destroy (other.gameObject);
		Destroy (gameObject);
	}
}