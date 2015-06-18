using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

[Serializable]
public class Highscores
{
	private const string HIGHSCORES_FILE = "Alphabet.SpaceShooter.Unity.3D.Highscores.dat";

	public List<HighscoreEntry> Entries { get; set; }

	public Highscores ()
	{
		Entries = new List<HighscoreEntry> ();
	}

	public static Highscores DeserializeHighscores ()
	{
		string filename = Path.Combine (Application.persistentDataPath, HIGHSCORES_FILE);
		Highscores result = null;
		if (File.Exists (filename)) {
			try {
				BinaryFormatter formatter = new BinaryFormatter ();
				using (FileStream fs = File.Open(filename, FileMode.Open)) {
					result = (Highscores)formatter.Deserialize (fs);
					fs.Close ();
				}
			} catch (Exception ex) {
				Debug.LogError (ex.Message);
			}
		}
		if (result == null)
			result = new Highscores ();
		return(result);
	}

	public static void SerializeHighscores (Highscores target)
	{
		string filename = Path.Combine (Application.persistentDataPath, HIGHSCORES_FILE);
		try {
			BinaryFormatter formatter = new BinaryFormatter ();
			using (FileStream fs = File.Open(filename, FileMode.OpenOrCreate)) {
				if (fs.Length > 0)
					fs.SetLength (0);
				formatter.Serialize (fs, target);
				fs.Close ();
			}
		} catch (Exception ex) {
			Debug.LogError (ex.Message);
		}
	}
}

[Serializable]
public class HighscoreEntry
{
	public int Score { get; set; }

	public int ShotsFired { get; set; }

	public int EnemyShipsDestroyed { get; set; }

	public int EnemyAsteroidsDestroyed { get; set; }

	public int TextEnemiesDestroyed { get; set; }

	public string PlayerName { get; set; }

	public DateTime HighscoreDate { get; set; }
}
