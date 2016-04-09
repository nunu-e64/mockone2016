using UnityEngine;
using System.Collections;

public class MovePlayer : MonoBehaviour {
 
	private const float GRAVITY_POWER = 500;

	private Rigidbody playerRigidbody;
	private Vector3 touchObjectPos;
	private ActionState actionState;
	private MoveDirectionState moveDirectionState;

	public enum ActionState {
		NONE,		//直線移動中
		RELEASE,	//リリース瞬間
		MOVE,		//軌道に向かって移動中
		AROUND,		//周回中
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
		}
	}
		
	public void SetActionState (ActionState _actionState, Vector3 _touchObjectPos) {
		this.actionState = _actionState;
		this.touchObjectPos = _touchObjectPos;

		switch (this.actionState) {
		case ActionState.MOVE:
			//タップポイントに方向転換
			this.playerRigidbody.velocity = (_touchObjectPos - this.transform.position).normalized * this.playerRigidbody.velocity.magnitude;
			break;
		case ActionState.RELEASE:
			this.actionState = ActionState.NONE;
			break;
		default:
			break;
		}
	}

	public ActionState GetActionState () {
		return this.actionState;
	}

	private void SetMove (Vector3 _touchObjectPos) {
		this.playerRigidbody.AddForce ((_touchObjectPos - transform.position).normalized * GRAVITY_POWER);
	}

	private void SetAround (Vector3 _touchObjectPos){
		Debug.Log (this.moveDirectionState);
		return;
		if (this.moveDirectionState == MoveDirectionState.RIGHT) {
			this.playerRigidbody.AddForce (new Vector3(-GRAVITY_POWER, 0, 0));
		} else {
			this.playerRigidbody.AddForce (new Vector3(GRAVITY_POWER, 0, 0));
		}
		this.actionState = ActionState.NONE;
	}
		
	private void Init () {
		this.playerRigidbody.velocity = Vector3.zero;
	}
}
