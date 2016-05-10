using UnityEngine;
using System.Collections;

public class MainCamera : MonoBehaviour {

	private float moveTime = 1.5f;

	// Use this for initialization
	IEnumerator Start () {
		this.gameObject.transform.position += new Vector3 (0, 19.2f, 0);
		yield return new WaitForSeconds(0.5f);

		Hashtable hash = new Hashtable ();
		hash.Add ("y", 0);
		hash.Add ("time", this.moveTime);
		hash.Add ("easeType", iTween.EaseType.linear);  
		iTween.MoveTo (this.gameObject, hash);
	}

	public void Up() {
		iTween.MoveTo (this.gameObject, new Vector3 (0, 19.2f, transform.position.z), 0.5f);
	}

	public void Down() {
		iTween.MoveTo (this.gameObject, new Vector3 (0, 0, transform.position.z), 0.5f);
	}

}
