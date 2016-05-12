using UnityEngine;
using System.Collections;

public class TouchObject : MonoBehaviour {

	[SerializeField]
	private GameObject spakleEffect;
	[SerializeField]
	private GameObject sphireFromEffect;
	[SerializeField]
	private GameObject sphireToEffect;
	[SerializeField]
	private GameObject grow;

	private	bool isAvailable = false;
	private bool isSetEffect  = false;
	private bool isSphireToEffect  = false;
	private Hashtable hash;

	// Use this for initialization
	void Start () {
		this.isAvailable = true;
		this.isSetEffect = false;
		this.isSphireToEffect = false;
		this.grow.transform.parent = null;

		//Hashの生成
		this.hash = new Hashtable ();
		this.hash.Add ("x", 0);
		this.hash.Add ("time", 2);
		//this.hash.Add ("loopType", "pingPong");
		//this.hash.Add ("easeType", iTween.EaseType.easeInOutSine);  
	}
	
	// Update is called once per frame
	void Update () {
	
	}
		
	void OnTriggerEnter2D (Collider2D other) {
		if (this.isAvailable && other.CompareTag(GameManager.PLAYER_TAG)) {
			other.gameObject.GetComponent<MovePlayer> ().SetActionState (MovePlayer.ActionState.AROUND, this.gameObject);
			this.isAvailable = false;
		}
	}

	public void Reset () {
		Instantiate (spakleEffect, transform.position, Quaternion.identity);
		if (isSphireToEffect) {
			Instantiate (sphireToEffect, transform.position, Quaternion.identity);
		}
		this.grow.GetComponent<ParticleSystem> ().startColor = new Color(1, 1, 1, 0);

		Destroy (this.grow, 1); //TODO 1秒でエフェクトフェードアウトしてほしい
		Destroy (gameObject); //TODO: アニメーション再生->アニメーション終了時にDestroy
	}

	public void Init(float _radius) {
		//出現時に軌道半径を設定
		float ratio = transform.GetChild(0).localScale.magnitude / transform.localScale.magnitude;
		transform.localScale = Vector3.one * _radius / ratio * 2;
	}

	public void SetEffect () {
		if (!this.isSetEffect) {
			this.isSetEffect = true;
			this.sphireFromEffect.SetActive (true);
		}
	}

	public void SetSphireToEffect () {
		if (!this.isSphireToEffect) {
			this.isSphireToEffect = true;
		}
	}
}
