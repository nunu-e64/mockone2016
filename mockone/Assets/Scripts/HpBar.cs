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
	[SerializeField]
	private GameObject seigenJikan;

	private float RECOVERY;
	private float TIME_LIMIT;

	private float t;
	private float maxValue;
	private RectTransform gageTransform;
	private GameObject canvas;
	private Image gage1Image;
	private Image gage2Image;
	private float timeElapsed;

	private Color32 color1 = new Color32 (255, 0, 0, 255);
	private Color32 color2 = new Color32 (255, 127, 127, 255);
	private Color32 color3 = new Color32 (255, 255, 255, 255);

	void Start () {
		this.RECOVERY = GameManager.Instance.RECOVERY;
		this.TIME_LIMIT = GameManager.Instance.TIME_LIMIT;
		this.gageTransform = gage2.GetComponent<RectTransform> ();
		this.maxValue = gageTransform.sizeDelta.x;
		this.t = 1;
		this.gage1Image = gage1.GetComponent<Image> ();
		this.gage2Image = gage2.GetComponent<Image> ();
		this.timeElapsed = 0;
	}

	void Update () {
		if (GameManager.Instance.gameState != GameManager.GameState.PLAYING)
			return;

		this.t -= Time.deltaTime / this.TIME_LIMIT;

		float x = Mathf.Lerp (0, this.maxValue, this.t);
		this.gageTransform.sizeDelta = new Vector2 (x, this.gageTransform.sizeDelta.y);

		if (this.t <= 0) {
			movePlayer.GetComponent<MovePlayer> ().Dead ();
		} else if (this.t <= 0.33f) {
			this.gage1Image.color = this.color1;
			AudioManager.Instance.SetBGMPitch (2);
			timeElapsed += Time.deltaTime;
			if (timeElapsed >= 0.5f) {
				seigenJikan.SetActive (!seigenJikan.activeSelf);
				timeElapsed = 0;
			}
		} else if (this.t <= 0.66f) {
			this.gage1Image.color = this.color2;
			AudioManager.Instance.SetBGMPitch (1);
		} else {
			this.gage1Image.color = this.color3;
			AudioManager.Instance.SetBGMPitch (1);
		}
		this.gage2Image.color = this.gage1Image.color;
	}

	public void Recover() {
		this.t += this.RECOVERY / this.TIME_LIMIT;
		this.t = Mathf.Min (this.t, 1.0f);
	}
		
}