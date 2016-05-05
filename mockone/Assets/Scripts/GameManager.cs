using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class GameManager : SingletonMonoBehaviour<GameManager> {

	//時間制限
	public float TIME_LIMIT = 30;
	//時間回復
	public float RECOVERY = 10;
	//タッチインターバル
	public float TOUCH_INTERVAL = 1;
	//基本スピード
	public float SPEED_LOW = 4;
	//高速スピード
	public float SPEED_HIGH = 8;
	//高速判定時間
	public float AROUND_BORDER_TIME = 5;
	//最大反射回数
	public int MAX_REFLECT_TIMES = 5;
	//基本回転スピード
	public float AROUND_SPEED_LOW = 4;
	//高速回転スピード
	public float AROUND_SPEED_HIGH = 8;

	[SerializeField, Space(15)]

	//モンスターの吹っ飛ばされ速度
	public float MONSTER_BLAST_SPEED = 10;
	//モンスターの吹っ飛ばされ回転量
	public float MONSTER_BLAST_ROTATE = 10;

	[System.NonSerialized]
	public GameState gameState;

	public enum GameState {
		GAME_START,
		PLAYING,
		GAME_OVER,
		CLEAR
	}

	public enum SceneName{
		Title = 0,
		StageSelect,
		Prologue,
		Epilogue
	}
		
	public const string PLAYER_TAG = "Player";
	public const string TOUCH_OBJECT_TAG = "TouchObject";
	public const string STAR_TAG = "Star";				
	public const string METEO_TAG = "Meteo";
	public const string MONSTER_TAG = "Monster";
	public const string WALL_HORIZONTAL_TAG = "WallHorizontal";
	public const string WALL_VERTICAL_TAG = "WallVertical";
	public const string WALL_CAMERA_UP_TAG = "WallCameraUp";
	public const string WALL_CAMERA_DOWN_TAG = "WallCameraDown";
	public const string GOAL_TAG = "Goal";
	public const string CANDY_TAG = "Candy";
	public const string TOUCH_FIELD_TAG = "TouchField";
	public const string TAP_TARGET_LAYER = "TapTarget";

	private int sceneNum;

	void Awake () {
		if (this != Instance) {
			Destroy (this);
			return;
		}

		DontDestroyOnLoad (this.gameObject);
	}

	void Start () {
		this.sceneNum = SceneManager.sceneCountInBuildSettings;
	}

	public void ChangeScene (string _name) {
		SceneManager.LoadScene (_name);
	}

	public void ChangeScene (int _num) {
		SceneManager.LoadScene (_num);
	}

	public void ReloadScene () {
		this.ChangeScene (this.GetActiveSceneName ());
	}

	public void NextScene () {
		//BuildSettingsに従って次のステージへ
		if (SceneManager.GetActiveScene ().buildIndex == sceneNum - 1) {
			GameManager.Instance.ChangeScene (GameManager.SceneName.Epilogue.ToString ());
		} else {
			this.ChangeScene (SceneManager.GetActiveScene ().buildIndex + 1);
		}
	}

	public string GetActiveSceneName () {
		return SceneManager.GetActiveScene ().name;
	}
}
