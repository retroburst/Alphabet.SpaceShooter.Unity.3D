using UnityEngine;
using System.Collections;

public class BoundaryController : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void OnTriggerExit(Collider otherCollider){
		if(otherCollider.transform.parent != null){
			Destroy (otherCollider.transform.parent.gameObject);
			return;
		}
		Destroy (otherCollider.gameObject);
	}
}
