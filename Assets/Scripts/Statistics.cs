using UnityEngine;
using System.Collections;

/// <summary>
/// Represents game play statistics.
/// </summary>
public class Statistics
{
	/// <summary>
	/// Gets or sets the score.
	/// </summary>
	/// <value>The score.</value>
	public int Score { get; set; }

	/// <summary>
	/// Gets or sets the lives left.
	/// </summary>
	/// <value>The lives left.</value>
	public int LivesLeft{ get; set; }

	/// <summary>
	/// Gets or sets the shots fired.
	/// </summary>
	/// <value>The shots fired.</value>
	public int ShotsFired{ get; set; }

	/// <summary>
	/// Gets or sets the times player destroyed.
	/// </summary>
	/// <value>The times player destroyed.</value>
	public int TimesPlayerDestroyed{ get; set; }

	/// <summary>
	/// Gets or sets the enemy ships destroyed.
	/// </summary>
	/// <value>The enemy ships destroyed.</value>
	public int EnemyShipsDestroyed{ get; set; }
	
	/// <summary>
	/// Gets or sets the enemy texts destroyed.
	/// </summary>
	/// <value>The enemy texts destroyed.</value>
	public int EnemyTextsDestroyed{ get; set; }

	/// <summary>
	/// Gets or sets the enemy asteroids destroyed.
	/// </summary>
	/// <value>The enemy asteroids destroyed.</value>
	public int EnemyAsteroidsDestroyed{ get; set; }
}
