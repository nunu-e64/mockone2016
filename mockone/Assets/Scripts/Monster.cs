using UnityEngine;
using System.Collections;

public class Monster : MonoBehaviour {

	public bool hasBlasted { get; private set;}
	float blastedRotationAngle;

	// Use this for initialization
	protected void Start () {
		hasBlasted = false;
		blastedRotationAngle = GameManager.Instance.MONSTER_BLAST_ROTATE;
	}
	
	// Update is called once per frame
	protected void Update () {
		if (hasBlasted) {
			this.gameObject.transform.Rotate (new Vector3 (0f, 0f, blastedRotationAngle));
		}
	}

	public virtual bool Dead (Vector2 hitDirection) {
		this.GetComponent<Animator> ().Stop ();
		this.gameObject.GetComponent<Rigidbody2D>().velocity = Quaternion.Euler(0, 0, 30) * hitDirection * GameManager.Instance.MONSTER_BLAST_SPEED;
		hasBlasted = true;
		return hasBlasted;
	}

	public virtual void FinishPlayerStrong() {
	}
}
