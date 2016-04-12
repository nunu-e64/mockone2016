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

	private const string TOUCH_OBJECT_TAG = "TouchObject";
	private const float TOUCH_INTERVAL = 1.0f;
	private float interval;
	private const float UNTAPABLE_EDGE_WIDTH = 1.0f;

	// Use this for initialization
	void Start () {
		this.GetComponent<AudioManager> ();
		AudioManager.Instance.PlayBGM ("stage");
	}
	
	// Update is called once per frame
	void Update () {
		interval += Time.deltaTime;
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