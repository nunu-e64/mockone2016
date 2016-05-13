using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class PrologueManager : MonoBehaviour {

	//ページが残る時間
	[SerializeField]
	private float prologuePageTime;
	//ページがフェードインする時間
	[SerializeField]
	private float prologueFadeInTime;
	//ページがフェードアウトする時間
	[SerializeField]
	private float prologueFadeOutTime;
	[SerializeField]
	private GameObject image;
	[SerializeField]
	private Sprite[] sprites;
	[SerializeField]
	private float soundVolume = 0.8f;

	private float timeElapsed;
	private int touchCount;
	private string sceneName;

	private enum SceneName {
		Prologue,
		Epilogue
	}

	// Use this for initialization
	void Start () {
		this.timeElapsed = 0 - ScreenFadeManager.Instance.GetFadeOutTime ();
		this.touchCount = 0;
		this.sceneName = GameManager.Instance.GetActiveSceneName ();
		AudioManager.Instance.PlayBGM ((this.sceneName==SceneName.Prologue.ToString() ? "BGM_Prologue" : "BGM_Epilogue"), soundVolume);
	}
	
	// Update is called once per frame
	void Update () {
		timeElapsed += Time.deltaTime;
		if (timeElapsed >= this.prologuePageTime) {
			this.SetAction ();
		} else if (Input.GetMouseButtonDown(0)) {
			this.SetAction ();
		}
	}

	public void Skip () {
		if (this.sceneName == SceneName.Prologue.ToString ()) {
			GameManager.Instance.ChangeScene (GameManager.SceneName.TitleScene.ToString ());	
		} else {
			if (GameManager.PREVIOUS_SCENE == GameManager.SceneName.StageSelect.ToString ()) {
				GameManager.Instance.ChangeScene (GameManager.SceneName.StageSelect.ToString ());
			} else {
				GameManager.Instance.ChangeScene (GameManager.SceneName.TitleScene.ToString ());
			}
		}
	}

	private void SetAction () {
		if (this.touchCount == this.sprites.Length - 1) {
			this.Skip ();
		} else {
			this.NextPage ();
		}
	}

	private void NextPage () {
		this.touchCount ++;
		float time1 = this.prologueFadeInTime;
		float time2 = this.prologueFadeOutTime;

		//エピローグの2番目の画像のときはfadetimeを2倍にする
		if (this.sceneName == SceneName.Epilogue.ToString () && this.touchCount == 2) {
			time1 = this.prologuePageTime;
			time2 = this.prologuePageTime;
		} 

		ScreenFadeManager.Instance.FadeIn (time1, Color.black, ()=> {
			this.image.GetComponent<Image> ().sprite = sprites[this.touchCount];
			ScreenFadeManager.Instance.FadeOut (time2, Color.black, ()=> {}); 

			//プロローグの8番目の画像の時はBGMを変更する
			if (this.sceneName == SceneName.Prologue.ToString () && this.touchCount == 7) {
				AudioManager.Instance.PlayBGM ("BGM_Prologue2", this.soundVolume);
			} 

			//エピローグの2番目の画像の時はBGMを変更する
			if (this.sceneName == SceneName.Epilogue.ToString () && this.touchCount == 2) {
				AudioManager.Instance.PlayBGM ("BGM_Epilogue2", this.soundVolume);
			} 
		});  

		this.timeElapsed = 0 - time1 - time2;
	}
}
