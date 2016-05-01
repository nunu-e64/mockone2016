using UnityEngine;
using System.Collections;

public class Title : MonoBehaviour {

	public void OnClick () {
		GameManager.Instance.ChangeScene (GameManager.SceneName.Prologue.ToString ());
	}
}
