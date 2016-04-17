using UnityEngine;
using System.Collections;

public class HpBar : MonoBehaviour {

	private const float RECOVERY = 5.0f;

	[SerializeField]
	private float timeLimit;
	[SerializeField]
	private GameObject movePlayer;

	private float t;
	private float maxValue;
	private RectTransform gageTransform;
	private GameObject canvas;

	void Start () {
		this.gageTransform = gameObject.GetComponent<RectTransform> ();
		this.maxValue = gageTransform.sizeDelta.x;
		this.t = 1;
	}

	void Update () {
		if (GameManager.gameState != GameManager.GameState.PLAYING)
			return;

		this.t -= Time.deltaTime / this.timeLimit;

		float x = Mathf.Lerp (0, this.maxValue, this.t);
		this.gageTransform.sizeDelta = new Vector2 (x, this.gageTransform.sizeDelta.y);

		if (this.t <= 0) {
			movePlayer.GetComponent<MovePlayer> ().Dead ();
		}
	}

	public void Recover() {
		this.t += RECOVERY / this.timeLimit;
		this.t = Mathf.Min (this.t, 1.0f);
	}
		
}