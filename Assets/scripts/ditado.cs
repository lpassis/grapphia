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
		Debug.Log(idPalavra);
	}


	//---------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------

	//Função criada por Magno 
	public void PegaTexto (){		
		string palavraAnalisada = bancoPalavras.Instance.palavras [dadosJogo.Instance.currentUser.Nivel] [idPalavra].palavra_completa;
		string texto = textoDigitado.text;
		string upperString = texto.ToUpper();

		if (palavraAnalisada == upperString) {
			Debug.Log ("Acertou");
			++dadosJogo.Instance.currentUser.scoreDitado;
			textoDigitado.text = "";
		} else {
			erroPalavra++;
			textoDigitado.text = "";
		}

		bancoPalavras.Instance.numWordsDitado--;
		StartCoroutine ("Start");

		//Definir o numero de palavras no ditado no comandosBasicos.cs
		if (bancoPalavras.Instance.numWordsDitado == 0) {
			Debug.Log ("Fim do ditado");
			SceneManager.LoadScene ("telaFimJogo");
		}
	}

	//Função criada por Magno
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
