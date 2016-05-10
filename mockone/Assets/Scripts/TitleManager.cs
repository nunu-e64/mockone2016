using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class TitleManager : MonoBehaviour {

	[SerializeField]
	private float findPlanetTime = 0.5f;
	[SerializeField]
	private float characterTime = 0.8f;
	[SerializeField]
	private float planetTime = 0.8f;
	[SerializeField]
	private float[] starTimes;

	[SerializeField]
	private GameObject button;
	[SerializeField]
	private Sprite[] sprites;
	[SerializeField]
	private GameObject character;
	[SerializeField]
	private GameObject[] planets;
	[SerializeField]
	private GameObject[] stars;

	private float timeElapsed;
	private int index;

	void Start () {
		this.timeElapsed = 0;
		this.index = 0;
		AudioManager.Instance.PlayBGM ("BGM_Title", 1, false);

		//Character
		Hashtable hash = new Hashtable ();
		hash.Add ("y", 5);
		hash.Add ("time", this.characterTime);
		hash.Add ("loopType", "pingPong");
		hash.Add ("easeType", iTween.EaseType.easeInOutSine);  
		iTween.MoveBy (this.character, hash);

		//Planets
		for (int i = 0; i < planets.Length; i++) {
			hash.Clear();
			hash.Add ("z", 10);
			hash.Add ("time", this.planetTime);
			hash.Add ("loopType", "pingPong");
			hash.Add ("easeType", iTween.EaseType.easeInOutSine);
			iTween.RotateTo (this.planets[i], hash); 
		} 

		//Stars
		for (int i = 0; i < stars.Length; i++) {
			Vector3 pos = stars [i].transform.position;
			hash.Clear();
			hash.Add ("x", -600);
			hash.Add ("y", -400);
			hash.Add ("time", this.starTimes[i]);
			hash.Add ("loopType", "loop");
			hash.Add ("easeType", iTween.EaseType.easeInOutSine);
			iTween.MoveBy (this.stars[i], hash); 
		} 

	}

	void Update () {
		timeElapsed += Time.deltaTime;

		if (timeElapsed >= this.findPlanetTime) {
			if (this.index == 0) {
				this.index = 1;
			} else {
				this.index = 0;
			}
			this.button.GetComponent<Image> ().sprite = this.sprites [index];
			timeElapsed = 0;
		}

		if (Input.GetMouseButtonDown(0)) {
			AudioManager.Instance.PlaySE ("SE_Ok");
			GameManager.Instance.ChangeScene (GameManager.SceneName.StageSelect.ToString ());
		}
	}
}
