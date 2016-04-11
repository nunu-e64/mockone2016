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
		this.GetComponent<AudioManager> ();
		AudioManager.Instance.PlayBGM ("stage");
	}
	
	// Update is called once per frame
	void Update () {
		if (Input.GetMouseButtonDown (0)) {
			//タップ座標の取得と変換
			Vector3 mouseScreenPos = Input.mousePosition;
			mouseScreenPos.z = -mainCamera.transform.position.z;
			Vector3 touchPos = Camera.main.ScreenToWorldPoint(mouseScreenPos);

			if (GameObject.FindGameObjectsWithTag (TOUCH_OBJECT_TAG).Length == 0) {
				this.CreateGravitation (touchPos);
			} else {
				this.ReleaseGravitation (touchPos);
			}
		}
	}

	void CreateGravitation (Vector3 _touchPos) {
		if ((_touchPos - movePlayer.transform.position).sqrMagnitude > touchObjectRadius*touchObjectRadius) {
			GameObject obj = Instantiate (touchObject, _touchPos, Quaternion.identity) as GameObject;
			obj.GetComponent<TouchObject> ().Init (touchObjectRadius);
			movePlayer.GetComponent<MovePlayer> ().SetActionState (MovePlayer.ActionState.MOVE, obj);
		}
	}

	void ReleaseGravitation (Vector3 _touchPos) {
		movePlayer.GetComponent<MovePlayer> ().SetActionState (MovePlayer.ActionState.RELEASE);
		GameObject[] touchObjects = GameObject.FindGameObjectsWithTag (TOUCH_OBJECT_TAG);
		foreach (GameObject touchObject in touchObjects) {
			var touchObjectClass = touchObject.GetComponent<TouchObject> ();
			if (touchObjectClass) {
				touchObjectClass.Reset ();
			}
		}
	}
}