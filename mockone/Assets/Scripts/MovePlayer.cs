using UnityEngine;
using System.Collections;

public class MovePlayer : MonoBehaviour {
 
	private const float GRAVITY_POWER = 10;
	private const float SPEED_LOW = 4.0f;
	private const float SPEED_HIGH = 8.0f;

	private Rigidbody2D playerRigidbody;
	private ActionState actionState;
	private MoveDirectionState moveDirectionState;	//右回転か左回転か
	private float playerRadius;		//当たり判定半径

	public bool alive{get; set;}

	[SerializeField]
	private GameObject explosion;
	private GameObject mainCamera;
	private GameObject touchObject;

	public enum ActionState {
		NONE,		//直線移動中
		RELEASE,	//リリース瞬間
		MOVE,		//軌道に向かって移動中
		AROUND		//周回中
	}

	private enum MoveDirectionState {
		LEFT,
		RIGHT
	}

	// Use this for initialization
	void Start () {
		this.mainCamera = FindObjectOfType<MainCamera> ().gameObject;
		this.playerRigidbody = GetComponent<Rigidbody2D> ();
		this.playerRadius = this.gameObject.transform.localScale.x * this.gameObject.GetComponent<CircleCollider2D> ().radius / 2;
		this.Init ();
	}

	public void Init () {
		this.gameObject.SetActive (true);
		this.playerRigidbody.velocity = Vector2.zero;
		this.transform.position = Vector2.zero;
		this.alive = true;
		this.actionState = ActionState.NONE;
	}

	// Update is called once per frame
	void Update () {
		if (this.actionState == ActionState.MOVE) {
		} else if (this.actionState == ActionState.AROUND) {
			this.GoAround ();
		}
		//速度方向に自機の画像を回転
		if (this.playerRigidbody.velocity.sqrMagnitude > 0) {
			this.transform.rotation = Quaternion.Euler (0, 0, -90 + Mathf.Rad2Deg * Mathf.Atan2 (this.playerRigidbody.velocity.y, this.playerRigidbody.velocity.x));
		}
	}
		
	public void SetActionState (ActionState _actionState, GameObject _touchObject = null) {
		this.touchObject = (_touchObject == null ? this.touchObject : _touchObject);

		switch (_actionState) {
		case ActionState.MOVE:
			//タップポイントに方向転換
			this.playerRigidbody.velocity = (this.touchObject.transform.position - this.transform.position).normalized * SPEED_LOW;
			this.actionState = _actionState;
			break;
		case ActionState.RELEASE:
			if (this.touchObject) this.touchObject.GetComponent<TouchObject> ().Reset ();
			this.touchObject = null;
			if (this.actionState == ActionState.MOVE) {
				this.playerRigidbody.velocity = Vector2.zero;	//引力点に達する前にリリースされた場合その場停止
			}
			this.actionState = ActionState.NONE;
			break;
		case ActionState.AROUND:
			this.playerRigidbody.velocity = Vector2.zero;
			this.actionState = _actionState;
			break;
		default:
			this.actionState = _actionState;
			this.touchObject = null;
			break;
		}
	}

	public ActionState GetActionState () {
		return this.actionState;
	}

	void GoAround (){	//引力点の周りを周回
		var vec = (touchObject.transform.position - this.transform.position);

		if (this.moveDirectionState == MoveDirectionState.RIGHT) {
			this.playerRigidbody.velocity = new Vector2 (-1 * vec.y, 1 * vec.x).normalized * SPEED_LOW;
		} else {
			this.playerRigidbody.velocity = new Vector2 (1 * vec.y, -1 * vec.x).normalized * SPEED_LOW;
		}

		this.transform.position = this.touchObject.transform.position + -1 * vec.normalized * ((this.touchObject.transform.localScale.x / 2) + this.playerRadius);
	}

	void OnTriggerEnter2D (Collider2D other) {
		if (other.CompareTag (GameManager.STAR_TAG)) {
			this.transform.position = other.transform.position + (this.transform.position - other.transform.position).normalized * ((other.transform.localScale.x / 2) + this.playerRadius);
			this.playerRigidbody.velocity = Vector2.zero;
			SetActionState (ActionState.RELEASE);
		} else if (other.ComparedTags (GameManager.METEO_TAG, GameManager.MONSTER_TAG)) {
			this.alive = false;
			GameObject.Instantiate (explosion, this.transform.position, this.transform.localRotation);
			this.gameObject.SetActive (false);
		} else if (other.CompareTag (GameManager.WALL_TAG)) {
			this.playerRigidbody.velocity = Vector2.zero;
		} else if (other.CompareTag (GameManager.WALL_CAMERA_UP_TAG)) {
			this.mainCamera.GetComponent<MainCamera> ().Up();
			this.transform.position = new Vector2 (this.transform.position.x, 8 + 2);
		} else if (other.CompareTag (GameManager.WALL_CAMERA_DOWN_TAG)) {
			this.mainCamera.GetComponent<MainCamera> ().Down();
			this.transform.position = new Vector2 (this.transform.position.x, 8 - 2);
		} else if (other.CompareTag(GameManager.GOAL_TAG)) {
			this.playerRigidbody.velocity = Vector2.zero; //DEBUG
			SetActionState (ActionState.RELEASE);
		}
	}
}
