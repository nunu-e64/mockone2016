﻿using UnityEngine;
using System.Collections;
using System;

public class MovePlayer : MonoBehaviour {

	private float SPEED_LOW;
	private float SPEED_HIGH;
	private float AROUND_SPEED_LOW;
	private float AROUND_SPEED_HIGH;
	private float AROUND_BORDER_TIME;
	private int MAX_REFLECT_TIMES;
	private float PLAYER_SPIN_SPEED;
	private float PLAYER_SPIN_ROTATE;
	private float DRAWING_ACCELERATION;

	private Rigidbody2D playerRigidbody;
	private ActionState actionState;
	private MoveDirectionState moveDirectionState = MoveDirectionState.LEFT;	//右回転か左回転か
	private float playerRadius;		//当たり判定半径
	private float aroundTime;	//回転時間
	private bool strong;	//強ビューン状態
	private int remainReflectable;	//残り反射可能回数
	private bool isClear;
	private int crackCount;

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
	[SerializeField]
	private GameObject shockEffect;
	[SerializeField]
	private GameObject shockEffectStrong;
	[SerializeField]
	private GameObject strongEffect;

	private GameObject touchArea;
	public bool hasTouchIntervalPassed { set; private get;}
	private float hitStop = 0;
	private float strongTime = 0;

	private Vector3 collisionPosition;

	public enum ActionState {
		NONE,		//ステージ開始状態、カメラ移動時、ビューン中
		RELEASE,	//リリース瞬間
		MOVE,		//軌道に向かって移動中
		AROUND,		//周回中
		FLOATING	//壁に当たってくるくるふわふわ飛んでる
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
		this.DRAWING_ACCELERATION = GameManager.Instance.DRAWING_ACCELERATION;

		this.mainCamera = FindObjectOfType<MainCamera> ().gameObject;
		this.playerRigidbody = GetComponent<Rigidbody2D> ();
		this.playerRadius = this.gameObject.transform.localScale.x * this.gameObject.GetComponent<CircleCollider2D> ().radius / 2;
		this.isClear = false;

		this.touchArea = this.transform.FindChild ("TouchArea").gameObject;
		if (!this.touchArea) {
			Debug.LogError ("Not Found TouchArea in Player Children");
		}
		this.Init ();
	}

	public void Init () {
		this.gameObject.SetActive (true);
		this.playerRigidbody.velocity = Vector2.zero;
//		this.transform.position = new Vector3(0.0f, 9.0f, 0.0f);	//TODO
		this.transform.position = new Vector3(0.0f, -6.5f, 0.0f);
		this.alive = true;
		this.actionState = ActionState.NONE;
		this.strong = false;
		this.remainReflectable = 0;
		this.SetActiveTouchArea (false);
		this.crackCount = 0;
	}

	// Update is called once per frame
	void Update () {
		//ビューン状態に応じた色とエフェクト
		if (this.strong) {
			if (this.collisionPosition != null) {
				if (Vector3.Distance (transform.position, this.collisionPosition) >= 10.0f) {
					this.SetLifeTime (0.5f);
					this.collisionPosition = new Vector3(1000, 1000, 0);
				} else {
					this.SetLifeTime (Mathf.Lerp (0.0f, 0.5f, Vector3.Distance (transform.position, this.collisionPosition) / 6.0f));
				}
			}
			this.GetComponent<SpriteRenderer> ().color = new Color (255 / 255.0f, 170 / 255.0f, 70 / 255.0f, 1);
			this.effects [0].SetActive (false);
			this.effects [1].SetActive (true);
			if (this.actionState == ActionState.NONE) {
				strongTime -= Time.deltaTime;
				if (strongTime <= 0f) {
					var speed = this.playerRigidbody.velocity.magnitude - 60f * Time.deltaTime;
					if (speed <= this.PLAYER_SPIN_SPEED) {
						finishStrong ();
						this.SetActionState (ActionState.FLOATING);
					} else {
						this.playerRigidbody.velocity = this.playerRigidbody.velocity.normalized * speed;
					}
				}
			}
		} else {
			this.GetComponent<SpriteRenderer> ().color = new Color (1, 1, 1, 1);
			this.effects [0].SetActive (this.actionState != ActionState.MOVE);
			this.effects [1].SetActive (false);
		}

		//タップエリアの表示判定
		if (GameManager.Instance.gameState == GameManager.GameState.PLAYING && !touchArea.activeSelf &&
			hasTouchIntervalPassed && (this.actionState == ActionState.FLOATING || this.actionState == ActionState.NONE)) {
			this.SetActiveTouchArea(true);
			hasTouchIntervalPassed = false;
		}

		//周回挙動
		if (this.actionState == ActionState.AROUND) {
			this.aroundTime += Time.deltaTime;
			if (!this.strong && this.aroundTime > AROUND_BORDER_TIME) {
				this.strong = true;
				AudioManager.Instance.PlaySE ("SE_ChangeGravyColor");
			} else if (!this.strong && this.aroundTime < AROUND_BORDER_TIME) {
				AudioManager.Instance.SetSEPitch ("SE_Around2", Mathf.Lerp(0.7f, 7.0f, this.aroundTime / this.AROUND_BORDER_TIME));
			}
			this.GoAround ();
		}

		//その場回転または速度方向に自機の画像を回転
		if (this.actionState == ActionState.FLOATING || this.actionState == ActionState.MOVE) {
			this.transform.Rotate(new Vector3 (0f, 0f, this.PLAYER_SPIN_ROTATE));
		} else if (this.playerRigidbody.velocity.sqrMagnitude > 0) {
			this.transform.rotation = Quaternion.Euler (0, 0, -90 + Mathf.Rad2Deg * Mathf.Atan2 (this.playerRigidbody.velocity.y, this.playerRigidbody.velocity.x));
		}

		//引力点に引き寄せられてる間は加速
		if (this.actionState == ActionState.MOVE
				&& this.playerRigidbody.velocity.sqrMagnitude < this.SPEED_LOW * this.SPEED_LOW) {
			this.playerRigidbody.velocity += this.playerRigidbody.velocity.normalized * this.DRAWING_ACCELERATION;
			if (this.playerRigidbody.velocity.sqrMagnitude > this.SPEED_LOW * this.SPEED_LOW) {
				this.playerRigidbody.velocity = this.playerRigidbody.velocity.normalized * this.SPEED_LOW;
			}
		}
//		Debug.Log (this.actionState);
	}
		
	public void SetActionState (ActionState _actionState, GameObject _touchObject = null) {
		if (_touchObject != null)
		{
			this.touchObject = _touchObject;
			this.crackCount = 0;
		}

		switch (_actionState) {
		case ActionState.MOVE:
			this.SetLifeTime (0.5f);
			//強ビューン終了
			this.finishStrong ();
			//タップエリア削除
			this.SetActiveTouchArea(false);
			//一回でも移動開始したらアニメーション終了
			this.GetComponent<Animator> ().Stop ();
			this.GetComponent<SpriteRenderer> ().sprite = this.defaultImage;
			//タップポイントに方向転換
			this.playerRigidbody.velocity = (this.touchObject.transform.position - this.transform.position).normalized * 0.01f;
			this.actionState = ActionState.MOVE;
			this.remainReflectable = 0;
			break;
		case ActionState.RELEASE:
			this.SetActiveTouchArea (false);	//タップエリア削除
			if (this.touchObject) {
				this.touchObject.GetComponent<TouchObject> ().Reset ();
				this.touchObject = null;
			}
			if (this.actionState == ActionState.MOVE) {
				SetActionState (ActionState.FLOATING);
			} else {
				if (strong) {
					AudioManager.Instance.PlaySE ("SE_ByuunStrong");
					remainReflectable = MAX_REFLECT_TIMES;
					this.playerRigidbody.velocity = this.playerRigidbody.velocity.normalized * this.SPEED_HIGH;
					this.GetComponent<SpriteRenderer> ().sprite = this.strongImage;
					strongTime = GameManager.Instance.BYUUN_TIME;
				} else {
					AudioManager.Instance.PlaySE ("SE_ByuunWeak");
					this.playerRigidbody.velocity = this.playerRigidbody.velocity.normalized * this.SPEED_LOW;
				}
				SetActionState (ActionState.NONE);
			}
			AudioManager.Instance.StopSE ("SE_Around2");
			break;
		case ActionState.AROUND:
			this.SetActiveTouchArea(false); //タップエリア削除
			AudioManager.Instance.PlaySE ("SE_Around2", 0.15f, true);
			this.playerRigidbody.velocity = Vector2.zero;
			this.actionState = ActionState.AROUND;
			this.aroundTime = 0.0f;
			break;
		case ActionState.FLOATING:
			if (this.touchObject) {
				this.touchObject.GetComponent<TouchObject> ().Reset ();
				this.touchObject = null;
			}
			this.playerRigidbody.velocity = this.playerRigidbody.velocity.normalized * this.PLAYER_SPIN_SPEED;
			this.actionState = ActionState.FLOATING;
			break;
		case ActionState.NONE:
			if (this.touchObject) {
				this.touchObject.GetComponent<TouchObject> ().Reset ();
				this.touchObject = null;
			}
			this.actionState = ActionState.NONE;
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
		if (this.isClear)
			return;

		if (other.CompareTag (GameManager.STAR_TAG)) {
			if (this.strong) {
				this.collisionPosition = other.transform.position;
				this.SetLifeTime (0);
				Reflect (this.transform.position - other.transform.position);
			} else {
				this.transform.position = other.transform.position + (this.transform.position - other.transform.position).normalized * ((other.transform.localScale.x / 2) + this.playerRadius);
				Reflect (this.transform.position - other.transform.position);
				SetActionState (ActionState.FLOATING);
			}

		}else if (other.CompareTag (GameManager.CRACK_TAG)) {
			if (this.strong) {
				//this.collisionPosition = other.transform.position;
				//this.SetLifeTime (0);
				other.GetComponent<Crack> ().Hp--;
				if (other.GetComponent<Crack> ().Hp <= 0) {
					//破壊
					this.crackCount ++;
					AudioManager.Instance.PlaySE ("SE_BlastMonster");
					AudioManager.Instance.SetSEPitch ("SE_BlastMonster", this.crackCount);
					other.GetComponent<Crack>().Dead();
				} else {
					Reflect (this.transform.position - other.transform.position);
				}
			} else {
				this.transform.position = other.transform.position + (this.transform.position - other.transform.position).normalized * ((other.transform.localScale.x / 2) + this.playerRadius);
				Reflect (this.transform.position - other.transform.position);
				SetActionState (ActionState.FLOATING);
			}

		} else if (other.CompareTag (GameManager.METEO_TAG)) {
			if (this.strong) {
				this.collisionPosition = other.transform.position;
				this.SetLifeTime (0);
				Reflect (this.transform.position - other.transform.position);
			} else {
				Dead ();
			}

		} else if (other.CompareTag (GameManager.MONSTER_TAG)) {
			Monster monster = other.GetComponent<Monster> ();
			if (!monster.hasBlasted) {
				if (this.strong) {
					if (!(monster.Dead (this.playerRigidbody.velocity.normalized))) {
						this.collisionPosition = other.transform.position;
						this.SetLifeTime (0);
						Reflect (this.transform.position - other.transform.position);
					} else {
						AudioManager.Instance.PlaySE ("SE_ImpactStar");
						hitStop = 0.5f;
						BlastedMonster ();
					}
				} else {
					Dead ();
				}
			}

		} else if (other.CompareTags (GameManager.WALL_HORIZONTAL_TAG, GameManager.WALL_VERTICAL_TAG)) {
			if (this.strong && this.remainReflectable > 0) {
				this.collisionPosition = other.transform.position;
				this.SetLifeTime (0);
				Reflect (other.CompareTag(GameManager.WALL_HORIZONTAL_TAG) ? new Vector2(1, 0) : new Vector2(0, 1));
			} else {
				Reflect (other.CompareTag(GameManager.WALL_HORIZONTAL_TAG) ? new Vector2(1, 0) : new Vector2(0, 1));
				SetActionState (ActionState.FLOATING);
			}

		} else if (other.CompareTag (GameManager.WALL_CAMERA_UP_TAG)) {
			if (transform.position.y - other.gameObject.transform.position.y < 0) {
				this.mainCamera.GetComponent<MainCamera> ().Up();
				this.finishStrong ();
				playerRigidbody.velocity = Vector2.zero;
				iTween.MoveTo (this.gameObject, new Vector2 (this.transform.position.x, other.transform.position.y + 2.0f * (other.transform.localScale.y / 2.0f)), 0.5f);
				SetActionState (ActionState.NONE);

				//Animation Restart
				this.GetComponent<Animator> ().enabled = false;
				this.GetComponent<Animator> ().enabled = true;
			}

		} else if (other.CompareTag (GameManager.WALL_CAMERA_DOWN_TAG)) {
			if (transform.position.y - other.gameObject.transform.position.y > 0) {
				if (this.strong && this.remainReflectable > 0) {
					this.collisionPosition = other.transform.position;
					this.SetLifeTime (0);
					Reflect (new Vector2(0, 1));
				} else {
					Reflect (new Vector2(0, 1));
					SetActionState (ActionState.FLOATING);
				}
			}
		} else if (other.CompareTag(GameManager.CANDY_TAG)) {
			AudioManager.Instance.PlaySE ("SE_GetItem", 0.5f);
			hpBar.Recover ();
			Destroy (other.gameObject);
		}
	}

	public void Dead() {
		if (this.alive) {
			AudioManager.Instance.StopSE ();
			AudioManager.Instance.PlaySE ("SE_Dead3");
			this.alive = false;
			var obj = GameObject.Instantiate (explosion, this.transform.position, this.transform.localRotation) as GameObject;
			obj.GetComponent<DestroyEffect> ().isGameOver = true;
			this.gameObject.SetActive (false);
		}
	}

	public void SetIsClear (bool isClear) {
		this.isClear = isClear;
	}

	public bool GetIsStrong () {
		return this.strong;
	}

	void Reflect(Vector2 _normalVector) {
		AudioManager.Instance.PlaySE ("SE_Impact");
		this.playerRigidbody.velocity = Vector2.Reflect (this.playerRigidbody.velocity, _normalVector.normalized);
		remainReflectable--;
		Debug.Log ("<color=green>remainReflectable: " + remainReflectable + "</color>");
		if (remainReflectable == 0) {
			this.finishStrong ();
		}
		GameObject.Instantiate ((this.strong ? shockEffectStrong : shockEffect), this.transform.position, this.transform.rotation);
	}

	void finishStrong() {
		strong = false;
		this.collisionPosition = new Vector3(1000, 1000, 0);
		this.GetComponent<SpriteRenderer> ().sprite = this.defaultImage;
		if (playerRigidbody.velocity.sqrMagnitude > 0) {
			playerRigidbody.velocity = playerRigidbody.velocity.normalized * SPEED_LOW;
		}

		var monsters = GameObject.FindGameObjectsWithTag (GameManager.MONSTER_TAG);
		foreach (var monster in monsters) {
			monster.GetComponent<Monster> ().FinishPlayerStrong ();
		}
	}

	void SetActiveTouchArea(bool active) {
		this.touchArea.SetActive (active);
	}

	void BlastedMonster() {
		var monsterObjects = GameObject.FindGameObjectsWithTag (GameManager.MONSTER_TAG);
		foreach (var monster in monsterObjects) {
			if (!(monster.GetComponent<Monster> ().hasBlasted)) {
				return;
			}
		}

		//モンスター全滅
		GameManager.Instance.gameState = GameManager.GameState.CLEAR;
		StartCoroutine(DelayMethod(1.0f, ()=>{
			Debug.Log("OpenGate");
			OpenGate();
			GoUp();
		}));

		StartCoroutine (DelayMethod (2.0f, () => {
			//prepare clear logo
			CanvasManager.Instance.SetLogo (GameManager.GameState.CLEAR);
		}));
	}

	IEnumerator DelayMethod(float waitTime, Action action) {
		yield return new WaitForSeconds(waitTime);
		action();
	}

	void OpenGate() {
		var gateStars = GameObject.FindObjectsOfType<GateStar> ();
		foreach (var gateStar in gateStars) {
			gateStar.OpenGate (1.0f);
		}
	}

	void GoUp() {
		SetIsClear (true);
		this.playerRigidbody.velocity = Vector2.up;
		this.finishStrong ();
//		this.SetActionState (ActionState.RELEASE);
	}

	void SetLifeTime(float lifeTime) {
		this.strongEffect.GetComponent<ParticleSystem> ().startLifetime = lifeTime;
	}
}
