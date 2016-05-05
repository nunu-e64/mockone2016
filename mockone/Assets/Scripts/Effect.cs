using UnityEngine;
using System.Collections;

public class Effect : MonoBehaviour {

	[SerializeField]
	private float limit;

	private float timeElapsed;

	// Use this for initialization
	void Start () {
		this.timeElapsed = 0;
	}
	
	// Update is called once per frame
	void Update () {
		this.timeElapsed += Time.deltaTime;
		if (this.timeElapsed >= this.limit) {
			Destroy (gameObject);
		}
	}
}
