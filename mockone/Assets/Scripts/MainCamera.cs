using UnityEngine;
using System.Collections;

public class MainCamera : MonoBehaviour {

	[SerializeField]
	private GameObject movePlayer;

	private Vector3 startPosition;
	private Vector3 startRotation;
	private Vector3 defaultOffset;

	private CameraState cameraState;
	public enum CameraState {
		FOLLOWING,
		FIXED,
		CHAISING
	}

	// Use this for initialization
	void Start () {
		this.startPosition = transform.position;
		this.startRotation = transform.localEulerAngles;
		this.defaultOffset = transform.position - movePlayer.transform.position;
		cameraState = CameraState.FOLLOWING;
	}

	// Update is called once per frame
	void Update () {
	}

	public void SetState(CameraState _state) {
		cameraState = _state;
	}

	void ChasePlayer () {
		Hashtable table = new Hashtable ();
		table.Add ("from", transform.position - movePlayer.transform.position); 
		table.Add ("to", defaultOffset); 
		table.Add ("time", 0.8f);
		table.Add ("onupdate", "SetPosition");
		table.Add ("oncomplete", "SetState");
		table.Add ("oncompletetarget", gameObject);
		table.Add ("oncompleteparams", CameraState.FOLLOWING);
		iTween.ValueTo(gameObject, table);
	}

	void SetPosition(Vector3 _offset) {
		transform.position = movePlayer.transform.position + _offset;
	}
}
