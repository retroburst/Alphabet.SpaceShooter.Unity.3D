using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System;

public class AddHighscore : MonoBehaviour
{
	public GameObject Panel;
	public InputField PlayerNameField;
	public Button AddHighscoreButton;
	public Button Close;
	public GameObject HighscoresPanel;

	// Use this for initialization
	void Start ()
	{
		AddHighscoreButton.onClick.AddListener (AddHighscoreButtonClick);
		Close.onClick.AddListener(CloseAddHighscorePanel);
		PlayerNameField.ActivateInputField();
	}

	void AddHighscoreButtonClick ()
	{
		Done_GameController gameController = GameObject.FindGameObjectWithTag("GameController").GetComponent<Done_GameController>();
		string playerName = PlayerNameField.text;
		if (string.IsNullOrEmpty (playerName))
			playerName = "No Name";
		Highscores scores = Highscores.DeserializeHighscores();
		HighscoreEntry entry = new HighscoreEntry();
		entry.Score = gameController.Statistics.Score;
		entry.PlayerName = playerName;
		entry.HighscoreDate = DateTime.Now;
		entry.ShotsFired = gameController.Statistics.ShotsFired;
		entry.TextEnemiesDestroyed = gameController.Statistics.EnemyTextsDestroyed;
		entry.EnemyAsteroidsDestroyed = gameController.Statistics.EnemyAsteroidsDestroyed;
		entry.EnemyShipsDestroyed = gameController.Statistics.EnemyShipsDestroyed;
		scores.Entries.Add(entry);
		Highscores.SerializeHighscores(scores);
		CloseAddHighscorePanel();
	}

	public void CloseAddHighscorePanel()
	{
		Panel.SetActive(false);
		HighscoresPanel.GetComponent<HighscoresPanel>().UpdateHighscores();
		HighscoresPanel.SetActive(true);
	}
	
}
