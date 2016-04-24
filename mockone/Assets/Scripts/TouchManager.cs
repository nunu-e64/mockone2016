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

	private const float TOUCH_INTERVAL = 1.0f;
	private float interval;

	// Use this for initialization
	void Start () {
		this.GetComponent<AudioManager> ();
		AudioManager.Instance.PlayBGM ("stage");
	}
	// タップで引力点の生成or消滅
	void Update () {
		interval += Time.deltaTime;
		if (Input.GetMouseButtonDown (0) && Time.timeScale > 0) {
			if (GameManager.gameState == GameManager.GameState.GAME_START) {
				CanvasManager.Instance.SetLogo (GameManager.GameState.PLAYING);
			} else if (GameManager.gameState == GameManager.GameState.PLAYING) {
				//UIタップ時は判定しない
				if (!EventSystem.current.IsPointerOverGameObject ()) { //TODO: スマホ対応後はInput.GetTouch(0).fingerIdを渡す)) {
					//タップ座標の取得と変換
					Vector3 mouseScreenPos = Input.mousePosition;
					mouseScreenPos.z = -mainCamera.transform.position.z;
					Vector3 touchPos = Camera.main.ScreenToWorldPoint (mouseScreenPos);

					//タップ位置に障害物がなかった時だけ処理
					Collider2D touchedCollider = Physics2D.OverlapPoint (touchPos);
					if (GameObject.FindGameObjectsWithTag (GameManager.TOUCH_OBJECT_TAG).Length == 0) {
					
						if ((touchedCollider && touchedCollider.CompareTag (GameManager.TOUCH_FIELD_TAG))) {
							Debug.Log (touchedCollider.name);
							this.CreateGravitation (touchPos);
						}
					} else {
						if ((touchedCollider && touchedCollider.CompareTag (GameManager.TOUCH_OBJECT_TAG))) {
							this.ReleaseGravitation (touchPos);
						}
					}
				}
			} else {
				GameManager.Instance.ReloadScene ();
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