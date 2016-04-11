using UnityEngine;
using System.Collections;

public class MovePlayer : MonoBehaviour {
 
	private const float GRAVITY_POWER = 10;
	private const float SPEED_LOW = 4.0f;
	private const float SPEED_HIGH = 8.0f;

	private Rigidbody2D playerRigidbody;
	private GameObject touchObject;
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
		this.playerRigidbody = GetComponent<Rigidbody2D> ();
	}
	
	// Update is called once per frame
	void Update () {
		if (this.actionState == ActionState.MOVE) {
		} else if (this.actionState == ActionState.AROUND) {
			this.GoAround ();
		}
		//速度方向に自機回転
		if (this.playerRigidbody.velocity.sqrMagnitude > 0) {
			this.transform.rotation = Quaternion.Euler (0, 0, -90 + Mathf.Rad2Deg * Mathf.Atan2 (playerRigidbody.velocity.y, playerRigidbody.velocity.x));
		}
		Debug.Log (this.playerRigidbody.velocity);
	}
		
	public void SetActionState (ActionState _actionState, GameObject _touchObject = null) {
		this.touchObject = _touchObject;

		switch (_actionState) {
		case ActionState.MOVE:
			//タップポイントに方向転換
			this.playerRigidbody.velocity = (this.touchObject.transform.position - this.transform.position).normalized * SPEED_LOW;
			this.actionState = _actionState;
			break;
		case ActionState.RELEASE:
			if (this.actionState == ActionState.MOVE) {
				this.playerRigidbody.velocity = Vector2.zero;
			}
			this.actionState = ActionState.NONE;
			break;
		case ActionState.AROUND:
			this.playerRigidbody.velocity = Vector2.zero;
			this.actionState = _actionState;
			break;
		default:
			break;
		}
	}

	public ActionState GetActionState () {
		return this.actionState;
	}
		
	void GoAround (){
		var vec = (touchObject.transform.position - this.transform.position);

		if (this.moveDirectionState == MoveDirectionState.RIGHT) {
			this.playerRigidbody.velocity = new Vector2 (-1 * vec.y, 1 * vec.x).normalized * SPEED_LOW;
		} else {
			this.playerRigidbody.velocity = new Vector2 (1 * vec.y, -1 * vec.x).normalized * SPEED_LOW;
		}

		this.transform.position = this.touchObject.transform.position + -1 * vec.normalized * this.touchObject.transform.localScale.x / 2;
	}
		
	void Init () {
		this.playerRigidbody.velocity = Vector3.zero;
	}
}
