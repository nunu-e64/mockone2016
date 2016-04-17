using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class CanvasManager : SingletonMonoBehaviour<CanvasManager>
{
	[SerializeField]
	private GameObject logo;
	[SerializeField]
	private GameObject stopModal;
	[SerializeField]
	private GameObject stopButton;
	[SerializeField]
	private GameObject backButton;
	[SerializeField]
	private GameObject retryButton;

	// Use this for initialization
	void Start ()
	{
		this.SetLogo (GameManager.GameState.GAME_START);

		//一時停止
		stopButton.GetComponent<Button> ().onClick.AddListener (() => {
			stopModal.SetActive (true);
			Hashtable hash = new Hashtable();
			hash.Add ("x", 100);
			hash.Add ("time", 0.5);
			hash.Add ("ignoretimescale", true);
			backButton.transform.position += new Vector3 (-100, 0, 0);
			iTween.MoveBy (backButton, hash);
			retryButton.transform.position += new Vector3 (-100, 0, 0);
			iTween.MoveBy (retryButton, hash);
			Time.timeScale = 0;
		});
		//ゲーム画面に戻る
		backButton.GetComponent<Button> ().onClick.AddListener (() => {
			stopModal.SetActive (false);
			Time.timeScale = 1;
		});
		//シーン再読み込み
		retryButton.GetComponent<Button> ().onClick.AddListener (() => {
			GameManager.Instance.ReloadScene ();
			Time.timeScale = 1;
		});
	}

	public void SetLogo (GameManager.GameState _gameState)
	{
		GameManager.gameState = _gameState;

		switch (_gameState) {
		case GameManager.GameState.GAME_START:
			this.logo.SetActive (true);
			this.logo.GetComponent<Text> ().text = "GameStart";
			break;
		case GameManager.GameState.PLAYING:
			this.logo.SetActive (false);
			break;
		case GameManager.GameState.GAME_OVER:
			this.logo.SetActive (true);
			this.logo.GetComponent<Text> ().text = "GameOver";
			break;
		case GameManager.GameState.CLEAR:
			this.logo.SetActive (true);
			this.logo.GetComponent<Text> ().text = "Clear";
			break;
		default:
			break;
		}
	}
}