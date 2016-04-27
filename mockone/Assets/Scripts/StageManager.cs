using UnityEngine;
using System.Collections;

public class StageManager : MonoBehaviour {

	[SerializeField]
	private GameObject goal;

	void Start () {
		goal.SetActive (false);
	}

	void Update() {
		var monster = GameObject.FindGameObjectWithTag (GameManager.MONSTER_TAG);
		if (monster == null) {
			goal.SetActive (true);
		}
	}
}
