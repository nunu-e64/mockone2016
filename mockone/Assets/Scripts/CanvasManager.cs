using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class CanvasManager : SingletonMonoBehaviour<CanvasManager>
{
	[SerializeField]
	private GameObject logo;
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
	// Use this for initialization
	void Start () {
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
			Debug.Log ("NextStage");
		});
		//ステージセレクト
		for (int i = 0; i < stageSelectButton.Length; i ++) {
			stageSelectButton[i].GetComponent<Button> ().onClick.AddListener (() => {
				Debug.Log ("StageSelect");
			});
		}
	}

	public void SetLogo (GameManager.GameState _gameState) {
		GameManager.Instance.gameState = _gameState;

		switch (_gameState) {
		case GameManager.GameState.GAME_START:
			this.logo.SetActive (true);
			this.logo.GetComponent<Text> ().text = "GameStart";
			break;
		case GameManager.GameState.PLAYING:
			this.logo.SetActive (false);
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
		this.logo.SetActive (false);
		this.gameOverLogo.SetActive (false);
		this.clearLogo.SetActive (false);
	}
}