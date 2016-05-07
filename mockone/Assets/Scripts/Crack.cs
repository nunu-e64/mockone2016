using UnityEngine;
using System.Collections;

public class Crack : MonoBehaviour {

	public int Hp = 1;
	[SerializeField]
	private GameObject crackPiece;	//破片

	public void Dead() {
		GameObject.Instantiate (crackPiece, this.transform.position, this.transform.localRotation);
		Destroy (gameObject);
	}
}
