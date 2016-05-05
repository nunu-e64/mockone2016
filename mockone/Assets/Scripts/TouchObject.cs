using UnityEngine;
using System.Collections;

public class TouchObject : MonoBehaviour {

	[SerializeField]
	private GameObject spakleEffect;

	private	bool isAvailable = false;

	// Use this for initialization
	void Start () {
		this.isAvailable = true;
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
		Instantiate(spakleEffect, transform.position, Quaternion.identity);
		Destroy (gameObject); //TODO: アニメーション再生->アニメーション終了時にDestroy
	}

	public void Init(float _radius) {
		//出現時に軌道半径を設定
		float ratio = transform.GetChild(0).localScale.magnitude / transform.localScale.magnitude;
		transform.localScale = Vector3.one * _radius / ratio * 2;
	}
}
