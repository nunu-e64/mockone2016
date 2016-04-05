using UnityEngine;
using System.Collections;

public class MovePlayer : MonoBehaviour {
 
	[SerializeField]
	private GameObject mainCamera;

	private const float movePower = 500;

	private Rigidbody playerRigidbody;
	private Vector3 touchObjectPos;
	private ActionState actionState;
	private MoveDirectionState moveDirectionState;

	public enum ActionState {
		NONE,
		RELEASE,
		MOVE,
		AROUND,
	}

	private enum MoveDirectionState {
		LEFT,
		RIGHT
	}

	// Use this for initialization
	void Start () {
		this.playerRigidbody = GetComponent<Rigidbody> ();
	}
	
	// Update is called once per frame
	void Update () {
		if (this.actionState == ActionState.MOVE) {
			this.SetMove (this.touchObjectPos);
		} else if (this.actionState == ActionState.AROUND) {
			this.SetAround (this.touchObjectPos);
		} else if (this.actionState == ActionState.RELEASE){
			this.SetRelease (this.touchObjectPos);
		}
	}
		
	public void SetActionState (ActionState _actionState, Vector3 _touchObjectPos) {
		this.actionState = _actionState;
		this.touchObjectPos = _touchObjectPos;

		if (transform.position.x - _touchObjectPos.x > 0) {
			this.moveDirectionState = MoveDirectionState.LEFT;
		} else {
			this.moveDirectionState = MoveDirectionState.RIGHT;
		}

		if (_actionState == ActionState.AROUND) {
			mainCamera.GetComponent<MainCamera>().SetAction(_touchObjectPos);
		}
	}

	public ActionState GetActionState () {
		return this.actionState;
	}

	private void SetMove (Vector3 _touchObjectPos) {
		this.playerRigidbody.AddForce (_touchObjectPos - transform.position);
	}

	private void SetAround (Vector3 _touchObjectPos){
		if (this.moveDirectionState == MoveDirectionState.RIGHT) {
			this.playerRigidbody.AddForce (new Vector3(-movePower, 0, 0));
		} else {
			this.playerRigidbody.AddForce (new Vector3(movePower, 0, 0));
		}
		this.actionState = ActionState.NONE;
	}
		
	private void SetRelease (Vector3 _touchObjectPos) {
		GameObject[] touchObjects = GameObject.FindGameObjectsWithTag("TouchObject");
		foreach (GameObject touchObject in touchObjects) {
			touchObject.GetComponent<TouchObject> ().Reset();
		}
		this.actionState = ActionState.NONE;
	}
		
	private void Init () {
		this.playerRigidbody.velocity = Vector3.zero;
	}
}
