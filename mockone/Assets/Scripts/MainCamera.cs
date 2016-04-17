using UnityEngine;
using System.Collections;

public class MainCamera : MonoBehaviour {

	// Use this for initialization
	void Start () {
	}

	// Update is called once per frame
	void Update () {
	}

	public void Up() {
		iTween.MoveTo (this.gameObject, new Vector3 (0, 19.2f, transform.position.z), 0.5f);
	}

	public void Down() {
		iTween.MoveTo (this.gameObject, new Vector3 (0, 0, transform.position.z), 0.5f);
	}

}
