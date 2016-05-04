using UnityEngine;
using System.Collections;

public class Player : MonoBehaviour {

	private float first_speed = 10.0f;
	private Rigidbody playerRigidbody;
	private GameObject playerCamera;
	public GameObject star;
	private GameObject activeStar;

	private float power;
	private float cameraSpeed = 1.0f;

	public bool furiko = false;
	private float gravity = 10f;

	// Use this for initialization
	void Start () {
		playerRigidbody = this.GetComponent<Rigidbody> ();
		playerRigidbody.velocity = new Vector3 (0, first_speed, 0);
		playerCamera = this.GetComponentInChildren<Camera> ().gameObject;
	}
	
	// Update is called once per frame
	void FixedUpdate () {
		if (Input.GetMouseButtonDown(0)) {
			var mouseScreenPos = Input.mousePosition;
			mouseScreenPos.z = 30.0f;
			var mouseWorldPos = Camera.main.ScreenToWorldPoint(mouseScreenPos);
			activeStar = GameObject.Instantiate (star, mouseWorldPos, star.transform.localRotation) as GameObject;

			playerCamera.transform.parent = null;
			power = playerRigidbody.velocity.sqrMagnitude / ((transform.position - activeStar.transform.position).magnitude * playerRigidbody.mass);
		}

		if (Input.GetMouseButtonUp (0)) {
			playerCamera.transform.parent = this.transform;
		}

		if (Input.GetMouseButton (0)) {
			if (furiko) {
				Vector3 nowPower = (activeStar.transform.position - transform.position).normalized * power
				                   + Vector3.down * gravity / playerRigidbody.mass;
				playerRigidbody.AddForce (nowPower);
			} else {
				playerRigidbody.AddForce ((activeStar.transform.position - transform.position).normalized * power);
			}
		} else {
			var cameraLocalPosition = playerCamera.transform.localPosition;
			var cameraLocalPositionZ = cameraLocalPosition.z;
			cameraLocalPosition.z = 0;

			if (cameraLocalPosition.magnitude > cameraSpeed) {
				cameraLocalPosition -= (cameraLocalPosition).normalized * cameraSpeed;
			} else if (cameraLocalPosition.magnitude > 0) {
				cameraLocalPosition = Vector3.zero;
			}

			cameraLocalPosition.z = cameraLocalPositionZ;
			playerCamera.transform.localPosition = cameraLocalPosition;
		}
			

	}
}