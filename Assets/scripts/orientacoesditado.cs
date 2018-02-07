using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class orientacoesditado : MonoBehaviour {
	public AudioClip sound;

	// Use this for initialization
	void Start () {
		AudioSource audio = GetComponent<AudioSource> ();
		audio.Play ();
		StartCoroutine ("audioEnd");
	}

	IEnumerator audioEnd(){
		yield return new WaitForSeconds (sound.length);
		SceneManager.LoadScene ("telaDitado");
	}

}
