using UnityEngine;
using System.Collections;

public class TitleManager : MonoBehaviour {

	[SerializeField]
	private GameObject button;

	private float timeElapsed;

	void Start () {
		this.timeElapsed = 0;
		AudioManager.Instance.PlayBGM ("BGM_Title");
	}

	void Update () {
		timeElapsed += Time.deltaTime;

		if (timeElapsed >= 0.5f) {
			button.SetActive (!button.activeSelf);
			timeElapsed = 0;
		}

		if (Input.GetMouseButtonDown(0)) {
			GameManager.Instance.ChangeScene (GameManager.SceneName.Prologue.ToString ());
		}
	}
}
