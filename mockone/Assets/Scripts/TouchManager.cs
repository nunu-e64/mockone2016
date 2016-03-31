using UnityEngine;
using System.Collections;

public class TouchManager : MonoBehaviour {

	[SerializeField]
	private GameObject mainCamera;
	[SerializeField]
	private GameObject movePlayer;
	[SerializeField]
	private GameObject touchObject;

	private enum TouchState {
		RELEASE,
		PRESS,
		LONG_PRESS
	}

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		if (Input.GetMouseButtonUp (0)) {
			this.SetAction (TouchState.RELEASE);
		}
		if (Input.GetMouseButtonDown (0)) {
			this.SetAction (TouchState.PRESS);
		}
		if (Input.GetMouseButton (0)) {
			this.SetAction (TouchState.LONG_PRESS);
		}
	}

	void SetAction (TouchState _touchState) {
		Vector3 mouseScreenPos = Input.mousePosition;
		mouseScreenPos.z = -mainCamera.transform.position.z;
		Vector3 touchObjectPos = Camera.main.ScreenToWorldPoint(mouseScreenPos);
		if (_touchState == TouchState.PRESS) {
			MovePlayer.ActionState nowState = movePlayer.GetComponent<MovePlayer> ().GetActionState ();
			if (nowState != MovePlayer.ActionState.MOVE && nowState != MovePlayer.ActionState.AROUND) {
				GameObject obj = Instantiate (touchObject, touchObjectPos, Quaternion.identity) as GameObject;
				movePlayer.GetComponent<MovePlayer> ().SetActionState (MovePlayer.ActionState.MOVE, touchObjectPos);
			}
		} else if (_touchState == TouchState.RELEASE) {
			movePlayer.GetComponent<MovePlayer> ().SetActionState (MovePlayer.ActionState.RELEASE, touchObjectPos);
		}
	}
}