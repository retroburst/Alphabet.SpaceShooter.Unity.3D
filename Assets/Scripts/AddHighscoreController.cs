using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System;

public class AddHighscoreController : MonoBehaviour
{
	public GameObject Panel;
	public InputField PlayerNameField;
	public Button AddHighscoreButton;
	public Button CloseButton;
	public GameObject HighscoresPanel;

	// Use this for initialization
	void Start ()
	{
		AddHighscoreButton.onClick.AddListener (AddHighscoreButtonClick);
		CloseButton.onClick.AddListener(CloseAddHighscorePanel);
		PlayerNameField.ActivateInputField();
	}

	void AddHighscoreButtonClick ()
	{
		GameController gameController = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameController>();
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
		HighscoresPanel.GetComponent<HighscoresController>().UpdateHighscores();
		HighscoresPanel.SetActive(true);
	}
	
}
