using UnityEngine;
using System.Collections;

public class GateStar : MonoBehaviour {
	
	void Update () {
	
	}

	public void OpenGate(float time) {
		iTween.MoveBy (this.gameObject, new Vector3 (0, 1.0f, 0), time);
		iTween.FadeTo (this.gameObject, 0.0f, time);
	}
}
