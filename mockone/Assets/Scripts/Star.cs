using UnityEngine;
using System.Collections;

public class Star : MonoBehaviour {

	[SerializeField]
	StarImageKind starImageKind;
	[SerializeField]
	private Sprite[] starImage;

	private enum StarImageKind {
		BLUE_BIG,
		BLUE_SMALL,
		RED_BIG,
		RED_SMALL,
		YELLOW_BIG,
		YELLOW_SMALL
	}

	void Start () {
		if (starImage.Length <= (int)starImageKind) {
			Debug.LogError ("StarImage is less than starImageKindNum");
		} else {
			this.GetComponent<SpriteRenderer> ().sprite = starImage [(int)starImageKind];
		}
	}
}
