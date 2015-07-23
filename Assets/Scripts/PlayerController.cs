using UnityEngine;
using System.Collections;
using System;

/// <summary>
/// Represents a boundary.
/// </summary>
[System.Serializable]
public class Boundary
{
	public float xMin, xMax, zMin, zMax;
}

/// <summary>
/// Player controller, responsible
// for game play, score caculation,
/// spawning enemies etc.
/// </summary>
public class PlayerController : MonoBehaviour
{
	private char[] letters;
	public float speed;
	public float tilt;
	public Boundary boundary;
	public string trigger;
	public GameObject configurationPrefab;
	private ConfigurableSettings configurableSettings;
	public GameObject shot;
	public Transform shotSpawn;
	public float fireRate;
	private float nextFire;
	private float weaponsUnlockTime;
	private bool weaponsLocked = false;
	private int wrongTriggerTimes = 0;

	public event Action ShotFired;
	public event Action WeaponsLocked;
	public event Action WeaponsUnlocked;
	
	/// <summary>
	/// Start this instance.
	/// </summary>
	private void Start ()
	{
		letters = Constants.ALPHABET_LETTERS.ToCharArray ();
		configurableSettings = Instantiate (configurationPrefab).GetComponent<ConfigurableSettings> ();
	}

	/// <summary>
	/// Update this instance.
	/// </summary>
	private void Update ()
	{
		if (weaponsLocked && Time.time >= weaponsUnlockTime) {
			weaponsLocked = false;
			if (WeaponsUnlocked != null) {
				WeaponsUnlocked ();
			}
			weaponsUnlockTime = 0.0f;
			wrongTriggerTimes = 0;
		} else if (weaponsLocked && Time.time < weaponsUnlockTime) {
			return;
		}
		// check for wrong trigger presses
		if (ProcessWrongTriggerPress ()) {
			return;
		}
		// if right trigger pressed
		ProcessTriggerPress ();
	}
	
	/// <summary>
	/// Processes the trigger press.
	/// </summary>
	private void ProcessTriggerPress ()
	{
		if (RightTriggerPressed () && Time.time > nextFire) {
			wrongTriggerTimes = 0;
			nextFire = Time.time + fireRate;
			Instantiate (shot, shotSpawn.position, shotSpawn.rotation);
			GetComponent<AudioSource> ().Play ();
			if (ShotFired != null) {
				ShotFired ();
			}
		}
	}
	
	/// <summary>
	/// Processes the wrong trigger press.
	/// </summary>
	/// <returns><c>true</c>, if wrong trigger press was processed, <c>false</c> otherwise.</returns>
	private bool ProcessWrongTriggerPress ()
	{
		if (WrongTriggerPressed () && Time.time > nextFire) {
			wrongTriggerTimes++;
			if (wrongTriggerTimes >= 5) {
				weaponsLocked = true;
				weaponsUnlockTime = Time.time + configurableSettings.WeaponsLockTime;
				if (WeaponsLocked != null) {
					WeaponsLocked ();
				}
			}
			return(true);
		}
		return(false);
	}
	
	/// <summary>
	/// Was wrong trigger pressed.
	/// </summary>
	/// <returns><c>true</c>, if trigger pressed was wronged, <c>false</c> otherwise.</returns>
	private bool WrongTriggerPressed ()
	{
		foreach (char letter in letters) {
			if (!string.IsNullOrEmpty (trigger) && letter.ToString () == trigger) {
				continue;
			}
			if (!string.IsNullOrEmpty (trigger) && Input.GetKeyDown (letter.ToString ().ToLower ())) {
				return (true);
			}
		}
		return (false);
	}
	
	/// <summary>
	/// Was rights trigger pressed.
	/// </summary>
	/// <returns><c>true</c>, if trigger pressed was righted, <c>false</c> otherwise.</returns>
	private bool RightTriggerPressed ()
	{
		return (((string.IsNullOrEmpty (trigger) && Input.GetButton (Constants.INPUT_FIRE)) || Input.GetKeyDown (trigger.ToLower ())));
	}
	
	/// <summary>
	/// Fixed update.
	/// </summary>
	private void FixedUpdate ()
	{
		float moveHorizontal = Input.GetAxis (Constants.AXIS_HORIZONTAL);
		float moveVertical = Input.GetAxis (Constants.AXIS_VERTICAL);

		Vector3 movement = new Vector3 (moveHorizontal, 0.0f, moveVertical);
		GetComponent<Rigidbody> ().velocity = movement * speed;
		
		GetComponent<Rigidbody> ().position = new Vector3
		(
			Mathf.Clamp (GetComponent<Rigidbody> ().position.x, boundary.xMin, boundary.xMax), 
			0.0f, 
			Mathf.Clamp (GetComponent<Rigidbody> ().position.z, boundary.zMin, boundary.zMax)
		);
		
		GetComponent<Rigidbody> ().rotation = Quaternion.Euler (0.0f, 0.0f, GetComponent<Rigidbody> ().velocity.x * -tilt);
	}

}
