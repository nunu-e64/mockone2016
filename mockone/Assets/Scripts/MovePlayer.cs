using UnityEngine;
using System.Collections;

public class MovePlayer : MonoBehaviour {

	private float SPEED_LOW;
	private float SPEED_HIGH;
	private float AROUND_SPEED_LOW;
	private float AROUND_SPEED_HIGH;
	private float AROUND_BORDER_TIME;
	private int MAX_REFLECT_TIMES;
	private float PLAYER_SPIN_SPEED;
	private float PLAYER_SPIN_ROTATE;

	private Rigidbody2D playerRigidbody;
	private ActionState actionState;
	private MoveDirectionState moveDirectionState = MoveDirectionState.LEFT;	//右回転か左回転か
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
	[SerializeField]
	private GameObject[] effects;
	[SerializeField]
	private Sprite defaultImage;
	[SerializeField]
	private Sprite strongImage;

	public enum ActionState {
		NONE,	
		RELEASE,	//リリース瞬間
		MOVE,		//軌道に向かって移動中
		AROUND,		//周回中
		FLOATING	//くるくる飛んでる
	}

	private enum MoveDirectionState {
		LEFT,
		RIGHT
	}

	// Use this for initialization
	void Start () {
		this.PLAYER_SPIN_SPEED = GameManager.Instance.PLAYER_SPIN_SPEED;
		this.PLAYER_SPIN_ROTATE = GameManager.Instance.PLAYER_SPIN_ROTATE;
		this.SPEED_LOW = GameManager.Instance.SPEED_LOW;
		this.SPEED_HIGH = GameManager.Instance.SPEED_HIGH;
		this.AROUND_BORDER_TIME = GameManager.Instance.AROUND_BORDER_TIME;
		this.MAX_REFLECT_TIMES = GameManager.Instance.MAX_REFLECT_TIMES;
		this.AROUND_SPEED_LOW = GameManager.Instance.AROUND_SPEED_LOW;
		this.AROUND_SPEED_HIGH = GameManager.Instance.AROUND_SPEED_HIGH;

		this.mainCamera = FindObjectOfType<MainCamera> ().gameObject;
		this.playerRigidbody = GetComponent<Rigidbody2D> ();
		this.playerRadius = this.gameObject.transform.localScale.x * this.gameObject.GetComponent<CircleCollider2D> ().radius / 2;
		this.Init ();
	}

	public void Init () {
		this.gameObject.SetActive (true);
		this.playerRigidbody.velocity = Vector2.zero;
		this.transform.position = new Vector3(0.0f, -6.5f, 0.0f);
		this.alive = true;
		this.actionState = ActionState.NONE;
		this.strong = false;
		this.remainReflectable = 0;
	}

	// Update is called once per frame
	void Update () {
		if (this.strong) {
			this.GetComponent<SpriteRenderer> ().color = new Color (255 / 255.0f, 170 / 255.0f, 70 / 255.0f, 1);
			this.effects [0].SetActive (false);
			this.effects [1].SetActive (true);
		} else {
			this.GetComponent<SpriteRenderer> ().color = new Color (1, 1, 1, 1);
			this.effects [0].SetActive (true);
			this.effects [1].SetActive (false);
		}

		//周回挙動
		if (this.actionState == ActionState.AROUND) {
			this.aroundTime += Time.deltaTime;
			if (this.aroundTime > AROUND_BORDER_TIME) {
				this.strong = true;
			}
			this.GoAround ();
		}
		//その場回転または速度方向に自機の画像を回転
		if (this.actionState == ActionState.FLOATING) {
			this.transform.Rotate(new Vector3 (0f, 0f, this.PLAYER_SPIN_ROTATE));
		} else if (this.playerRigidbody.velocity.sqrMagnitude > 0) {
			this.transform.rotation = Quaternion.Euler (0, 0, -90 + Mathf.Rad2Deg * Mathf.Atan2 (this.playerRigidbody.velocity.y, this.playerRigidbody.velocity.x));
		}
	}
		
	public void SetActionState (ActionState _actionState, GameObject _touchObject = null) {
		this.touchObject = (_touchObject == null ? this.touchObject : _touchObject);

		switch (_actionState) {
		case ActionState.MOVE:
			//一回でも移動開始したらアニメーション終了
			this.GetComponent<Animator> ().Stop ();
			this.GetComponent<SpriteRenderer> ().sprite = this.defaultImage;
			//タップポイントに方向転換
			this.playerRigidbody.velocity = (this.touchObject.transform.position - this.transform.position).normalized * SPEED_LOW;
			this.actionState = _actionState;
			this.finishStrong ();
			this.remainReflectable = 0;
			break;
		case ActionState.RELEASE:
			if (this.touchObject) {
				this.touchObject.GetComponent<TouchObject> ().Reset ();
				this.touchObject = null;
			}
			if (this.actionState == ActionState.MOVE) {
				this.playerRigidbody.velocity = this.playerRigidbody.velocity.normalized * this.PLAYER_SPIN_SPEED;	//引力点に達する前にリリースされた場合速度低下
				SetActionState (ActionState.FLOATING);
			} else {
				if (strong) {
					remainReflectable = MAX_REFLECT_TIMES;
					this.playerRigidbody.velocity = this.playerRigidbody.velocity.normalized * this.SPEED_HIGH;
					this.GetComponent<SpriteRenderer> ().sprite = this.strongImage;
				} else {
					this.playerRigidbody.velocity = this.playerRigidbody.velocity.normalized * this.SPEED_LOW;
				}
				this.actionState = ActionState.NONE;
			}
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
			this.playerRigidbody.velocity = new Vector2 (-1 * vec.y, 1 * vec.x).normalized * (strong ? AROUND_SPEED_HIGH : AROUND_SPEED_LOW);
		} else {
			this.playerRigidbody.velocity = new Vector2 (1 * vec.y, -1 * vec.x).normalized * (strong ? AROUND_SPEED_HIGH : AROUND_SPEED_LOW);
		}

		this.transform.position = this.touchObject.transform.position + -1 * vec.normalized * ((this.touchObject.transform.localScale.x / 2) + this.playerRadius);
	}

	void OnTriggerEnter2D (Collider2D other) {
		if (other.CompareTag (GameManager.STAR_TAG)) {
			if (this.strong) {
				Reflect (this.transform.position - other.transform.position);
			} else {
				this.transform.position = other.transform.position + (this.transform.position - other.transform.position).normalized * ((other.transform.localScale.x / 2) + this.playerRadius);
				Reflect (this.transform.position - other.transform.position);
				this.playerRigidbody.velocity = this.playerRigidbody.velocity.normalized * this.PLAYER_SPIN_SPEED;
				SetActionState (ActionState.FLOATING);
			}

		} else if (other.CompareTag (GameManager.METEO_TAG)) {
			if (this.strong) {
				Reflect (this.transform.position - other.transform.position);
			} else {
				Dead ();
			}

		} else if (other.CompareTag (GameManager.MONSTER_TAG)) {
			Monster monster = other.GetComponent<Monster> ();
			if (!monster.hasBlasted) {
				if (this.strong) {
					monster.Dead (this.playerRigidbody.velocity.normalized);
					Reflect (this.transform.position - other.transform.position);
				} else {
					Dead ();
				}
			}

		} else if (other.CompareTags (GameManager.WALL_HORIZONTAL_TAG, GameManager.WALL_VERTICAL_TAG)) {
			if (this.strong && this.remainReflectable > 0) {
				Reflect (other.CompareTag(GameManager.WALL_HORIZONTAL_TAG) ? new Vector2(1, 0) : new Vector2(0, 1));
			} else {
				Reflect (other.CompareTag(GameManager.WALL_HORIZONTAL_TAG) ? new Vector2(1, 0) : new Vector2(0, 1));
				this.playerRigidbody.velocity = this.playerRigidbody.velocity.normalized * this.PLAYER_SPIN_SPEED;
				SetActionState (ActionState.FLOATING);
			}

		} else if (other.CompareTag (GameManager.WALL_CAMERA_UP_TAG)) {
			if (transform.position.y - other.gameObject.transform.position.y < 0) {
				this.mainCamera.GetComponent<MainCamera> ().Up();
				this.finishStrong ();
				playerRigidbody.velocity = Vector2.zero;
				iTween.MoveTo (this.gameObject, new Vector2 (this.transform.position.x, other.transform.position.y + other.transform.localScale.y / 2.0f), 0.5f);
				SetActionState (ActionState.NONE);
			}

		} else if (other.CompareTag (GameManager.WALL_CAMERA_DOWN_TAG)) {
			if (transform.position.y - other.gameObject.transform.position.y > 0) {
				if (this.strong && this.remainReflectable > 0) {
					Reflect (other.CompareTag(GameManager.WALL_HORIZONTAL_TAG) ? new Vector2(1, 0) : new Vector2(0, 1));
				} else {
					Reflect (other.CompareTag(GameManager.WALL_HORIZONTAL_TAG) ? new Vector2(1, 0) : new Vector2(0, 1));
					this.playerRigidbody.velocity = this.playerRigidbody.velocity.normalized * this.PLAYER_SPIN_SPEED;
					SetActionState (ActionState.FLOATING);
				}
			}
		} else if (other.CompareTag(GameManager.GOAL_TAG)) {
			//this.playerRigidbody.velocity = Vector2.zero; //DEBUG
			this.playerRigidbody.velocity = Vector2.up;
			SetActionState (ActionState.RELEASE);
			CanvasManager.Instance.SetLogo (GameManager.GameState.CLEAR);
		} else if (other.CompareTag(GameManager.CANDY_TAG)) {
			hpBar.Recover ();
			Destroy (other.gameObject);
		}
	}

	public void Dead() {
		this.alive = false;
		var obj = GameObject.Instantiate (explosion, this.transform.position, this.transform.localRotation) as GameObject;
		obj.GetComponent<DestroyEffect> ().isGameOver = true;
		this.gameObject.SetActive (false);
	}

	void Reflect(Vector2 _normalVector) {
		this.playerRigidbody.velocity = Vector2.Reflect (this.playerRigidbody.velocity, _normalVector.normalized);
		remainReflectable--;
		Debug.Log ("<color=green>remainReflectable: " + remainReflectable + "</color>");
		if (remainReflectable == 0) {
			this.finishStrong ();
		}
	}

	void finishStrong() {
		strong = false;
		this.GetComponent<SpriteRenderer> ().sprite = this.defaultImage;
		if (playerRigidbody.velocity.sqrMagnitude > 0) {
			playerRigidbody.velocity = playerRigidbody.velocity.normalized * SPEED_LOW;
		}

		var monsters = GameObject.FindGameObjectsWithTag (GameManager.MONSTER_TAG);
		foreach (var monster in monsters) {
			monster.GetComponent<Monster> ().FinishPlayerStrong ();
		}
	}
}
