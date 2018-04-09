using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class gameController3 : MonoBehaviour {
	public GameObject caranguejo;
	public GameObject caranguejo2;

	//Referenciando os caranguejos
	private float xCaranguejo;
	private float yCaranguejo;

	private float xCaranguejo2;
	private float yCaranguejo2;

	private float xCaranguejo_Inicial;
	private float yCaranguejo_Inicial;

	private float xCaranguejo2_Inicial;
	private float yCaranguejo2_Inicial;

	//Referenciando as letras contendo as opções
	private float xLetraOpcao1;
	private float yLetraOpcao1;

	private float xLetraOpcao2;
	private float yLetraOpcao2;

	private float xLetraOpcao1_Inicial;
	private float yLetraOpcao1_Inicial;

	private float xLetraOpcao2_Inicial;
	private float yLetraOpcao2_Inicial;


	/*Referenciando os personagens
	public GameObject playerMasculino;
	public GameObject playerFeminino;

	//Referenciando os personagens laçando a letra correspondente 1 e 2 : duas opções para cada palavra
	public GameObject playerMasculino_lacando1;
	public GameObject playerMasculino_lacando2;

	public GameObject playerFeminino_lacando1;
	public GameObject playerFeminino_lacando2;

	//Referenciando botão de pause
	public GameObject menuPause;

	//Referenciando a mensagem inicial contendo orientações
	public GameObject mensagemInicial;
	public GameObject mensagemInicial_frase;

	//Referenciando caixa onde conterá a palavra
	public Text txtPalavraCompleta;*/


	void Start () {
		//Inicializando as posições dos objetos
		xCaranguejo_Inicial = xCaranguejo = caranguejo.transform.position.x;
		yCaranguejo_Inicial = yCaranguejo = caranguejo.transform.position.y;

		xCaranguejo2_Inicial = xCaranguejo2 = caranguejo.transform.position.x;
		yCaranguejo2_Inicial = yCaranguejo2 = caranguejo.transform.position.y;
				
	}
	
	// Update is called once per frame
	void FixedUpdate () {
		if (xLetraOpcao1 >= -2f) {
			xLetraOpcao1 -= 0.125f; //velocidade da caixa 1
		} else if (yLetraOpcao1 <= -1.7f) {
			yLetraOpcao1 += 0.090f;
			yCaranguejo -= 0.06f;
		}

		if (xLetraOpcao2 >= -2f) {
			xLetraOpcao2 -= 0.125f; //velocidade da caixa 2
		} else if (yLetraOpcao1 <= -1.7f) {
			yLetraOpcao2 += 0.090f;
			yCaranguejo2 -= 0.06f;
		}

		xCaranguejo -= 0.125f;
		xCaranguejo -= 0.125f;
	}
}
