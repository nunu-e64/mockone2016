using UnityEngine;
using System.Collections;

public class TouchObject : MonoBehaviour {

	private Rigidbody touchObjectRigidbody;
	private	bool isAvailable = false;

	// Use this for initialization
	void Start () {
		this.touchObjectRigidbody = GetComponent<Rigidbody> ();
		this.isAvailable = true;
	}
	
	// Update is called once per frame
	void Update () {
	
	}
		
	void OnTriggerEnter (Collider other) {
		if (this.isAvailable) {
			FixedJoint joint = gameObject.AddComponent<FixedJoint> ();
			joint.connectedBody = other.gameObject.GetComponent<Rigidbody> ();
			this.touchObjectRigidbody.constraints = RigidbodyConstraints.FreezePosition;

			other.gameObject.GetComponent<MovePlayer> ().SetActionState (MovePlayer.ActionState.AROUND, transform.position);
			this.isAvailable = false;
			GetComponent<Renderer>().material.color = Color.blue;
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
