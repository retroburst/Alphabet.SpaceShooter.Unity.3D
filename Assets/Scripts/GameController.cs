using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class GameController : MonoBehaviour
{
	public GameObject[] hazards;
	public Vector3 spawnValues;
	public GameObject configurationPrefab;
	public GUIText scoreText;
	public GUIText restartText;
	public GUIText gameStatusText;
	public GUIText triggerText;
	public GUIText livesText;
	public GameObject currentPlayer;
	public PlayerController currentPlayerController;
	public GameObject playerPrefab;
	public SettingsController settingsPanel;
	public GameObject AddHighscorePanel;
	public GameObject HighscoresPanel;
	private bool gameOver;
	private bool restart;
	private int livesLeft;
	private Vector3 defaultPlayerPosition;
	private int alphabetIndex = 0;//25;
	private bool playerDestroyed = false;
	private char[] alphabet = null;
	private ConfigurableSettings configurableSettings;
	private Statistics statistics = null;
	private const float ENEMY_MAX_SPEED = 5.0f;
	private bool weaponsLocked = false;
	private bool notStarted = true;
	
	public ConfigurableSettings ConfigurableSettings { get { return configurableSettings; } }

	public Statistics Statistics { get { return statistics; } }

	void Start ()
	{
		statistics = new Statistics ();
		configurableSettings = Instantiate (configurationPrefab).GetComponent<ConfigurableSettings> ();
		defaultPlayerPosition = currentPlayer.transform.position;
		RegisterCurrentPlayer (currentPlayer);
		livesLeft = ConfigurableSettings.InitialLives;
		triggerText.text = string.Empty;
		alphabet = GameConstants.ALPHABET_LETTERS.ToCharArray ();
		gameOver = false;
		restart = false;
		restartText.text = string.Empty;
		statistics.Score = 0;
		UpdateScore ();
		UpdateLives ();
		gameStatusText.text = "Press any key to start...";
	}
	
	void StartCoroutinue ()
	{
		StartCoroutine (SpawnWaves ());
	}
	
	void Update ()
	{
		if (notStarted) {
			if (Input.anyKeyDown) {
				notStarted = false;
				StartCoroutinue ();
			}
		}
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
		currentPlayerController.trigger = alphabet [alphabetIndex].ToString ();
		UpdateTriggerText (alphabet [alphabetIndex]);
		ShowTriggerText ();
		yield return new WaitForSeconds (ConfigurableSettings.StartWait);
		gameStatusText.text = string.Empty;
		while (true) {
			// game won
			if (alphabetIndex == alphabet.Length) {
				gameStatusText.text = string.Format ("You Win!! Your score is {0}.\nYour accuracy was {1}%", statistics.Score, CalculateAccuracy ());
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
					RegisterCurrentPlayer ((GameObject)Instantiate (playerPrefab, defaultPlayerPosition, Quaternion.identity));
					playerDestroyed = false;
					currentPlayerController.trigger = alphabet [alphabetIndex].ToString ();
					UpdateTriggerText (alphabet [alphabetIndex]);
					continue;
				}
				if (weaponsLocked) {
					gameStatusText.text = string.Format ("Oh no, WRONG trigger!\nWeapons system locked.\nWait for unlock...", alphabet [alphabetIndex]);
					yield return new WaitForSeconds (ConfigurableSettings.WeaponsLockTime);
					gameStatusText.text = string.Empty;
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

				if (weaponsLocked) {
					gameStatusText.text = string.Format ("Oh no, WRONG trigger!\nWeapons system locked.\nWait for unlock...", alphabet [alphabetIndex]);
					yield return new WaitForSeconds (ConfigurableSettings.WeaponsLockTime);
					gameStatusText.text = string.Empty;
					continue;
				}

				yield return new WaitForSeconds (ConfigurableSettings.SpawnWait);
			}
			if (!gameOver) {
				alphabetIndex++;
				if (alphabetIndex < alphabet.Length) {
					if (currentPlayer != null)
						currentPlayerController.trigger = alphabet [alphabetIndex].ToString ();
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

	private string CalculateAccuracy ()
	{
		decimal result = 0.00m;
		if (statistics.ShotsFired > 0) {
			int targetsHit = statistics.EnemyAsteroidsDestroyed + statistics.EnemyShipsDestroyed + statistics.EnemyTextsDestroyed;
			result = ((decimal)targetsHit) / ((decimal)statistics.ShotsFired) * 100.00m;
		}
		return(result.ToString ("n2"));
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

	private void RegisterCurrentPlayer (GameObject player)
	{
		currentPlayer = player;
		currentPlayerController = currentPlayer.GetComponent<PlayerController> ();
		currentPlayerController.ShotFired += ShotFiredEventHandler;
		currentPlayerController.WeaponsLocked += WeaponsLockedHandler;
		currentPlayerController.WeaponsUnlocked += WeaponsUnlockedHandler;
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

	private void WeaponsLockedHandler ()
	{
		weaponsLocked = true;
	}

	private void WeaponsUnlockedHandler ()
	{
		weaponsLocked = false;
	}
}