using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public enum gameStatus {
	next, play, gameover, win
}

public class GameManager : Singleton<GameManager> {
	[SerializeField]
	private int totalWaves = 10;
	[SerializeField]
	private Text totalMoneyLbl;
	[SerializeField]
	private Text currentWaveLbl;
	[SerializeField]
	private Text totalEscapedLbl;
	[SerializeField]
	private GameObject spawnPoint;
	[SerializeField]
	private Enemy[] enemies;
	[SerializeField]
	private int totalEnemies = 3;
	[SerializeField]
	private int enemiesPerSpawn;
	[SerializeField]
	private Text playBtnLbl;
	[SerializeField]
	private Button playBtn;

	private int waveNumber = 0;
	private int totalMoney = 10;
	private int totalEscaped = 0;
	private int roundEscaped = 0;
	private int totalKilled = 0;
	private int WhichEnemiesToSpawn = 0;
	private int enemiesToSpawn = 0;
	private gameStatus currentState  = gameStatus.play;
	private AudioSource audioSource;

	public List<Enemy> EnemyList = new List<Enemy> ();

	const float spawnDelay = 0.5f;

	public int TotalEscaped{
		get{ 
			return totalEscaped;
		}
		set{
			totalEscaped = value;
		}
	}

	public int RoundEscaped{
		get{ 
			return roundEscaped;
		}
		set{ 
			roundEscaped = value;
		}
	}

	public int TotalKilled{
		get{
			return totalKilled;
		}
		set{ 
			totalKilled = value;
		}
	}

	public int TotalMoney{
		get{ 
			return totalMoney;
		}
		set{ 
			totalMoney = value;
			totalMoneyLbl.text = totalMoney.ToString();
		}
	}

	public AudioSource AudioSource{
		get{ 
			return audioSource;
		}
	}
	// Use this for initialization
	void Start () {
		playBtn.gameObject.SetActive (false);
		audioSource = GetComponent<AudioSource> ();
		showMenu ();
	}
		
	void Update(){
		handleEscape ();
	}


	IEnumerator spawn(){
		if (enemiesPerSpawn > 0 && EnemyList.Count < totalEnemies) {
			for (int i = 0; i < enemiesPerSpawn; i++) {
				if (EnemyList.Count < totalEnemies) {
					Enemy newEnemy = Instantiate (enemies [Random.Range(0, enemiesToSpawn)]);
					newEnemy.transform.position = spawnPoint.transform.position; // appear at the spawn position
				}
			}
			yield return new WaitForSeconds (spawnDelay);
			StartCoroutine (spawn());
		}
	}

	public void RegisterEnemy(Enemy enemy){
		EnemyList.Add (enemy);
	}

	public void UnregisterEnemy(Enemy enemy){
		EnemyList.Remove (enemy);
		Destroy (enemy.gameObject);
	}

	public void DestroyAllEnemies(){
		foreach(Enemy enemy in EnemyList){
			Destroy(enemy.gameObject);
		}

		EnemyList.Clear ();
	}

	public void addMoney(int amount){
		TotalMoney += amount;
	}
	public void substractMoney(int amount){
		TotalMoney -= amount;
	}

	public void isWaveOver(){
		totalEscapedLbl.text = "Escaped " + TotalEscaped + "/10";
		if ((RoundEscaped + TotalKilled) == totalEnemies) {
			if (waveNumber <= enemies.Length) {
				enemiesToSpawn = waveNumber;
			}
			setCurrentGameState();
			showMenu ();
		}
	}

	public void setCurrentGameState(){
		if (TotalEscaped >= 10) {
			currentState = gameStatus.gameover;
			AudioSource.PlayOneShot (SoundManager.Instance.Gameover);
		} else if (waveNumber == 0 && (TotalKilled + RoundEscaped) == 0) {
			currentState = gameStatus.play;
		} else if (waveNumber >= totalWaves) {
			currentState = gameStatus.win;
		} else {
			currentState = gameStatus.next;

		}
	}
	public void showMenu(){
		switch (currentState) {
		case gameStatus.gameover:
			playBtnLbl.text = "Play Again!";
			AudioSource.PlayOneShot (SoundManager.Instance.Gameover);
			break;
		case gameStatus.next:
			playBtnLbl.text = "Next Wave";
			break;
		case gameStatus.play:
			playBtnLbl.text = "Play";
			break;
		case gameStatus.win:
			playBtnLbl.text = "Play";
			break;
		}
		playBtn.gameObject.SetActive (true);
	}


	public void playBtnPressed(){
		//Debug.Log ("Pressed");
		switch(currentState){
		case gameStatus.next:
			waveNumber += 1;
			totalEnemies += waveNumber;
			break;
		default:
			totalEnemies = 3;
			TotalEscaped = 0;
			TotalMoney = 10;
			enemiesToSpawn = 0;
			TowerManager.Instance.DestroyAllTower ();
			TowerManager.Instance.RenameTagsBuildSites ();
			totalMoneyLbl.text = TotalMoney.ToString ();
			totalEscapedLbl.text = "Escaped " + TotalEscaped + "/10";
			audioSource.PlayOneShot(SoundManager.Instance.Newgame);
			break;
		}
		DestroyAllEnemies ();
		TotalKilled = 0;
		RoundEscaped = 0;
		currentWaveLbl.text = "Wave " + (waveNumber + 1);
		StartCoroutine (spawn());
		playBtn.gameObject.SetActive (false);
	}

	public void handleEscape(){
		if (Input.GetKeyDown (KeyCode.Escape)) {
			TowerManager.Instance.disableDragSprite ();
			TowerManager.Instance.towerBtnPressed = null;

		}
	} 
}
