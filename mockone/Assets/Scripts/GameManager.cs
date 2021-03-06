﻿using UnityEngine;
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

	[SerializeField, Space(10)]
	//モンスターの吹っ飛ばされ速度
	public float MONSTER_BLAST_SPEED = 10;
	//モンスターの吹っ飛ばされ回転量
	public float MONSTER_BLAST_ROTATE = 10;

	[SerializeField, Space(10)]
	//浮遊スピード
	public float PLAYER_SPIN_SPEED = 1;
	//浮遊回転量
	public float PLAYER_SPIN_ROTATE = 1.0f;

	[SerializeField, Space(10)]
	public float DRAWING_ACCELERATION = 0.1f;

	[System.NonSerialized]
	public GameState gameState;

	public float BYUUN_TIME = 0.8f;

	public float fadeInTime = 0.5f; 
	public float fadeOutTime = 0.5f;

	public enum GameState {
		NONE,
		GAME_START,
		PLAYING,
		GAME_OVER,
		CLEAR
	}

	public enum SceneName{
		TitleScene = 0,
		StageSelect,
		Prologue,
		Epilogue
	}

	public enum StageName{
		Stage1 = 0,
		Stage2,
		Stage3
	}
		
	public const string PLAYER_TAG = "Player";
	public const string TOUCH_OBJECT_TAG = "TouchObject";
	public const string STAR_TAG = "Star";				
	public const string METEO_TAG = "Meteo";
	public const string MONSTER_TAG = "Monster";
	public const string CRACK_TAG = "Crack";
	public const string WALL_HORIZONTAL_TAG = "WallHorizontal";
	public const string WALL_VERTICAL_TAG = "WallVertical";
	public const string WALL_CAMERA_UP_TAG = "WallCameraUp";
	public const string WALL_CAMERA_DOWN_TAG = "WallCameraDown";
	public const string GOAL_TAG = "Goal";
	public const string CANDY_TAG = "Candy";
	public const string TOUCH_FIELD_TAG = "TouchField";
	public const string TAP_TARGET_LAYER = "TapTarget";

	public static string PREVIOUS_SCENE;

	private int sceneNum;

	private delegate void OnComplete();
	private OnComplete callBack;

	void Awake () {
		if (this != Instance) {
			Destroy (this);
			return;
		}

		DontDestroyOnLoad (this.gameObject);
	}

	void Start () {
		this.sceneNum = SceneManager.sceneCountInBuildSettings;
		PREVIOUS_SCENE = SceneName.TitleScene.ToString ();
	}

	public void ChangeScene (string _name) {
		PREVIOUS_SCENE = this.GetActiveSceneName ();
		this.ChangeScene(()=> {
			SceneManager.LoadScene (_name);
			ScreenFadeManager.Instance.FadeOut (this.fadeOutTime, Color.black, ()=> {}); 
		});
	}

	public void ChangeScene (int _num) {
		PREVIOUS_SCENE = this.GetActiveSceneName ();
		this.ChangeScene(()=> {
			SceneManager.LoadScene (_num); 
			ScreenFadeManager.Instance.FadeOut (this.fadeOutTime, Color.black, ()=> {}); 
		});
	}

	public void ReloadScene () {
		this.ChangeScene (this.GetActiveSceneName ());
	}

	public void NextScene () {
		//BuildSettingsに従って次のステージへ
		if (this.isLastStage ()) {
			this.ChangeScene (GameManager.SceneName.Epilogue.ToString ());
		} else {
			this.ChangeScene (SceneManager.GetActiveScene ().buildIndex + 1);
		}
	}

	public bool isLastStage () {
		if (SceneManager.GetActiveScene ().buildIndex == this.sceneNum - 1) {
			return true;
		}

		return false;
	}

	public string GetActiveSceneName () {
		return SceneManager.GetActiveScene ().name;
	}

	private void ChangeScene (OnComplete onComplete) {
		this.callBack = onComplete;
		ScreenFadeManager.Instance.FadeIn (this.fadeInTime, Color.black, ()=> {
			this.callBack ();
		});  
	} 
}
