using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System;

public class StageSelect : MonoBehaviour
{

	[SerializeField]
	private GameObject[] stages;
	[SerializeField]
	private GameObject epilogue;

	// Use this for initialization
	void Start ()
	{
		AudioManager.Instance.PlayBGM ("BGM_StageSelect", 0.5f);
		for (int i = 0; i < stages.Length; i++) {
			string stageName = stages [i].name;
			stages [i].GetComponent<Button> ().onClick.AddListener (() => {
				AudioManager.Instance.PlaySE ("SE_PushStageButton");
				GameManager.Instance.ChangeScene (stageName);
			});	
		}

		epilogue.GetComponent<Button> ().onClick.AddListener (() => {
			if (PlayerPrefsManager.Instance.GetClearStage () == Enum.GetValues (typeof(GameManager.StageName)).Length) {
				AudioManager.Instance.PlaySE ("SE_PushStageButton");
				GameManager.Instance.ChangeScene (GameManager.SceneName.Epilogue.ToString ());
			}
		});	
	}
}
