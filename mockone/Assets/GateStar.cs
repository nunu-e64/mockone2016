using UnityEngine;
using System.Collections;

public class GateStar : MonoBehaviour {
	
	void Update () {
	
	}

	public void OpenGate(float time) {
		iTween.MoveBy (this.gameObject,  iTween.Hash("y", 1, "easeType", "easeInOutExpo", "time", time));
		iTween.FadeTo (this.gameObject, 0.0f, time);
	}
}
