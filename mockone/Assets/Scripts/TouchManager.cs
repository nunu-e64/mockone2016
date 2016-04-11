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

	private enum TouchState {
		RELEASE,
		PRESS,
		PRESSING
	}

	private const string TOUCH_OBJECT_TAG = "TouchObject";

	// Use this for initialization
	void Start () {
		if (GameObject.FindGameObjectsWithTag (TOUCH_OBJECT_TAG).Length > 0) {
			Debug.Log (GameObject.FindGameObjectWithTag (TOUCH_OBJECT_TAG).name);
		}
	}
	
	// Update is called once per frame
	void Update () {
		if (Input.GetMouseButtonDown (0)) {
			//引力点がある場合
			Debug.Log(GameObject.FindGameObjectsWithTag (TOUCH_OBJECT_TAG).Length);
			if (GameObject.FindGameObjectsWithTag (TOUCH_OBJECT_TAG).Length == 0) {
				this.SetAction (TouchState.PRESS);
			} else {
				Debug.Log (GameObject.FindGameObjectWithTag (TOUCH_OBJECT_TAG).name);
				this.SetAction (TouchState.RELEASE);
			}
		}
		if (Input.GetMouseButton (0)) {
			this.SetAction (TouchState.PRESSING);
		}
	}

	void SetAction (TouchState _touchState) {
		//タップ座標の取得と変換
		Vector3 mouseScreenPos = Input.mousePosition;
		mouseScreenPos.z = -mainCamera.transform.position.z;
		Vector3 touchPos = Camera.main.ScreenToWorldPoint(mouseScreenPos);
		switch (_touchState) {
		case TouchState.PRESS:
			if (GameObject.FindGameObjectsWithTag(TOUCH_OBJECT_TAG).Length == 0
				&& (touchPos - movePlayer.transform.position).sqrMagnitude > touchObjectRadius*touchObjectRadius) {
				//タップ時はタップポイント作成し自機進行方向をタップポイントへ
				GameObject obj = Instantiate (touchObject, touchPos, Quaternion.identity) as GameObject;
				obj.GetComponent<TouchObject> ().Init (touchObjectRadius);
				movePlayer.GetComponent<MovePlayer> ().SetActionState (MovePlayer.ActionState.MOVE, obj);
			}
			break;
		case TouchState.RELEASE:
			//リリース時には自機のリリースとタップポイントの消滅
			movePlayer.GetComponent<MovePlayer> ().SetActionState (MovePlayer.ActionState.RELEASE);
			GameObject[] touchObjects = GameObject.FindGameObjectsWithTag (TOUCH_OBJECT_TAG);
			if (touchObjects.Length > 0) {
				foreach (GameObject touchObject in touchObjects) {
					var touchObjectClass = touchObject.GetComponent<TouchObject> ();
					if (touchObjectClass) {
						touchObjectClass.Reset ();
					}
				}
			}
			break;
		}
	}
}