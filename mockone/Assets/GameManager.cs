using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour {

	public void ChanageScene(string _name) {
		SceneManager.LoadScene (_name);
	}

}
