using UnityEngine;
using System.Collections;

public class GateStar : MonoBehaviour {
	
	void Update () {
	
	}

	public void OpenGate(float time) {
		iTween.MoveBy (this.gameObject,  iTween.Hash("y", 0.5, "easeType", "easeOutExpo", "time", time));
		iTween.FadeTo (this.gameObject, iTween.Hash("amount", 0.0f, "time", time, "easeType", "easeOutExpo"));
	}
}
