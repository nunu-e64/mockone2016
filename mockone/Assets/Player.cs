using UnityEngine;
using System.Collections;

public class Player : MonoBehaviour {

	private float first_speed = 10.0f;
	private Rigidbody rigid;

	// Use this for initialization
	void Start () {
		rigid = gameObject.GetComponent<Rigidbody> ();
		rigid.velocity = new Vector3 (0, first_speed, 0);
	}
	
	// Update is called once per frame
	void Update () {
	}
}
