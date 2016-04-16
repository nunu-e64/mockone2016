using UnityEngine;
using System.Collections;

public class TouchManager : MonoBehaviour {

	[SerializeField]
	private GameObject mainCamera;
	[SerializeField]
	private GameObject movePlayer;
	[SerializeField]
	private GameObject touchObject;
	[SerializeField]
	private float touchObjectRadius;

	private const float TOUCH_INTERVAL = 1.0f;
	private float interval;
	private const float UNTAPABLE_EDGE_WIDTH = 1.0f;

	// Use this for initialization
	void Start () {
		this.GetComponent<AudioManager> ();
		AudioManager.Instance.PlayBGM ("stage");
	}
	
	// タップで引力点の生成or消滅
	void Update () {
		interval += Time.deltaTime;
		if (Input.GetMouseButtonDown (0)) {
			if (!movePlayer.GetComponent<MovePlayer> ().alive) {	//DEBUG: 死亡時にはリトライ
				movePlayer.GetComponent<MovePlayer> ().Init();
			}

			//タップ座標の取得と変換
			Vector3 mouseScreenPos = Input.mousePosition;
			mouseScreenPos.z = -mainCamera.transform.position.z;
			Vector3 touchPos = Camera.main.ScreenToWorldPoint(mouseScreenPos);

			//タップ位置に障害物がなかった時だけ処理
			Collider2D touchedCollider = Physics2D.OverlapPoint(touchPos);
			if (GameObject.FindGameObjectsWithTag (GameManager.TOUCH_OBJECT_TAG).Length == 0) {
				
				if ((touchedCollider && (!touchedCollider.CompareTags (GameManager.STAR_TAG, GameManager.METEO_TAG, GameManager.MONSTER_TAG)))
				    || !touchedCollider) {
					this.CreateGravitation (touchPos);
				}
			} else {
				if ((touchedCollider && touchedCollider.CompareTag(GameManager.TOUCH_OBJECT_TAG))) {
					this.ReleaseGravitation (touchPos);
				}
			}
		}
	}

	void CreateGravitation (Vector3 _touchPos) {
		var edge = -1 * Camera.main.ScreenToWorldPoint (Vector3.zero).x;
		if (interval > TOUCH_INTERVAL && _touchPos.x > -edge + UNTAPABLE_EDGE_WIDTH && _touchPos.x < edge - UNTAPABLE_EDGE_WIDTH) {
			GameObject obj = Instantiate (touchObject, _touchPos, Quaternion.identity) as GameObject;
			obj.GetComponent<TouchObject> ().Init (touchObjectRadius);
			movePlayer.GetComponent<MovePlayer> ().SetActionState (MovePlayer.ActionState.MOVE, obj);
		}
	}

	void ReleaseGravitation (Vector3 _touchPos) {
		interval = 0;
		movePlayer.GetComponent<MovePlayer> ().SetActionState (MovePlayer.ActionState.RELEASE);
	}
}