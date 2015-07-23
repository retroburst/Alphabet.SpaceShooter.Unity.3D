using UnityEngine;
using System.Collections;

/// <summary>
/// Configurable settings for use
/// by game components and set in the
/// Unity editor.
/// </summary>
public class ConfigurableSettings : MonoBehaviour {
	public int InitialLives;
	public int hazardCount;
	public float SpawnWait;
	public float StartWait;
	public float WaveWait;
	public int NumberOfInitialLetters;
	public int PlayerDestroyedWaitTime;
	public float WeaponsLockTime;
}
