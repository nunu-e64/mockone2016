using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class StageManager : MonoBehaviour {

	[SerializeField]
	private GameObject goal;

	private List<Monster> monsters;

	void Start () {
		AudioManager.Instance.PlayBGM ("BGM_Stage");
		goal.SetActive (false);

		//すべての敵を事前にリストに格納
		monsters = new List<Monster>();
		var monsterObjects = GameObject.FindGameObjectsWithTag (GameManager.MONSTER_TAG);
		foreach (var monsterObject in monsterObjects) {
			monsters.Add(monsterObject.GetComponent<Monster>());
		}
	}

	void Update() {
		//もし敵が全滅していればゴールを表示
		foreach (var monster in monsters) {
			if (!monster.hasBlasted) {
				return;
			}
		}
		goal.SetActive (true);
	}
}
