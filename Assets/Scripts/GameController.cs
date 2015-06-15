using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class GameController : MonoBehaviour {
	public GameObject asteroid = null;
	public Vector3 asteroidSpawn = Vector3.zero;
	public int asteroidCount = 0;
	public float asteroidSpawnWait = 0.0f;
	public float asteroidSpawnStartWait = 0.0f;
	public float asteroidWaveWait = 0.0f;
	private int score = 0;
	public Text scoreText = null;
	public Text restartText = null;
	public Text gameOverText = null;
	private bool gameOver = false;
	private bool restart = false;

	// Use this for initialization
	void Start () {
		StartCoroutine(SpawnAsteroidWaves ());
		UpdateScore ();
		restartText.text = string.Empty;
		gameOverText.text = string.Empty;
	}
	
	// Update is called once per frame
	void Update () {
		if (restart && Input.GetKeyDown (KeyCode.R)) {
			Application.LoadLevel(Application.loadedLevel);
		}
	}

	IEnumerator SpawnAsteroidWaves(){
		yield return new WaitForSeconds(asteroidSpawnStartWait);

		while(!gameOver)
		{
			for(int i=0; i < asteroidCount && !gameOver; i++)
			{
				Vector3 spawnPosition = new Vector3 (Random.Range(-asteroidSpawn.x, asteroidSpawn.x), asteroidSpawn.y, asteroidSpawn.z);
				Instantiate (asteroid, spawnPosition, Quaternion.identity);
				yield return new WaitForSeconds(asteroidSpawnWait);
			}


			if(!gameOver) yield return new WaitForSeconds(asteroidWaveWait);
		}

			restartText.text += "Restart game by pressing the 'R' key...";
			restart = true;
	}

	void UpdateScore()
	{
		scoreText.text = string.Format("Score: {0:N0}", score);
	}

	public void AddScore(int points)
	{
		score += points;
		UpdateScore ();
	}

	public void PlayerDestroyed()
	{
		gameOverText.text = "Game Over Man!";
		gameOver = true;
	}
}
