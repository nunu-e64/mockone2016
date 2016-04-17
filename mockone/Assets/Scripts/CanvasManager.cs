using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class CanvasManager : SingletonMonoBehaviour<CanvasManager> {

	[SerializeField]
	private GameObject logo;

	// Use this for initialization
	void Start () {
		this.SetLogo (GameManager.GameState.GAME_START);
	}
		
	public void SetLogo (GameManager.GameState _gameState){
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
