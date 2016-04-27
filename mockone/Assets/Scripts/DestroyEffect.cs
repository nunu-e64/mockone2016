using UnityEngine;
using System.Collections;

public class DestroyEffect : MonoBehaviour {
	public bool isGameOver{ private get; set; }

	void AnimationFinish () {
		if (isGameOver) CanvasManager.Instance.SetLogo (GameManager.GameState.GAME_OVER);
		Destroy (this.gameObject);
	}
}
