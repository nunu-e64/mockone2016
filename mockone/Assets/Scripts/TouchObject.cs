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

	void Reset () {
		if (GetComponent<FixedJoint> ()) {
			Destroy (GetComponent<FixedJoint> ());
		}
	}
}
