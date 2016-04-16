using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class GameManager : SingletonMonoBehaviour<GameManager> {

	public const string PLAYER_TAG = "Player";
	public const string TOUCH_OBJECT_TAG = "TouchObject";
	public const string STAR_TAG = "Star";				
	public const string METEO_TAG = "Meteo";

	void Awake() {
		if(this != Instance) {
			Destroy(this);
			return;
		}

		DontDestroyOnLoad(this.gameObject);
	}

	public void ChanageScene(string _name) {
		SceneManager.LoadScene (_name);
	}

}
