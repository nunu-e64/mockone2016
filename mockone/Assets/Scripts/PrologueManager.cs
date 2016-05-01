using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class PrologueManager : MonoBehaviour {

	[SerializeField]
	private GameObject image;
	[SerializeField]
	private Sprite[] sprites;

	private int touchCount;

	// Use this for initialization
	void Start () {
		this.touchCount = 0;
	}
	
	// Update is called once per frame
	void Update () {
		if (Input.GetMouseButtonDown(0)) {
			this.touchCount ++;
			if (touchCount == sprites.Length) {
				GameManager.Instance.ChangeScene (GameManager.SceneName.StageSelect.ToString ());
			} else {
				image.GetComponent<Image> ().sprite = sprites[this.touchCount];
			}
		}
	}
}
