using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class HpBar : MonoBehaviour {

	[SerializeField]
	private GameObject movePlayer;
	[SerializeField]
	private GameObject gage1;
	[SerializeField]
	private GameObject gage2;

	private float RECOVERY;
	private float TIME_LIMIT;

	private float t;
	private float maxValue;
	private RectTransform gageTransform;
	private GameObject canvas;
	private Image gage1Image;
	private Image gage2Image;

	private Color32 color1 = new Color32 (255, 0, 0, 255);
	private Color32 color2 = new Color32 (255, 85, 85, 255);
	private Color32 color3 = new Color32 (255, 170, 170, 255);
	private Color32 color4 = new Color32 (255, 255, 255, 255);

	void Start () {
		this.RECOVERY = GameManager.Instance.RECOVERY;
		this.TIME_LIMIT = GameManager.Instance.TIME_LIMIT;
		this.gageTransform = gage2.GetComponent<RectTransform> ();
		this.maxValue = gageTransform.sizeDelta.x;
		this.t = 1;
		this.gage1Image = gage1.GetComponent<Image> ();
		this.gage2Image = gage2.GetComponent<Image> ();
	}

	void Update () {
		if (GameManager.Instance.gameState != GameManager.GameState.PLAYING)
			return;

		this.t -= Time.deltaTime / this.TIME_LIMIT;

		float x = Mathf.Lerp (0, this.maxValue, this.t);
		this.gageTransform.sizeDelta = new Vector2 (x, this.gageTransform.sizeDelta.y);

		if (this.t <= 0) {
			movePlayer.GetComponent<MovePlayer> ().Dead ();
		} else if (this.t <= 0.25f) {
			this.gage1Image.color = this.color1;
		} else if (this.t <= 0.5f) {
			this.gage1Image.color = this.color2;
		} else if (this.t <= 0.75f) {
			this.gage1Image.color = this.color3;
		} else {
			this.gage1Image.color = this.color4;
		}
		this.gage2Image.color = this.gage1Image.color;
	}

	public void Recover() {
		this.t += this.RECOVERY / this.TIME_LIMIT;
		this.t = Mathf.Min (this.t, 1.0f);
	}
		
}