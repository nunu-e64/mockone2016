using UnityEngine;
using System.Collections;

public class Nicoichi : Monster {

	public bool hasKnockDowned { get; private set;}
	private Nicoichi buddy = null;

	[SerializeField]
	private Sprite animatedImage;
	[SerializeField]
	private Sprite damagedImage;
	private Sprite defaultImage;

	private float animationTime = 0.0f;

	[SerializeField]
	private GameObject monsterLight;

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

		animationTime = 1.4f;
	}

	new void Update() {
		base.Update();
		animationTime += Time.deltaTime;

		if (!hasKnockDowned) {
			if (animationTime >= 2.0f) {
				this.gameObject.GetComponent<SpriteRenderer> ().sprite = defaultImage;
				animationTime = animationTime % 2.0f;
			} else if (animationTime >= 1.0f) {
				this.gameObject.GetComponent<SpriteRenderer> ().sprite = animatedImage;
			}
		}
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
				if (monsterLight != null && monsterLight.activeSelf) {
					monsterLight.SetActive (true);
				}
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
 