using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.SceneManagement;

// Classe que controla todos os elementos gráficos do jogo!
public class ditado : MonoBehaviour
{

	public GameObject animacaoProfessora;
	//para animação de professora

	private int idPalavra;

	public InputField textoDigitado;

	private int acertoPalavra, erroPalavra;

	int cont;


	//---------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------

	// Inicializa a tela do jogo!
	void Start ()
	{
		
	}


	//---------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------

	// Vai ser chamada a cada frame por segundo!
	void FixedUpdate ()
	{
		
	}

	public void PegaTexto ()
	{
			idPalavra = Random.Range (0, bancoPalavras.Instance.qtd_Words); // Random para colocar palavra inicial aleatória
			string palavraAnalisada = bancoPalavras.Instance.palavras [dadosJogo.Instance.currentUser.Nivel] [idPalavra].palavra_completa;
			//Palavra digitada:
			string texto = textoDigitado.text;
			string upperString = texto.ToUpper ();//tornar os caracteres maiusculos assim como no BD

			if (palavraAnalisada == upperString) {
				Debug.Log ("Acertou");
				acertoPalavra++;
			} else {
				Debug.Log ("Errou");
				erroPalavra++;
			}
	}

	public void pressedAudioPalavra ()
	{
		//Para animação
		StartCoroutine ("audioEnd");

		string arquivo = "audiosditado/" + bancoPalavras.Instance.palavras [dadosJogo.Instance.currentUser.Nivel] [idPalavra].nome_audio_ditado;

		AudioClip clip = (AudioClip)Resources.Load (arquivo);
		AudioSource audio;
		audio = gameObject.AddComponent<AudioSource> ();
		audio.volume = 1;
		audio.clip = clip;
		audio.Play ();


	}

	IEnumerator audioEnd ()
	{
		animacaoProfessora.SetActive (true);
		string arquivo = "audiosditado/" + bancoPalavras.Instance.palavras [dadosJogo.Instance.currentUser.Nivel] [idPalavra].nome_audio_ditado;
		AudioClip clip = (AudioClip)Resources.Load (arquivo);

		yield return new WaitForSeconds (clip.length);
		animacaoProfessora.SetActive (false);
	}
}
