using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ditadoOrientacoes : MonoBehaviour {
	public AudioClip sound;

	// Use this for initialization
	void Start () {
		StartCoroutine ("audioEnd");
	}

	IEnumerator audioEnd(){
		yield return new WaitForSeconds (sound.length);
		SceneManager.LoadScene ("telaDitado");
	}
}
