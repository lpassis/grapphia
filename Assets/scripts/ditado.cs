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

	private int qtd_Palavras_Ditado = bancoPalavras.Instance.ListaIdPalavraAcerto.Count; 

	private int posicao;

	public Text quantidadePalavrasApresentadas;

	public GameObject orientacaoInicial;

	private bool mensagemInicial_Aplicada = false;

	//---------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------

	// Inicializa a tela do jogo!
	void Start ()
	{
		//Desativar o aparecimento constante da mensagem de orientação
		if (mensagemInicial_Aplicada == false) {
			orientacaoInicial.SetActive (true);
		} else {
			orientacaoInicial.SetActive (false);
		}

		Debug.Log (qtd_Palavras_Ditado);
		posicao = Random.Range (0, qtd_Palavras_Ditado);
		idPalavra = bancoPalavras.Instance.ListaIdPalavraAcerto[posicao];
		bancoPalavras.Instance.ListaIdPalavraAcerto.RemoveAt (posicao);
		qtd_Palavras_Ditado--;
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

		int quantidadePalavrasAtual = bancoPalavras.Instance.numWordsDitado - 1;
		quantidadePalavrasApresentadas.text = quantidadePalavrasAtual.ToString();
		quantidadePalavrasAtual--;

		bancoPalavras.Instance.numWordsDitado--;
		StartCoroutine ("Start");

		//Definir o numero de palavras no ditado no comandosBasicos.cs
		if (bancoPalavras.Instance.numWordsDitado == 0) {
			//volta para 5 palavras, caso opte por jogar novamente no mesmo usuário já que essa variável é decrementada
			//durante o processo do ditado
			bancoPalavras.Instance.numWordsDitado = 5;
			//toda vez que o jogo é executado a lista é novamente preenchida com todas as palavras do banco acrescida das palavras do jogo
			bancoPalavras.Instance.ListaIdPalavraAcerto.Clear();
			SceneManager.LoadScene ("telaFimJogo");
		}
	}

	//Função criada por Magno
	public void pressedAudioPalavra (){
		orientacaoInicial.SetActive (false);
		mensagemInicial_Aplicada = true;

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
