using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;

/// <summary>
/// Represents a highscores list.
/// </summary>
[Serializable]
public class Highscores
{
	private const string HIGHSCORES_FILE = "Alphabet.SpaceShooter.Unity.3D.Highscores.dat";

	public List<HighscoreEntry> Entries { get; set; }
	
	/// <summary>
	/// Initializes a new instance of the <see cref="Highscores"/> class.
	/// </summary>
	public Highscores ()
	{
		Entries = new List<HighscoreEntry> ();
	}
	
	/// <summary>
	/// Deserializes the highscores.
	/// </summary>
	/// <returns>The highscores.</returns>
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
		if (result == null) {
			result = new Highscores ();
		}
		return(result);
	}
	
	/// <summary>
	/// Serializes the highscores.
	/// </summary>
	/// <param name="target">Target.</param>
	public static void SerializeHighscores (Highscores target)
	{
		string filename = Path.Combine (Application.persistentDataPath, HIGHSCORES_FILE);
		try {
			BinaryFormatter formatter = new BinaryFormatter ();
			using (FileStream fs = File.Open(filename, FileMode.OpenOrCreate)) {
				if (fs.Length > 0) {
					fs.SetLength (0);
				}
				formatter.Serialize (fs, target);
				fs.Close ();
			}
		} catch (Exception ex) {
			Debug.LogError (ex.Message);
		}
	}
}

/// <summary>
/// Represents a highscore entry.
/// </summary>
[Serializable]
public class HighscoreEntry
{
	/// <summary>
	/// Gets or sets the score.
	/// </summary>
	/// <value>The score.</value>
	public int Score { get; set; }
	
	/// <summary>
	/// Gets or sets the shots fired.
	/// </summary>
	/// <value>The shots fired.</value>
	public int ShotsFired { get; set; }

	/// <summary>
	/// Gets or sets the enemy ships destroyed.
	/// </summary>
	/// <value>The enemy ships destroyed.</value>
	public int EnemyShipsDestroyed { get; set; }
	
	/// <summary>
	/// Gets or sets the enemy asteroids destroyed.
	/// </summary>
	/// <value>The enemy asteroids destroyed.</value>
	public int EnemyAsteroidsDestroyed { get; set; }
	
	/// <summary>
	/// Gets or sets the text enemies destroyed.
	/// </summary>
	/// <value>The text enemies destroyed.</value>
	public int TextEnemiesDestroyed { get; set; }
	
	/// <summary>
	/// Gets or sets the name of the player.
	/// </summary>
	/// <value>The name of the player.</value>
	public string PlayerName { get; set; }
	
	/// <summary>
	/// Gets or sets the highscore date.
	/// </summary>
	/// <value>The highscore date.</value>
	public DateTime HighscoreDate { get; set; }
}
