using UnityEngine;
using System.Collections;

public class Nicoichi : Monster {

	public bool hasKnockDowned { get; private set;}
	private Nicoichi buddy = null;

	[SerializeField]
	private Sprite damagedImage;

	// Use this for initialization
	void Start () {
		base.Start ();
		hasKnockDowned = false;

		var nicoichis = GameObject.FindGameObjectsWithTag(GameManager.MONSTER_NIKOICHI_TAG);
		foreach (var monster in nicoichis) {
			if (monster != this.gameObject) {
				Debug.Log (this.name + ":" + monster.name);	
				buddy = monster.GetComponent<Nicoichi>();
			}
		}

		if (buddy == null) {
			Debug.LogError ("Nicoichi : Not Found buddy");
		}
	}

	void Update() {
		base.Update();
	}

	public override void Dead(Vector2 hitDirection) {
		Debug.Log (this.name + ":Nicoichi Dead");
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
	}
}
 