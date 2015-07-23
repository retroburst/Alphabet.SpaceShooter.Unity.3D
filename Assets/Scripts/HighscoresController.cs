using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Text;
using System.Linq;

/// <summary>
/// Controls the highscores display.
/// </summary>
public class HighscoresController : MonoBehaviour
{
	public GameObject Panel;
	public Button CloseButton;
	public Text HighscoreText;

	/// <summary>
	/// Start this instance.
	/// </summary>
	private void Start ()
	{
		CloseButton.onClick.AddListener (CloseHighscores);
	}
	
	/// <summary>
	/// Updates the highscores.
	/// </summary>
	public void UpdateHighscores ()
	{
		Highscores scores = Highscores.DeserializeHighscores ();
		StringBuilder sb = new StringBuilder ();
		var sortedScores = scores.Entries.OrderByDescending (x => x.Score);
		foreach (HighscoreEntry entry in sortedScores) {
			sb.AppendLine (string.Format ("{0} {1}  {2} {3}", entry.Score, entry.PlayerName, entry.HighscoreDate.ToLongDateString (), entry.HighscoreDate.ToShortTimeString (), entry.ShotsFired, entry.EnemyShipsDestroyed, entry.TextEnemiesDestroyed, entry.EnemyAsteroidsDestroyed));
		}
		HighscoreText.text = sb.ToString ();
	}
	
	/// <summary>
	/// Closes the highscores.
	/// </summary>
	public void CloseHighscores ()
	{
		Panel.SetActive (false);
	}
	
}
