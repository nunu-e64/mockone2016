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

	private float timeElapsed;
	private int touchCount;
	private string sceneName;

	private enum SceneName {
		Prologue,
		Epilogue
	}

	// Use this for initialization
	void Start () {
		AudioManager.Instance.PlayBGM ("BGM_StageSelect");
		this.timeElapsed = 0 - ScreenFadeManager.Instance.GetFadeOutTime ();
		this.touchCount = 0;
		this.sceneName = GameManager.Instance.GetActiveSceneName ();
	}
	
	// Update is called once per frame
	void Update () {
		timeElapsed += Time.deltaTime;
		if (timeElapsed >= this.prologuePageTime) {
			this.touchCount ++;
			if (touchCount == sprites.Length) {
				this.Skip ();
			} else {
				ScreenFadeManager.Instance.FadeIn (this.prologueFadeInTime, Color.black, ()=> {
					this.image.GetComponent<Image> ().sprite = sprites[this.touchCount];
					ScreenFadeManager.Instance.FadeOut (this.prologueFadeOutTime, Color.black, ()=> {}); 
				});  
			}
			this.timeElapsed = 0 - this.prologueFadeOutTime;

			//エピローグの2番目の画像のときはさらにtimeElapsedを引く
			if (this.sceneName == SceneName.Epilogue.ToString () && this.touchCount == 1) {
				this.timeElapsed -= this.prologuePageTime;
			} 
		}
	}

	public void Skip () {
		if (this.sceneName == SceneName.Prologue.ToString ()) {
			GameManager.Instance.ChangeScene (GameManager.SceneName.TitleScene.ToString ());	
		} else {
			GameManager.Instance.ChangeScene (GameManager.SceneName.StageSelect.ToString ());
		}
	}
}
