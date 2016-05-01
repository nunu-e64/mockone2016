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

	//GameStart用Hash
	private int pingPongCount = 0;
	private Hashtable hash;

	// Use this for initialization
	void Start () {

		//GameStartHashの生成
		this.hash = new Hashtable ();
		this.hash.Add ("scale", new Vector3 (1.5f, 1.5f, 1.5f));
		this.hash.Add ("time", 0.8f);
		this.hash.Add ("loopType", "pingPong");
		this.hash.Add ("easeType", iTween.EaseType.easeInQuad);
		this.hash.Add ("oncomplete", "OnComplete");
		this.hash.Add ("oncompletetarget", gameObject);

		this.Init ();
		this.SetLogo (GameManager.GameState.GAME_START);

		//一時停止
		stopButton.GetComponent<Button> ().onClick.AddListener (() => {
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
			stopModal.SetActive (false);
			Time.timeScale = 1;
		});
		//リトライ
		for (int i = 0; i < retryButton.Length; i ++) {
			retryButton[i].GetComponent<Button> ().onClick.AddListener (() => {
				GameManager.Instance.ReloadScene ();
				Time.timeScale = 1;
			});
		}
		//Nextステージ
		nextButton.GetComponent<Button> ().onClick.AddListener (() => {
			GameManager.Instance.NextScene ();
		});
		//ステージセレクト
		for (int i = 0; i < stageSelectButton.Length; i ++) {
			stageSelectButton[i].GetComponent<Button> ().onClick.AddListener (() => {
				GameManager.Instance.ChangeScene (GameManager.SceneName.StageSelect.ToString ());
			});
		}
	}

	public void SetLogo (GameManager.GameState _gameState) {
		GameManager.Instance.gameState = _gameState;

		switch (_gameState) {
		case GameManager.GameState.GAME_START:
			this.gameStartLogo.SetActive (true);
			this.game.SetActive (true);
			iTween.ScaleTo (this.game, this.hash);
			break;
		case GameManager.GameState.PLAYING:
			this.gameStartLogo.SetActive (false);
			break;
		case GameManager.GameState.GAME_OVER:
			this.gameOverLogo.SetActive (true);
			break;
		case GameManager.GameState.CLEAR:
			this.clearLogo.SetActive (true);
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