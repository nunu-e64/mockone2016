using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class CrackPiece : MonoBehaviour {

	private List<GameObject> pieces;

	void Start() {
		pieces = new List<GameObject> ();

		foreach (Transform child in transform) {
			pieces.Add (child.gameObject);
		}
		SprashPieces ();
	}

	void SprashPieces() {
		foreach (var piece in pieces) {
			Vector3 amount = piece.GetComponent<CrackPieceChild> ().localCenterPositon * 5.0f;
			iTween.MoveBy(piece, iTween.Hash("time", 0.5f, "amount", amount, "delay", 0.0f, "easeType", "easeOutExpo"));
			iTween.FadeTo(piece, iTween.Hash("time", 0.3f, "alpha", 0.0f, "delay", 0.0f));
		}
		StartCoroutine(DelayMethod(1.0f, ()=>{
			AnimationFinish();
		}));
	}

	IEnumerator DelayMethod(float waitTime, Action action) {
		yield return new WaitForSeconds(waitTime);
		action();
	}

	void AnimationFinish () {
		Destroy (this.gameObject);
	}
}
