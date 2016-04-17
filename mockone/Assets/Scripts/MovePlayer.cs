using UnityEngine;
using System.Collections;

public class MovePlayer : MonoBehaviour {
 
	private const float GRAVITY_POWER = 10;
	private const float SPEED_LOW = 4.0f;
	private const float SPEED_HIGH = 8.0f;
	private const float AROUND_BORDER_TIME = 5.0f;
	private const int MAX_REFLECT_TIMES = 5;

	private Rigidbody2D playerRigidbody;
	private ActionState actionState;
	private MoveDirectionState moveDirectionState;	//右回転か左回転か
	private float playerRadius;		//当たり判定半径
	private float aroundTime;	//回転時間
	private bool strong;	//強ビューン状態
	private int remainReflectable;	//残り反射可能回数

	public bool alive{get; set;}

	[SerializeField]
	private GameObject explosion;
	private GameObject mainCamera;
	private GameObject touchObject;
	[SerializeField]
	private HpBar hpBar;

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
		this.transform.position = new Vector3(0.0f, -4.5f, 0.0f);
		this.alive = true;
		this.actionState = ActionState.NONE;
		this.strong = false;
		this.remainReflectable = 0;
	}

	// Update is called once per frame
	void Update () {
		if (this.strong) {
			this.GetComponent<SpriteRenderer> ().color = new Color (255 / 255.0f, 170 / 255.0f, 70 / 255.0f, 1);
		} else {
			this.GetComponent<SpriteRenderer> ().color = new Color (1, 1, 1, 1);
		}

		//周回挙動
		if (this.actionState == ActionState.AROUND) {
			this.aroundTime += Time.deltaTime;
			if (this.aroundTime > AROUND_BORDER_TIME) {
				this.strong = true;
			}
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
			this.strong = false;
			this.remainReflectable = 0;
			break;
		case ActionState.RELEASE:
			if (this.touchObject)
				this.touchObject.GetComponent<TouchObject> ().Reset ();
			this.touchObject = null;
			if (this.actionState == ActionState.MOVE) {
				this.playerRigidbody.velocity = Vector2.zero;	//引力点に達する前にリリースされた場合その場停止
			} else {
				if (strong) {
					remainReflectable = MAX_REFLECT_TIMES;
				}
			}
			this.actionState = ActionState.NONE;
			break;
		case ActionState.AROUND:
			this.playerRigidbody.velocity = Vector2.zero;
			this.actionState = _actionState;
			this.aroundTime = 0.0f;
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
			this.playerRigidbody.velocity = new Vector2 (-1 * vec.y, 1 * vec.x).normalized * (strong ? SPEED_HIGH : SPEED_LOW);
		} else {
			this.playerRigidbody.velocity = new Vector2 (1 * vec.y, -1 * vec.x).normalized * (strong ? SPEED_HIGH : SPEED_LOW);
		}

		this.transform.position = this.touchObject.transform.position + -1 * vec.normalized * ((this.touchObject.transform.localScale.x / 2) + this.playerRadius);
	}

	void OnTriggerEnter2D (Collider2D other) {
		if (other.CompareTag (GameManager.STAR_TAG)) {
			if (this.strong && this.remainReflectable > 0) {
				Reflect (this.transform.position - other.transform.position);
				remainReflectable--;
				Debug.Log (remainReflectable);
				if (remainReflectable == 0) {
					strong = false;
				}
			} else {
				this.transform.position = other.transform.position + (this.transform.position - other.transform.position).normalized * ((other.transform.localScale.x / 2) + this.playerRadius);
				this.playerRigidbody.velocity = Vector2.zero;
				SetActionState (ActionState.RELEASE);
			}

		} else if (other.CompareTag (GameManager.METEO_TAG)) {
			if (this.strong && this.remainReflectable > 0) {
				Reflect (this.transform.position - other.transform.position);
				remainReflectable--;
				if (remainReflectable == 0) {
					strong = false;
				}
			} else {
				Dead ();
			}

		} else if (other.CompareTag (GameManager.MONSTER_TAG)) {
			if (this.strong) {
				GameObject.Instantiate (explosion, this.transform.position, this.transform.localRotation);
			} else {
				Dead ();
			}

		} else if (other.CompareTag (GameManager.WALL_TAG)) {
			if (this.strong && this.remainReflectable > 0) {
				Reflect (new Vector2(1, 0));
				remainReflectable--;
				if (remainReflectable == 0) {
					strong = false;
				}
			} else {
				this.playerRigidbody.velocity = Vector2.zero;
			}				

		} else if (other.CompareTag (GameManager.WALL_CAMERA_UP_TAG)) {
			if (transform.position.y - other.gameObject.transform.position.y < 0) {
				this.mainCamera.GetComponent<MainCamera> ().Up();
			}
		} else if (other.CompareTag (GameManager.WALL_CAMERA_DOWN_TAG)) {
			if (transform.position.y - other.gameObject.transform.position.y > 0) {
				this.mainCamera.GetComponent<MainCamera> ().Down();
			}
		} else if (other.CompareTag(GameManager.GOAL_TAG)) {
			this.playerRigidbody.velocity = Vector2.zero; //DEBUG
			SetActionState (ActionState.RELEASE);
			CanvasManager.Instance.SetLogo (GameManager.GameState.CLEAR);
		} else if (other.CompareTag(GameManager.CANDY_TAG)) {
			hpBar.Recover ();
			Destroy (other.gameObject);
		}
	}

	public void Dead() {
		this.alive = false;
		GameObject.Instantiate (explosion, this.transform.position, this.transform.localRotation);
		this.gameObject.SetActive (false);
		CanvasManager.Instance.SetLogo (GameManager.GameState.GAME_OVER);
	}

	void Reflect(Vector2 _normalVector) {
		this.playerRigidbody.velocity = Vector2.Reflect (this.playerRigidbody.velocity, _normalVector);
	}
}
