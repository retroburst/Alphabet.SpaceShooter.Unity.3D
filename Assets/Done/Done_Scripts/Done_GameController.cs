using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Done_GameController : MonoBehaviour
{
	public GameObject[] hazards;
	public Vector3 spawnValues;
	public int hazardCount;
	public float spawnWait;
	public float startWait;
	public float waveWait;
	public int numberOfInitialLetters;
	public int lives;
	public int playerDestroyedWaitTime;
	public GUIText scoreText;
	public GUIText restartText;
	public GUIText gameStatusText;
	public GUIText triggerText;
	public GUIText livesText;
	public GameObject currentPlayer;
	public GameObject playerPrefab;
	private bool gameOver;
	private bool restart;
	private int score;
	private int livesLeft;
	private Vector3 defaultPlayerPosition;
	private int alphabetIndex = 0;
	private bool playerDestroyed = false;
	private string letters = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
	private char[] alphabet = null;
	
	void Start ()
	{
		defaultPlayerPosition = currentPlayer.transform.position;
		livesLeft = lives;
		triggerText.text = string.Empty;
		alphabet = letters.ToCharArray ();
		gameOver = false;
		restart = false;
		restartText.text = string.Empty;
		gameStatusText.text = string.Empty;
		score = 0;
		UpdateScore ();
		StartCoroutine (SpawnWaves ());
	}
	
	void Update ()
	{
		if (restart) {
			if (Input.GetKeyDown (KeyCode.Space)) {
				Application.LoadLevel (Application.loadedLevel);
			}
		}
	}
	
	IEnumerator SpawnWaves ()
	{
		UpdateLives ();
		currentPlayer.GetComponent<Done_PlayerController> ().trigger = alphabet [alphabetIndex].ToString ();
		UpdateTriggerText (alphabet [alphabetIndex]);
		ShowTriggerText ();
		yield return new WaitForSeconds (startWait);
		gameStatusText.text = string.Empty;
		while (true) {
			// game won
			if (alphabetIndex == alphabet.Length)
			{
				gameStatusText.text = string.Format ("You Win!! Your score is {0}.", score);
				restartText.text = "Press 'Space' for Restart";
				restart = true;
				break;
			}
			for (int i = 0; i < hazardCount; i++) {
				if (gameOver) {
					restartText.text = "Press 'Space' for Restart";
					restart = true;
					break;
				}
				if (playerDestroyed) {
					gameStatusText.text = string.Format ("You were destroyed.\nTrigger is still '{0}'.\nPlease wait...", alphabet [alphabetIndex]);
					yield return new WaitForSeconds (playerDestroyedWaitTime);
					gameStatusText.text = string.Empty;
					currentPlayer = (GameObject)Instantiate (playerPrefab, defaultPlayerPosition, Quaternion.identity);
					currentPlayer.GetComponent<Done_PlayerController> ().trigger = alphabet [alphabetIndex].ToString ();
					playerDestroyed = false;
					continue;
				}

				GameObject hazard = null;
				if (i >= numberOfInitialLetters) {
					hazard = hazards [Random.Range (0, hazards.Length)];
				} else {
					hazard = hazards [hazards.Length - 1];
				}

				Vector3 spawnPosition = new Vector3 (Random.Range (-spawnValues.x, spawnValues.x), spawnValues.y, spawnValues.z);
				Quaternion spawnRotation = Quaternion.identity;
				GameObject newHazard = (GameObject)Instantiate (hazard, spawnPosition, spawnRotation);

				if (hazard == hazards [hazards.Length - 1]) {
					TextMesh textMesh = newHazard.transform.FindChild ("Letter").GetComponent<TextMesh> ();
					textMesh.text = string.Format ("{0}", alphabet [alphabetIndex]);
				}

				yield return new WaitForSeconds (spawnWait);
			}
			if (!gameOver) {
				alphabetIndex++;
				if (currentPlayer != null)
					currentPlayer.GetComponent<Done_PlayerController> ().trigger = alphabet [alphabetIndex].ToString ();
				UpdateTriggerText (alphabet [alphabetIndex]);
				ShowTriggerText ();
				yield return new WaitForSeconds (waveWait);
				gameStatusText.text = string.Empty;
			} else {
				restartText.text = "Press 'Space' for Restart";
				restart = true;
				break;
			}
		}
	}

	void UpdateTriggerText (char ch)
	{
		triggerText.text = string.Format ("Trigger ['{0}']", ch);
	}
	
	public void AddScore (int newScoreValue)
	{
		score += newScoreValue;
		UpdateScore ();
	}
	
	void UpdateScore ()
	{
		scoreText.text = string.Format ("Score//{0}", score.ToString ("00000000"));
	}

	void UpdateLives ()
	{
		livesText.text = string.Format ("Lives//{0}", livesLeft.ToString ("00"));
	}

	void ShowTriggerText ()
	{
		gameStatusText.text = string.Format ("Trigger is now '{0}'", alphabet [alphabetIndex]);
	}
	
	public void PlayerDestroyed ()
	{
		livesLeft--;
		UpdateLives (); 
		if (livesLeft <= 0) {
			gameStatusText.text = "Game Over!";
			gameOver = true;
		} else {
			playerDestroyed = true;
		}
	}
}