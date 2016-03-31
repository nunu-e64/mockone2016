using UnityEngine;
using System.Collections;

public class Sample : MonoBehaviour {

	private Rigidbody playerRigidbody;

	// Use this for initialization
	void Start () {
		this.playerRigidbody = GetComponent<Rigidbody> ();
		this.playerRigidbody.AddForce (new Vector3(1000,0,0));
	}
	
	// Update is called once per frame
	void Update () {
		//this.playerRigidbody.AddForce (new Vector3(10,0,0));
	}
}
