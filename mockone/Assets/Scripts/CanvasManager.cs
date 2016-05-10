using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System;

public class CanvasManager : SingletonMonoBehaviour<CanvasManager>
{
	[SerializeField]
	private GameObject gameStartLogo;
	[SerializeField]
	private GameObject game;
	[SerializeField]
	private GameObject start;
	[SerializeField]
	private GameObject clearLogo;
	[SerializeField]
	private GameObject gameOverLogo;
	[SerializeField]
	private GameObject stopModal;
	[SerializeField]
	private GameObject stopButton;
	[SerializeField]
	private GameObject backButton;
	[SerializeField]
	private GameObject[] retryButton;
	[SerializeField]
	private GameObject nextButton;
	[SerializeField]
	private GameObject[] stageSelectButton;
	[SerializeField]
	private GameObject goalEffect;
	[SerializeField]
	private GameObject debugStageNumber;

	private bool isClear;
	private float isClearTime;
	private GameObject player;

	//GameStart用Hash
	private int pingPongCount = 0;
	private Hashtable hash;

	// Use this for initialization
	void Start () {
		this.isClear = false;
		this.isClearTime = 0;
		this.player = GameObject.Find ("Player");

		debugStageNumber.GetComponent<Text> ().text = GameManager.Instance.GetActiveSceneName ();

		//GameStartHashの生成
		this.hash = new Hashtable ();
		this.hash.Add ("scale", new Vector3 (1.5f, 1.5f, 1.5f));
		this.hash.Add ("time", 1.0f);
		this.hash.Add ("loopType", "pingPong");
		this.hash.Add ("easeType", iTween.EaseType.easeInQuad);
		this.hash.Add ("oncomplete", "OnComplete");
		this.hash.Add ("oncompletetarget", gameObject);

		this.Init ();
		this.SetLogo (GameManager.GameState.GAME_START);

		//一時停止
		stopButton.GetComponent<Button> ().onClick.AddListener (() => {
			AudioManager.Instance.PlaySE("SE_Pause");
			stopModal.SetActive (true);
			Hashtable hash = new Hashtable ();
			hash.Add ("x", 100);
			hash.Add ("time", 0.5);
			hash.Add ("ignoretimescale", true);
			backButton.transform.position += new Vector3 (-100, 0, 0);
			iTween.MoveBy (backButton, hash);
			retryButton[0].transform.position += new Vector3 (-100, 0, 0);
			iTween.MoveBy (retryButton[0], hash);
			stageSelectButton[2].transform.position += new Vector3 (-100, 0, 0);
			iTween.MoveBy (stageSelectButton[2], hash);
			Time.timeScale = 0;
		});
		//ゲーム画面に戻る
		backButton.GetComponent<Button> ().onClick.AddListener (() => {
			AudioManager.Instance.PlaySE ("SE_Ok");
			stopModal.SetActive (false);
			Time.timeScale = 1;
		});
		//リトライ
		for (int i = 0; i < retryButton.Length; i ++) {
			retryButton[i].GetComponent<Button> ().onClick.AddListener (() => {
				AudioManager.Instance.StopBGM ();
				AudioManager.Instance.StopSE ();
				AudioManager.Instance.PlaySE ("SE_Ok");
				GameManager.Instance.ReloadScene ();
				Time.timeScale = 1;
			});
		}
		//Nextステージ
		nextButton.GetComponent<Button> ().onClick.AddListener (() => {
			AudioManager.Instance.StopSE ();
			AudioManager.Instance.PlaySE ("SE_Ok");
			GameManager.Instance.NextScene ();
		});
		//ステージセレクト
		for (int i = 0; i < stageSelectButton.Length; i ++) {
			stageSelectButton[i].GetComponent<Button> ().onClick.AddListener (() => {
				AudioManager.Instance.StopSE ();
				AudioManager.Instance.StopBGM ();
				AudioManager.Instance.PlaySE ("SE_Ok");
				GameManager.Instance.ChangeScene (GameManager.SceneName.StageSelect.ToString ());
				Time.timeScale = 1;
			});
		}
	}

	void Update () {
		if (this.isClear) {
			this.isClearTime += Time.deltaTime;
			if (isClearTime >= 1) {
				this.clearLogo.SetActive (true);
				this.isClear = false;
				this.isClearTime = 0;
			}
		}
	}

	public void SetLogo (GameManager.GameState _gameState) {
		GameManager.Instance.gameState = _gameState;

		switch (_gameState) {
		case GameManager.GameState.GAME_START:
			AudioManager.Instance.StopSE ();
			AudioManager.Instance.PlaySE ("SE_StageStart");
			this.gameStartLogo.SetActive (true);
			this.game.SetActive (true);
			iTween.ScaleTo (this.game, this.hash);
			break;
		case GameManager.GameState.PLAYING:
			this.gameStartLogo.SetActive (false);
			break;
		case GameManager.GameState.GAME_OVER:
			AudioManager.Instance.StopBGM ();
			AudioManager.Instance.PlaySE ("SE_GameOver");
			this.gameOverLogo.SetActive (true);
			break;
		case GameManager.GameState.CLEAR:
			PlayerPrefsManager.Instance.SetClearStage ();
			AudioManager.Instance.StopBGM ();
			AudioManager.Instance.PlaySE ("SE_Clear");
			this.goalEffect.SetActive (true);
			this.isClear = true;
			//ラストステージの時はNextだけ表示
			if (GameManager.Instance.isLastStage ()) {
				stageSelectButton[1].SetActive (false);
			}
			//プレイヤーの判定をなくす
			this.player.GetComponent<MovePlayer> ().SetIsClear (true);
			break;
		default:
			break;
		}
	}

	private void Init () {
		this.gameStartLogo.SetActive (false);
		this.gameOverLogo.SetActive (false);
		this.clearLogo.SetActive (false);
	}

	void OnComplete () {
		this.pingPongCount ++;
		if (this.pingPongCount == 2) {
			this.game.SetActive (false);
			this.start.SetActive (true);
			iTween.ScaleTo (this.start, this.hash);
		} else if (this.pingPongCount == 4) {
			this.start.SetActive (false);
			this.pingPongCount = 0;
			this.SetLogo (GameManager.GameState.PLAYING);
		}
	}
}