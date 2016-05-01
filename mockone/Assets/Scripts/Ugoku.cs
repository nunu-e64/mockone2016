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

	// Use this for initialization
	new void Start () {
		base.Start ();
		isFirst = true;
		t = 0f;
		startPosition = this.transform.position;
		this.changeMoveStyle(isFirst);
		isInvincible = false;
	}
	
	new void Update() {
		base.Update();

		this.changeMoveStyle (isFirst);		//DEBUG: for パラメータ調整しやすくするため毎回反映
		if (!hasBlasted) {
			t += Time.deltaTime;
			float x = Mathf.Cos (t * speed / size) * size;
			float y = Mathf.Sin (t * speed / size) * size;

			switch (moveStyle) {
			case MoveStyle.LEFT_CIRCLE:
				this.transform.position = startPosition + new Vector3 (x, y, 0);
				break;
			case MoveStyle.RIGHT_CIRCLE:
				this.transform.position = startPosition + new Vector3 (x, -y, 0);
				break;
			case MoveStyle.HORIZONTAL:
				this.transform.position = startPosition + new Vector3 (x, 0, 0);
				break;
			case MoveStyle.VERTICAL:
				this.transform.position = startPosition + new Vector3 (0, y, 0);
				break;
			case MoveStyle.NANAME_RIGHTTOP:
				this.transform.position = startPosition + new Vector3 (x, x, 0);
				break;
			case MoveStyle.NANAME_LEFTTOP:
				this.transform.position = startPosition + new Vector3 (x, -x, 0);
				break;
			}
		}
	}

	public override void Dead(Vector2 hitDirection) {
		if (!hasBlasted && !isInvincible) {
			if (isFirst) {
				this.changeMoveStyle (false);
				isInvincible = true;
			} else {
				base.Dead (hitDirection);
			}
		}
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
		isInvincible = false;
	}
}
