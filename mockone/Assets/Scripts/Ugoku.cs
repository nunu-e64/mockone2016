using UnityEngine;
using System.Collections;

public class Ugoku : Monster {

	private enum MoveStyle {
		LEFT_CIRCLE,	//左回り円軌道
		RIGHT_CIRCLE,	//右回り円軌道
		HORIZONTAL,		//横に移動
		VERTICAL,		//縦に移動
		NANAME_RIGHTTOP,
		NANAME_LEFTTOP
	}

	[SerializeField]
	private MoveStyle FIRST_MOVE_STYLE = MoveStyle.LEFT_CIRCLE;
	[SerializeField]
	private float FIRST_SIZE = 2.0f;	//移動半径もしくは移動距離の半分
	[SerializeField]
	private float FIRST_SPEED = 5.0f;
	[SerializeField]
	private MoveStyle SECOND_MOVE_STYLE = MoveStyle.HORIZONTAL;
	[SerializeField]
	private float SECOND_SIZE = 2.0f;	//移動半径もしくは移動距離の半分
	[SerializeField]
	private float SECOND_SPEED = 5.0f;
	[SerializeField, Space(10)]	
	private bool isFirst = true;	//DEBUG: for パラメータ調整

	private MoveStyle moveStyle;
	private float size;
	private float speed;
	private float t;
	private Vector3 startPosition;
	private bool isInvincible;
	private bool hasPlayerStrongFinished;
	private bool isInvincibleMoving;

	// Use this for initialization
	new void Start () {
		base.Start ();
		isFirst = true;
		t = 0f;
		startPosition = this.transform.position;
		this.changeMoveStyle(isFirst);
		isInvincible = false;
		hasPlayerStrongFinished = false;
	}
	
	new void Update() {
		base.Update();

		//一度ダメージを受けてたら色を変える
		if (this.isFirst) {
			this.GetComponent<SpriteRenderer> ().color = new Color (1, 1, 1, 1);
		} else {
			this.GetComponent<SpriteRenderer> ().color = new Color (255 / 255.0f, 170 / 255.0f, 70 / 255.0f, 1);
		}

		//無敵状態なら点滅
		if (this.isInvincible) {
			float alpha = (Mathf.Sin (Mathf.PI * t / 0.2f) + 1) / 2.0f;
			this.GetComponent<SpriteRenderer> ().color = new Color(1, 1, 1, alpha);
		}

		this.changeMoveStyle (isFirst);		//DEBUG: for パラメータ調整しやすくするため毎回反映
		if (!hasBlasted && !isInvincible) {
			this.transform.position = startPosition + GetMoveDiff (this.t);
		}

		//無敵解除後に位置が飛ばないようにループの最後でインクリメント
		t += Time.deltaTime;
	}

	private Vector3 GetMoveDiff(float t) {
		float x = Mathf.Cos (t * speed / size) * size;
		float y = Mathf.Sin (t * speed / size) * size;
//		Debug.Log (t);

		switch (moveStyle) {
		case MoveStyle.LEFT_CIRCLE:
			return new Vector3 (x, y, 0);
			break;
		case MoveStyle.RIGHT_CIRCLE:
			return new Vector3 (x, -y, 0);
			break;
		case MoveStyle.HORIZONTAL:
			return new Vector3 (y, 0, 0);
			break;
		case MoveStyle.VERTICAL:
			return new Vector3 (0, y, 0);
			break;
		case MoveStyle.NANAME_RIGHTTOP:
			return new Vector3 (y, y, 0);
			break;
		case MoveStyle.NANAME_LEFTTOP:
			return new Vector3 (y, -y, 0);
			break;
		}
		return Vector3.zero;
	}

	public override bool Dead(Vector2 hitDirection) {
		if (!hasBlasted && !isInvincible) {
			if (isFirst) {
				this.changeMoveStyle (false);
				isInvincible = true;
				isInvincibleMoving = true;
				Hashtable hash = new Hashtable ();
				hash.Add ("oncompletetarget", this.gameObject);
				hash.Add ("oncomplete", "FinishInvincibleMoving");
				hash.Add ("time", 1.0f);
				hash.Add ("position", startPosition + GetMoveDiff(0f));
				iTween.MoveTo (this.gameObject, hash);
			} else {
				base.Dead (hitDirection);
			}
		}
		return hasBlasted;
	}

	private void changeMoveStyle(bool _isFirst) {
		isFirst = _isFirst;
		if (isFirst) {
			moveStyle = FIRST_MOVE_STYLE;
			size = FIRST_SIZE;
			speed = FIRST_SPEED;
		} else {
			moveStyle = SECOND_MOVE_STYLE;
			size = SECOND_SIZE;
			speed = SECOND_SPEED;
		}
	}

	public override void FinishPlayerStrong() {
		if (isInvincible) {
			hasPlayerStrongFinished = true;
			if (!isInvincibleMoving) {
				isInvincible = false;
				t = 0.0f;
			}
		}
	}

	private void FinishInvincibleMoving() {
		isInvincibleMoving = false;
		if (hasPlayerStrongFinished) {
			isInvincible = false;
			t = 0.0f;
		}
	}
						
}
