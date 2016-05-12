using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CrackPiece : MonoBehaviour {

	private List<GameObject> pieces;

	void Start() {
		pieces = new List<GameObject> ();

		foreach (Transform child in transform) {
			pieces.Add (child.gameObject);
		}
	}

	void SprashPieces() {
		foreach (var piece in pieces) {
//			Vector2 dir = this.transform.position	
			//iTween.mo
		}
	}

	void AnimationFinish () {
		Destroy (this.gameObject);
	}
}
