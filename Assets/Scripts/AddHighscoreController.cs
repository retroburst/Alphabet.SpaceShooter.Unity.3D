using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System;

/// <summary>
/// Add highscore controller.
/// Resposible for adding and processing new high scores.
/// </summary>
public class AddHighscoreController : MonoBehaviour
{
	public GameObject Panel;
	public InputField PlayerNameField;
	public Button AddHighscoreButton;
	public Button CloseButton;
	public GameObject HighscoresPanel;
	
	/// <summary>
	/// Start this instance.
	/// </summary>
	private void Start ()
	{
		AddHighscoreButton.onClick.AddListener (AddHighscoreButtonClick);
		CloseButton.onClick.AddListener (CloseAddHighscorePanel);
		PlayerNameField.ActivateInputField ();
	}
	
	/// <summary>
	/// Add highscore button click handler.
	/// </summary>
	private void AddHighscoreButtonClick ()
	{
		GameController gameController = GameObject.FindGameObjectWithTag (Constants.TAG_GAME_CONTROLLER).GetComponent<GameController> ();
		string playerName = PlayerNameField.text;
		if (string.IsNullOrEmpty (playerName)) {
			playerName = "No Name";
		}
		Highscores scores = Highscores.DeserializeHighscores ();
		HighscoreEntry entry = new HighscoreEntry ();
		entry.Score = gameController.Statistics.Score;
		entry.PlayerName = playerName;
		entry.HighscoreDate = DateTime.Now;
		entry.ShotsFired = gameController.Statistics.ShotsFired;
		entry.TextEnemiesDestroyed = gameController.Statistics.EnemyTextsDestroyed;
		entry.EnemyAsteroidsDestroyed = gameController.Statistics.EnemyAsteroidsDestroyed;
		entry.EnemyShipsDestroyed = gameController.Statistics.EnemyShipsDestroyed;
		scores.Entries.Add (entry);
		Highscores.SerializeHighscores (scores);
		CloseAddHighscorePanel ();
	}
	
	/// <summary>
	/// Closes the add highscore panel.
	/// </summary>
	public void CloseAddHighscorePanel ()
	{
		Panel.SetActive (false);
		HighscoresPanel.GetComponent<HighscoresController> ().UpdateHighscores ();
		HighscoresPanel.SetActive (true);
	}
	
}
