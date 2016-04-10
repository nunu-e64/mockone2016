﻿using UnityEngine;
using System.Collections;

public class TouchObject : MonoBehaviour {

	private Rigidbody2D touchObjectRigidbody;
	private	bool isAvailable = false;
	private const string PLAYER_TAG = "Player";

	// Use this for initialization
	void Start () {
		this.touchObjectRigidbody = GetComponent<Rigidbody2D> ();
		this.isAvailable = true;
	}
	
	// Update is called once per frame
	void Update () {
	
	}
		
	void OnTriggerEnter2D (Collider2D other) {
		Debug.Log ("OnTriggerEnter:" + other.name);
		if (this.isAvailable && other.CompareTag(PLAYER_TAG)) {
			FixedJoint2D joint = gameObject.AddComponent<FixedJoint2D> ();
			joint.connectedBody = other.gameObject.GetComponent<Rigidbody2D> ();
			this.touchObjectRigidbody.constraints = RigidbodyConstraints2D.FreezePosition;

			other.gameObject.GetComponent<MovePlayer> ().SetActionState (MovePlayer.ActionState.AROUND, transform.position);
			this.isAvailable = false;
		}
	}

	public void Reset () {
		if (GetComponent<FixedJoint> ()) {
			Destroy (GetComponent<FixedJoint> ());
		}
		Destroy (gameObject); //TODO: アニメーション再生->アニメーション終了時にDestroy
	}

	public void Init(float _radius) {
		//出現時に軌道半径を設定
		float ratio = transform.GetChild(0).localScale.magnitude / transform.localScale.magnitude;
		transform.localScale = Vector3.one * _radius / ratio * 2;
	}
}
