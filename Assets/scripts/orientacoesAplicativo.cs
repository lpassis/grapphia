using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class orientacoesAplicativo : MonoBehaviour {
	int tempoEspera;

	// Use this for initialization
	void Start () {
		StartCoroutine ("telaEnd");
	}
	
	// Update is called once per frame
	void Update () {
		StartCoroutine ("telaEnd");
	}

	IEnumerator telaEnd(){
		tempoEspera = 5;
		yield return new WaitForSeconds (tempoEspera);
		SceneManager.LoadScene ("telaInicialOpcoes");
	}

	//Tela de créditos e sobre aplicativo 
	public void btnSobreAplicativo (){
		SceneManager.LoadScene ("");
	}
}
