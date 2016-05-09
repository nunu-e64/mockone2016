using UnityEngine;
using System.Collections;

public class Nicoichi : Monster {

	public bool hasKnockDowned { get; private set;}
	private Nicoichi buddy = null;

	[SerializeField]
	private Sprite damagedImage;
	private Sprite defaultImage;

	// Use this for initialization
	new void Start () {
		base.Start ();
		hasKnockDowned = false;
		defaultImage = this.GetComponent<SpriteRenderer> ().sprite;

		var nicoichis = GameObject.FindObjectsOfType<Nicoichi> ();
		foreach (var monster in nicoichis) {
			if (monster != this) {
				buddy = monster;
			}
		}

		if (buddy == null) {
			Debug.LogError ("Nicoichi : Not Found buddy");
		}
	}

	new void Update() {
		base.Update();
	}

	public override bool Dead(Vector2 hitDirection) {
		if (!hasBlasted) {
			if (!hasKnockDowned) {
				hasKnockDowned = true;
				this.gameObject.GetComponent<SpriteRenderer> ().sprite = damagedImage;
			}
			if (buddy.hasKnockDowned) {
				base.Dead (hitDirection);
				buddy.Dead (hitDirection);
			}
		}
		return hasBlasted;
	}

	public override void FinishPlayerStrong() {
		if (!hasBlasted && hasKnockDowned) {
			hasKnockDowned = false;
			this.gameObject.GetComponent<SpriteRenderer> ().sprite = defaultImage;
		}
	}
}
 