using UnityEngine;
using System.Collections;

public class PlayerPrefsManager : SingletonMonoBehaviour<PlayerPrefsManager> {

	[SerializeField]
	private bool setInitialize = false;

	const string CLEAR_STAGE = "clearStage";

	void Awake () {
		if (this != Instance) {
			Destroy (this);
			return;
		}

		DontDestroyOnLoad (this.gameObject);
	}


	// Use this for initialization
	void Start () {
		if (setInitialize) {
			this.Init ();
		}
	}
		
	private void Init () {
		PlayerPrefs.DeleteAll ();
	}

	public void SetClearStage () {
		string stageName = GameManager.Instance.GetActiveSceneName ();
		int stageNameLength = stageName.Length;
		int score = int.Parse (stageName.Substring(stageNameLength - 1, 1));
		if (score > this.GetClearStage ()) {
			PlayerPrefs.SetInt(CLEAR_STAGE, score);
			PlayerPrefs.Save();
		}
	}

	public int GetClearStage (){
		return PlayerPrefs.GetInt(CLEAR_STAGE, 0);
	}
}
