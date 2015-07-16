using UnityEngine;
using System.Collections;
using System;

[System.Serializable]
public class Boundary 
{
	public float xMin, xMax, zMin, zMax;
}

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

	void Start(){
		letters = GameConstants.ALPHABET_LETTERS.ToCharArray();
		configurableSettings = Instantiate (configurationPrefab).GetComponent<ConfigurableSettings> ();
	}

	void Update ()
	{
		if(weaponsLocked && Time.time > weaponsUnlockTime)
		{
			weaponsLocked = false;
			if(WeaponsUnlocked != null) WeaponsUnlocked();
			weaponsUnlockTime = 0.0f;
			wrongTriggerTimes = 0;
		}
		else if(weaponsLocked && Time.time < weaponsUnlockTime)
		{
			return;
		}
		// check for wrong trigger
		if(WrongTriggerPressed() && Time.time > nextFire) {
			wrongTriggerTimes++;
			if(wrongTriggerTimes >= 5)
			{
				weaponsLocked = true;
				weaponsUnlockTime = Time.time + configurableSettings.WeaponsLockTime;
				if(WeaponsLocked != null) WeaponsLocked();
			}
			return;
		}

		if (RightTriggerPressed() && Time.time > nextFire) 
		{
			wrongTriggerTimes = 0;
			nextFire = Time.time + fireRate;
			Instantiate(shot, shotSpawn.position, shotSpawn.rotation);
			GetComponent<AudioSource>().Play ();
			if(ShotFired != null) ShotFired();
		}
	}

	private bool WrongTriggerPressed()
	{
		foreach(char letter in letters)
		{
			if (!string.IsNullOrEmpty(trigger) && letter.ToString() == trigger) continue;
			if (!string.IsNullOrEmpty(trigger) && Input.GetKeyDown(letter.ToString().ToLower())) return true;
		}
		return (false);
	}

	private bool RightTriggerPressed()
	{
		return (((string.IsNullOrEmpty(trigger) && Input.GetButton("Fire1")) || Input.GetKeyDown(trigger.ToLower())));
	}
	
	void FixedUpdate ()
	{
		float moveHorizontal = Input.GetAxis ("Horizontal");
		float moveVertical = Input.GetAxis ("Vertical");

		Vector3 movement = new Vector3 (moveHorizontal, 0.0f, moveVertical);
		GetComponent<Rigidbody>().velocity = movement * speed;
		
		GetComponent<Rigidbody>().position = new Vector3
		(
			Mathf.Clamp (GetComponent<Rigidbody>().position.x, boundary.xMin, boundary.xMax), 
			0.0f, 
			Mathf.Clamp (GetComponent<Rigidbody>().position.z, boundary.zMin, boundary.zMax)
		);
		
		GetComponent<Rigidbody>().rotation = Quaternion.Euler (0.0f, 0.0f, GetComponent<Rigidbody>().velocity.x * -tilt);
	}
}
