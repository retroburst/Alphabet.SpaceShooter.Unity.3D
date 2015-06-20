using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class GameController : MonoBehaviour
{
	private const string letters = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
	public GameObject[] hazards;
	public Vector3 spawnValues;
	public GameObject configurationPrefab;
	public GUIText scoreText;
	public GUIText restartText;
	public GUIText gameStatusText;
	public GUIText triggerText;
	public GUIText livesText;
	public GameObject currentPlayer;
	public GameObject playerPrefab;
	public SettingsController settingsPanel;
	public GameObject AddHighscorePanel;
	public GameObject HighscoresPanel;
	private bool gameOver;
	private bool restart;
	private int livesLeft;
	private Vector3 defaultPlayerPosition;
	private int alphabetIndex = 0; //25;
	private bool playerDestroyed = false;
	private char[] alphabet = null;
	private ConfigurableSettings configurableSettings;
	private Statistics statistics = null;
	private const int ENEMY_MAX_SPEED = 6;
	
	public ConfigurableSettings ConfigurableSettings { get { return configurableSettings; } }

	public Statistics Statistics { get { return statistics; } }

	void Start ()
	{
		statistics = new Statistics ();
		configurableSettings = Instantiate (configurationPrefab).GetComponent<ConfigurableSettings> ();
		defaultPlayerPosition = currentPlayer.transform.position;
		currentPlayer.GetComponent<PlayerController> ().ShotFired += ShotFiredEventHandler;
		livesLeft = ConfigurableSettings.InitialLives;
		triggerText.text = string.Empty;
		alphabet = letters.ToCharArray ();
		gameOver = false;
		restart = false;
		restartText.text = string.Empty;
		gameStatusText.text = string.Empty;
		statistics.Score = 0;
		UpdateScore ();
		StartCoroutine (SpawnWaves ());
	}
	
	void Update ()
	{
		if (Input.GetButtonDown ("Restart"))
			Application.LoadLevel (Application.loadedLevel);
		if (Input.GetButtonDown ("Highscores")) {
			HighscoresPanel.GetComponent<HighscoresController> ().UpdateHighscores ();
			HighscoresPanel.SetActive (true);
		}
		if (Input.GetButton ("Settings"))
			settingsPanel.OpenSettingsPanel ();
		if (restart) {
			if (Input.GetKeyDown (KeyCode.Escape)) {
				Application.LoadLevel (Application.loadedLevel);
			}
		}
	}

	public void RestartGame ()
	{
		Application.LoadLevel (Application.loadedLevel);
	}
	
	private IEnumerator SpawnWaves ()
	{
		UpdateLives ();
		currentPlayer.GetComponent<PlayerController> ().trigger = alphabet [alphabetIndex].ToString ();
		UpdateTriggerText (alphabet [alphabetIndex]);
		ShowTriggerText ();
		yield return new WaitForSeconds (ConfigurableSettings.StartWait);
		gameStatusText.text = string.Empty;
		while (true) {
			// game won
			if (alphabetIndex == alphabet.Length) {
				gameStatusText.text = string.Format ("You Win!! Your score is {0}.", statistics.Score);
				yield return new WaitForSeconds (2);
				AddHighscorePanel.SetActive (true);
				restartText.text = "Press 'Esc' for Restart";
				restart = true;
				break;
			}
			for (int i = 0; i < ConfigurableSettings.hazardCount; i++) {
				if (gameOver) {
					restartText.text = "Press 'Esc' for Restart";
					restart = true;
					break;
				}
				if (playerDestroyed) {
					gameStatusText.text = string.Format ("You were destroyed.\nTrigger is still '{0}'.\nPlease wait...", alphabet [alphabetIndex]);
					yield return new WaitForSeconds (ConfigurableSettings.PlayerDestroyedWaitTime);
					gameStatusText.text = string.Empty;
					currentPlayer = (GameObject)Instantiate (playerPrefab, defaultPlayerPosition, Quaternion.identity);
					currentPlayer.GetComponent<PlayerController> ().trigger = alphabet [alphabetIndex].ToString ();
					currentPlayer.GetComponent<PlayerController> ().ShotFired += ShotFiredEventHandler;
					playerDestroyed = false;
					continue;
				}

				GameObject hazard = null;
				if (i >= ConfigurableSettings.NumberOfInitialLetters) {
					hazard = hazards [UnityEngine.Random.Range (0, hazards.Length)];
				} else {
					hazard = hazards [hazards.Length - 1];
				}

				Vector3 spawnPosition = new Vector3 (UnityEngine.Random.Range (-spawnValues.x, spawnValues.x), spawnValues.y, spawnValues.z);
				Quaternion spawnRotation = Quaternion.identity;
				GameObject newHazard = (GameObject)Instantiate (hazard, spawnPosition, spawnRotation);
				SetSpeed (newHazard, hazard, alphabetIndex);

				if (hazard == hazards [hazards.Length - 1]) {
					TextMesh textMesh = newHazard.transform.FindChild ("Letter").GetComponent<TextMesh> ();
					textMesh.text = string.Format ("{0}", alphabet [alphabetIndex]);
				}

				yield return new WaitForSeconds (ConfigurableSettings.SpawnWait);
			}
			if (!gameOver) {
				alphabetIndex++;
				if (alphabetIndex < alphabet.Length) {
					if (currentPlayer != null)
						currentPlayer.GetComponent<PlayerController> ().trigger = alphabet [alphabetIndex].ToString ();
					UpdateTriggerText (alphabet [alphabetIndex]);
					ShowTriggerText ();
				}
				yield return new WaitForSeconds (ConfigurableSettings.WaveWait);
				gameStatusText.text = string.Empty;
			} else {
				restartText.text = "Press 'Esc' for Restart";
				restart = true;
				break;
			}
		}
	}
	
	void SetSpeed (GameObject newHazard, GameObject prefab, int alphabetIndex)
	{ 
		if (prefab == hazards [hazards.Length - 1]) {
			MoverTextEnemy mover = newHazard.transform.FindChild ("Letter").GetComponent<MoverTextEnemy> ();
			float speed = Math.Abs (mover.speed);
			float speedIncrement = ((ENEMY_MAX_SPEED - speed) / alphabet.Length);
			mover.speed -= (speedIncrement * alphabetIndex);
			mover = null;
		} else {
			Mover mover = newHazard.GetComponent<Mover> ();
			float speed = Math.Abs (mover.speed);
			float speedIncrement = ((ENEMY_MAX_SPEED - speed) / alphabet.Length);
			mover.speed -= (speedIncrement * alphabetIndex);
			mover = null;
		}
	}

	void UpdateTriggerText (char ch)
	{
		triggerText.text = string.Format ("Trigger ['{0}']", ch);
	}
	
	private void AddScore (int newScoreValue)
	{
		statistics.Score += newScoreValue;
		UpdateScore ();
	}
	
	void UpdateScore ()
	{
		scoreText.text = string.Format ("Score//{0}", statistics.Score.ToString ("00000000"));
	}

	void UpdateLives ()
	{
		livesText.text = string.Format ("Lives//{0}", livesLeft.ToString ("00"));
	}

	void ShowTriggerText ()
	{
		gameStatusText.text = string.Format ("Trigger is now '{0}'", alphabet [alphabetIndex]);
	}
	
	private void PlayerDestroyed ()
	{
		livesLeft--;
		statistics.TimesPlayerDestroyed++;
		UpdateLives (); 
		if (livesLeft <= 0) {
			gameStatusText.text = "Game Over!";
			gameOver = true;
		} else {
			playerDestroyed = true;
		}
	}

	private void ShotFiredEventHandler ()
	{
		statistics.ShotsFired++;
	}

	private void EnemyDestroyed (EnemyType type, int scoreVaue)
	{
		switch (type) {
		case EnemyType.Ship:
			statistics.EnemyShipsDestroyed++;
			break;
		case EnemyType.Asteroid:
			statistics.EnemyAsteroidsDestroyed++;
			break;
		case EnemyType.Text:
			statistics.EnemyTextsDestroyed++;
			break;
		}
		AddScore (scoreVaue);
	}

	public void RegisterDestroyByContactForObservation (DestroyByContact dbc)
	{
		dbc.PlayerDestoryed += PlayerDestroyed;
		dbc.EnemyDestroyed += EnemyDestroyed;
	}



}