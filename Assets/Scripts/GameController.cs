using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

/// <summary>
/// Game controller.
/// Responsible for controlling game
/// play, spawning enemies,
/// calculating score etc.
/// </summary>
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
	
	/// <summary>
	/// Gets the configurable settings.
	/// </summary>
	/// <value>The configurable settings.</value>
	public ConfigurableSettings ConfigurableSettings { get { return configurableSettings; } }
	
	/// <summary>
	/// Gets the statistics.
	/// </summary>
	/// <value>The statistics.</value>
	public Statistics Statistics { get { return statistics; } }
	
	/// <summary>
	/// Start this instance.
	/// </summary>
	private void Start ()
	{
		statistics = new Statistics ();
		configurableSettings = Instantiate (configurationPrefab).GetComponent<ConfigurableSettings> ();
		defaultPlayerPosition = currentPlayer.transform.position;
		RegisterCurrentPlayer (currentPlayer);
		livesLeft = ConfigurableSettings.InitialLives;
		triggerText.text = string.Empty;
		alphabet = Constants.ALPHABET_LETTERS.ToCharArray ();
		gameOver = false;
		restart = false;
		restartText.text = string.Empty;
		statistics.Score = 0;
		UpdateScore ();
		UpdateLives ();
		gameStatusText.text = "Press any key to start...";
	}
	
	/// <summary>
	/// Update this instance.
	/// </summary>
	private void Update ()
	{
		if (notStarted) {
			if (Input.anyKeyDown) {
				notStarted = false;
				StartCoroutine (SpawnWaves ());
			}
		}
		if (Input.GetButtonDown (Constants.INPUT_RESTART))
			Application.LoadLevel (Application.loadedLevel);
		if (Input.GetButtonDown (Constants.INPUT_HIGHSCORES)) {
			HighscoresPanel.GetComponent<HighscoresController> ().UpdateHighscores ();
			HighscoresPanel.SetActive (true);
		}
		if (Input.GetButton (Constants.INPUT_SETTINGS))
			settingsPanel.OpenSettingsPanel ();
		if (restart) {
			if (Input.GetKeyDown (KeyCode.Escape)) {
				Application.LoadLevel (Application.loadedLevel);
			}
		}
	}
	
	/// <summary>
	/// Restarts the game.
	/// </summary>
	public void RestartGame ()
	{
		Application.LoadLevel (Application.loadedLevel);
	}
	
	/// <summary>
	/// Exits the game.
	/// </summary>
	public void ExitGame ()
	{
		Application.Quit ();
	}
	
	/// <summary>
	/// Spawns the waves.
	/// </summary>
	/// <returns>The waves.</returns>
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
				ProcessGameWin ();
				break;
			}
			for (int i = 0; i < ConfigurableSettings.hazardCount; i++) {
				if (gameOver) {
					ProcessGameOver ();
					break;
				}
				
				if (playerDestroyed) {
					gameStatusText.text = "You were destroyed.\nPlease wait...";
					yield return new WaitForSeconds (ConfigurableSettings.PlayerDestroyedWaitTime);
					gameStatusText.text = string.Empty;
					ProcessPlayerDestroyed ();
					continue;
				}
				if (weaponsLocked) {
					gameStatusText.text = string.Format ("Oh no, WRONG trigger!\nWeapons system locked.\nWait for unlock...", alphabet [alphabetIndex]);
					yield return new WaitForSeconds (ConfigurableSettings.WeaponsLockTime);
					gameStatusText.text = string.Empty;
					continue;
				}
				SpawnHazard (i);
				yield return new WaitForSeconds (ConfigurableSettings.SpawnWait);
			}
			if (!gameOver) {
				UpdateAlphabetPosition ();
				yield return new WaitForSeconds (ConfigurableSettings.WaveWait);
				gameStatusText.text = string.Empty;
			} else {
				ProcessGameOver ();
				break;
			}
		}
	}
	
	/// <summary>
	/// Spawns the hazard.
	/// </summary>
	/// <param name="hazardNumber">Hazard number.</param>
	private void SpawnHazard (int hazardNumber)
	{
		bool isTextEnemy = false;
		GameObject hazard = null;
		if (hazardNumber >= ConfigurableSettings.NumberOfInitialLetters) {
			hazard = hazards [UnityEngine.Random.Range (0, hazards.Length)];
		} else {
			hazard = hazards [hazards.Length - 1];
			isTextEnemy = true;
		}
		Vector3 spawnPosition = new Vector3 (UnityEngine.Random.Range (-spawnValues.x, spawnValues.x), spawnValues.y, spawnValues.z);
		Quaternion spawnRotation = Quaternion.identity;
		GameObject newHazard = (GameObject)Instantiate (hazard, spawnPosition, spawnRotation);
		SetSpeed (newHazard, hazard, alphabetIndex);
		if (isTextEnemy) {
			SetupTextEnemyMesh (newHazard);
		}
	}
	
	/// <summary>
	/// Sets up the text enemy mesh.
	/// </summary>
	/// <param name="newHazard">New hazard.</param>
	private void SetupTextEnemyMesh (GameObject newHazard)
	{
		TextMesh textMesh = newHazard.transform.FindChild ("Letter").GetComponent<TextMesh> ();
		textMesh.text = string.Format ("{0}", alphabet [alphabetIndex]);
	}
	
	/// <summary>
	/// Updates the alphabet position.
	/// </summary>
	private void UpdateAlphabetPosition ()
	{
		alphabetIndex++;
		if (alphabetIndex < alphabet.Length) {
			if (currentPlayer != null) {
				currentPlayerController.trigger = alphabet [alphabetIndex].ToString ();
			}
			UpdateTriggerText (alphabet [alphabetIndex]);
			ShowTriggerText ();
		}
	}
	
	/// <summary>
	/// Processes the player destroyed event.
	/// </summary>
	private void ProcessPlayerDestroyed ()
	{
		RegisterCurrentPlayer ((GameObject)Instantiate (playerPrefab, defaultPlayerPosition, Quaternion.identity));
		playerDestroyed = false;
		currentPlayerController.trigger = alphabet [alphabetIndex].ToString ();
		UpdateTriggerText (alphabet [alphabetIndex]);
	}
	
	/// <summary>
	/// Processes the game over event.
	/// </summary>
	private void ProcessGameOver ()
	{
		restartText.text = "Press 'Esc' for Restart";
		restart = true;
	}

	/// <summary>
	/// Processes the game won event.
	/// </summary>
	private void ProcessGameWin ()
	{
		AddHighscorePanel.SetActive (true);
		restartText.text = "Press 'Esc' for Restart";
		restart = true;
	}
	
	/// <summary>
	/// Calculates the accuracy.
	/// </summary>
	/// <returns>The accuracy.</returns>
	private string CalculateAccuracy ()
	{
		decimal result = 0.00m;
		if (statistics.ShotsFired > 0) {
			int targetsHit = statistics.EnemyAsteroidsDestroyed + statistics.EnemyShipsDestroyed + statistics.EnemyTextsDestroyed;
			result = ((decimal)targetsHit) / ((decimal)statistics.ShotsFired) * 100.00m;
		}
		return(result.ToString ("n2"));
	}
	
	/// <summary>
	/// Sets the speed of enemies as the game progresses.
	/// </summary>
	/// <param name="newHazard">New hazard.</param>
	/// <param name="prefab">Prefab.</param>
	/// <param name="alphabetIndex">Alphabet index.</param>
	private void SetSpeed (GameObject newHazard, GameObject prefab, int alphabetIndex)
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
	
	/// <summary>
	/// Updates the trigger text.
	/// </summary>
	/// <param name="ch">Ch.</param>
	private void UpdateTriggerText (char ch)
	{
		triggerText.text = string.Format ("Trigger ['{0}']", ch);
	}
	
	/// <summary>
	/// Registers the current player.
	/// </summary>
	/// <param name="player">Player.</param>
	private void RegisterCurrentPlayer (GameObject player)
	{
		currentPlayer = player;
		currentPlayerController = currentPlayer.GetComponent<PlayerController> ();
		currentPlayerController.ShotFired += ShotFiredEventHandler;
		currentPlayerController.WeaponsLocked += WeaponsLockedEventHandler;
		currentPlayerController.WeaponsUnlocked += WeaponsUnlockedEventHandler;
	}
	
	/// <summary>
	/// Adds to the score.
	/// </summary>
	/// <param name="newScoreValue">New score value.</param>
	private void AddScore (int newScoreValue)
	{
		statistics.Score += newScoreValue;
		UpdateScore ();
	}
	
	/// <summary>
	/// Updates the score.
	/// </summary>
	private void UpdateScore ()
	{
		scoreText.text = string.Format ("Score//{0}", statistics.Score.ToString ("00000000"));
	}
	
	/// <summary>
	/// Updates the lives.
	/// </summary>
	private void UpdateLives ()
	{
		livesText.text = string.Format ("Lives//{0}", livesLeft.ToString ("00"));
	}
	
	/// <summary>
	/// Shows the trigger text.
	/// </summary>
	private void ShowTriggerText ()
	{
		gameStatusText.text = string.Format ("Trigger is now '{0}'", alphabet [alphabetIndex]);
	}
	
	/// <summary>
	/// Player destroyed handler.
	/// </summary>
	private void PlayerDestroyedEventHandler ()
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
	
	/// <summary>
	/// Shot fired event handler.
	/// </summary>
	private void ShotFiredEventHandler ()
	{
		statistics.ShotsFired++;
	}
	
	/// <summary>
	/// Enemies destroyed event handler.
	/// </summary>
	/// <param name="type">Type.</param>
	/// <param name="scoreVaue">Score vaue.</param>
	private void EnemyDestroyedEventHandler (EnemyType type, int scoreVaue)
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
	
	/// <summary>
	/// Registers the destroy by contact component for observation.
	/// </summary>
	/// <param name="dbc">Dbc.</param>
	public void RegisterDestroyByContactForObservation (DestroyByContact dbc)
	{
		dbc.PlayerDestoryed += PlayerDestroyedEventHandler;
		dbc.EnemyDestroyed += EnemyDestroyedEventHandler;
	}
	
	/// <summary>
	/// Weapons locked event handler.
	/// </summary>
	private void WeaponsLockedEventHandler ()
	{
		weaponsLocked = true;
	}
	
	/// <summary>
	/// Weapons unlocked event handler.
	/// </summary>
	private void WeaponsUnlockedEventHandler ()
	{
		weaponsLocked = false;
	}
}