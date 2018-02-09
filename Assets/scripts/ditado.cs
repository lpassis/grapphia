using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.SceneManagement;
using System.IO;

// Classe que controla todos os elementos gráficos do jogo!
public class ditado : MonoBehaviour
{

	public GameObject animacaoProfessora;
	//para animação de professora

	private int idPalavra;

	public InputField textoDigitado;

	private int acertoPalavra, erroPalavra;


	//---------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------

	// Inicializa a tela do jogo!
	void Start ()
	{
		idPalavra = Random.Range (0, bancoPalavras.Instance.qtd_Words); // Random para colocar palavra inicial aleatória
	}


	//---------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------

	public void PegaTexto (){		
		string palavraAnalisada = bancoPalavras.Instance.palavras [dadosJogo.Instance.currentUser.Nivel] [idPalavra].palavra_completa;
		string texto = textoDigitado.text;
		string upperString = texto.ToUpper ();

		if (palavraAnalisada == upperString) {
			Debug.Log ("Acertou");
			++dadosJogo.Instance.currentUser.scoreDitado;
			textoDigitado.text = "";
			//Definir o número de palabras do ditado no comandosBasicos.cs
			if (dadosJogo.Instance.currentUser.scoreDitado == bancoPalavras.Instance.numWordsDitado) {
				Debug.Log ("Fim do ditado");
				//SceneManager.LoadScene (); cena de parabéns completou o ditado			
			}
		} else {
			erroPalavra++;
			textoDigitado.text = "";
		}

		//Volta para a função void Start() para atualizar o audio
		StartCoroutine ("Start");
	}

	public void pressedAudioPalavra (){
		animacaoProfessora.SetActive (true);
		StartCoroutine ("audioEnd");
		string arquivo = "audiosditado/" + bancoPalavras.Instance.palavras [dadosJogo.Instance.currentUser.Nivel] [idPalavra].nome_audio_ditado;

		AudioClip clip = (AudioClip)Resources.Load (arquivo);
		AudioSource audio;
		audio = gameObject.AddComponent<AudioSource> ();
		audio.volume = 1;
		audio.clip = clip;
		audio.Play ();
	}	

	IEnumerator audioEnd(){
		string arquivo = "audiosditado/" + bancoPalavras.Instance.palavras [dadosJogo.Instance.currentUser.Nivel] [idPalavra].nome_audio_ditado;

		AudioClip clip = (AudioClip)Resources.Load (arquivo);
		yield return new WaitForSeconds (clip.length);
		animacaoProfessora.SetActive (false);
	}
}
