using UnityEngine;
using System.Collections;

public class MainCamera : MonoBehaviour {

	[SerializeField]
	private float moveTime = 2;

	// Use this for initialization
	void Start () {
		Hashtable hash = new Hashtable ();
		hash.Add ("y", -19.2);
		hash.Add ("time", this.moveTime);
		hash.Add ("easeType", iTween.EaseType.linear);  
		iTween.MoveBy (this.gameObject, hash);
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
