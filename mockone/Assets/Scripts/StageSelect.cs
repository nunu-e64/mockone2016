using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System;

public class StageSelect : MonoBehaviour {

	[SerializeField]
	private GameObject[] stages;

	// Use this for initialization
	void Start () {
		for (int i = 0; i < stages.Length; i++) {
			string stageName = stages[i].name;
			stages[i].GetComponent<Button> ().onClick.AddListener (() => {
				GameManager.Instance.ChangeScene (stageName);
			});	
		}
	}
}
