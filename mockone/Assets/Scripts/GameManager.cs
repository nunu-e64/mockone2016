using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class GameManager : SingletonMonoBehaviour<GameManager> {

	public static GameState gameState;

	public enum GameState {
		GAME_START,
		PLAYING,
		GAME_OVER,
		CLEAR
	}

	public const string PLAYER_TAG = "Player";
	public const string TOUCH_OBJECT_TAG = "TouchObject";
	public const string STAR_TAG = "Star";				
	public const string METEO_TAG = "Meteo";
	public const string MONSTER_TAG = "Monster";
	public const string WALL_TAG = "Wall";
	public const string WALL_CAMERA_UP_TAG = "WallCameraUp";
	public const string WALL_CAMERA_DOWN_TAG = "WallCameraDown";
	public const string GOAL_TAG = "Goal";

	void Awake () {
		if (this != Instance) {
			Destroy (this);
			return;
		}

		DontDestroyOnLoad (this.gameObject);
	}

	public void ChanageScene (string _name) {
		SceneManager.LoadScene (_name);
	}

	public void ReloadScene () {
		this.ChanageScene (SceneManager.GetActiveScene ().name);
	}
}
