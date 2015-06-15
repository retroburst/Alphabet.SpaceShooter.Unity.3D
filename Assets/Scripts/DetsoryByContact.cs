using UnityEngine;
using System.Collections;

public class DetsoryByContact : MonoBehaviour {
	public GameObject explosion = null;
	public GameObject playerExplosion = null;
	public int points = 10;
	private GameController gameController = null;

	void Start(){
		GameObject gameControllerGameObject = GameObject.FindGameObjectWithTag ("GameController");
		if (gameControllerGameObject != null) {
			gameController = gameControllerGameObject.GetComponent<GameController>();
			if(gameController == null) Debug.Log("Could not get a reference to GameController script.");
		}
	}

	void OnTriggerEnter(Collider otherCollider){
		Debug.Log ("otherCollider name is: " + otherCollider.name);
		if (otherCollider.tag == "Boundary") {
			return;
		}
		Instantiate (explosion, transform.position, transform.rotation);
		if(otherCollider.tag == "Player"){
			gameController.PlayerDestroyed();
			Instantiate(playerExplosion, otherCollider.transform.position, otherCollider.transform.rotation);
		}
		Destroy(otherCollider.gameObject);
		if (gameObject.transform.parent != null) {
			Destroy (transform.parent.gameObject);
		}
		Destroy (gameObject);
		gameController.AddScore (points);
	}

}
