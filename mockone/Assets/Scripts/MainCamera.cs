using UnityEngine;
using System.Collections;

public class MainCamera : MonoBehaviour {

	[SerializeField]
	private GameObject movePlayer;

	private Vector3 startPosition;
	private Vector3 startRotation;
	private Vector3 offset;

	// Use this for initialization
	void Start () {
		this.startPosition = transform.position;
		this.startRotation = transform.localEulerAngles;
		this.offset = transform.position - movePlayer.transform.position;
	}

	// Update is called once per frame
	void Update () {

	}

	public void SetAction (Vector3 _touchObjectPos) {
		Hashtable table = new Hashtable ();
		table.Add ("position", _touchObjectPos + this.offset); 
		table.Add ("time", 0.8f);
		table.Add ("easeType", "easeInOutCirc");
		iTween.MoveTo(gameObject, table);
	}
}
