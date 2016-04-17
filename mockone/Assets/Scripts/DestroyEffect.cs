using UnityEngine;
using System.Collections;

public class DestroyEffect : MonoBehaviour {

	void AnimationFinish () {
		CanvasManager.Instance.SetLogo (GameManager.GameState.GAME_OVER);
		Destroy (this.gameObject);
	}
}
