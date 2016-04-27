using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;

public class TouchManager : MonoBehaviour {

	[SerializeField]
	private GameObject mainCamera;
	[SerializeField]
	private GameObject movePlayer;
	[SerializeField]
	private GameObject touchObject;
	[SerializeField]
	private float touchObjectRadius;

	private float TOUCH_INTERVAL;

	private float interval;

	// Use this for initialization
	void Start () {
		this.TOUCH_INTERVAL = GameManager.Instance.TOUCH_INTERVAL;
		this.GetComponent<AudioManager> ();
		AudioManager.Instance.PlayBGM ("stage");
	}
	// タップで引力点の生成or消滅
	void Update () {
		interval += Time.deltaTime;

		//タップとクリックの検出
		bool hasTapped = true;
		Vector3 tapPosition = new Vector3();
		if (Input.GetMouseButtonDown(0)) {
			tapPosition = Input.mousePosition;
		} else if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began) {
			tapPosition = Input.GetTouch(0).position;
		} else {
			hasTapped = false;
		}

		if (hasTapped && Time.timeScale > 0) {
			if (GameManager.Instance.gameState == GameManager.GameState.GAME_START) {
				CanvasManager.Instance.SetLogo (GameManager.GameState.PLAYING);
			} else if (GameManager.Instance.gameState == GameManager.GameState.PLAYING) {
				//UIタップ時は判定しない
				if (!EventSystem.current.IsPointerOverGameObject () && !(Input.touchCount > 0 && EventSystem.current.IsPointerOverGameObject(Input.GetTouch(0).fingerId))) {
					//タップ座標の取得と変換
					Vector3 mouseScreenPos = tapPosition;
					mouseScreenPos.z = -mainCamera.transform.position.z;
					Vector3 touchPos = Camera.main.ScreenToWorldPoint (mouseScreenPos);

					//タップ位置に障害物がなかった時だけ処理
					int layer = LayerMask.NameToLayer (GameManager.TAP_TARGET_LAYER);
					var touchedCollider = Physics2D.OverlapPoint (touchPos, 1 << layer);
					if (touchedCollider)
						Debug.Log (touchedCollider.name);
					if (GameObject.FindGameObjectsWithTag (GameManager.TOUCH_OBJECT_TAG).Length == 0) {					
						if ((touchedCollider && touchedCollider.CompareTag (GameManager.TOUCH_FIELD_TAG))) {
							Debug.Log (touchedCollider.name);
							this.CreateGravitation (touchPos);
						}
					} else {
						this.ReleaseGravitation (touchPos);
					}
				}
			}
		}
	}

	void CreateGravitation (Vector3 _touchPos) {
		if (interval > TOUCH_INTERVAL) {
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