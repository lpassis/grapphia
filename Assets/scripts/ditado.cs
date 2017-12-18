using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.SceneManagement;

// Classe que controla todos os elementos gráficos do jogo!
public class ditado : MonoBehaviour {

	//public GameObject mensagem_fim_jogo;

	//public GameObject mensagem_audio_frase;

	//public InputField palavra; // Campo onde o usuário insere a palavra

	//public Text Score; // Referenciando acertos que mostra na tela! Verificar se necessário

	public GameObject animacaoProfessora;//para animação de professora

	//public Text txtBoardWord;

	public AudioClip clip;

	private int idPalavra; // Utilizado para identificar qual é a palavra corrente. Busca feita no vetor de palavras apresentadas ao usuário

	private int indicePalavra; //// Utilizado para identificar qual o índice no vetor da palavra corrente. Busca feita no vetor de palavras geral

	private bool respondido; // Verificar se usuário já respondeu!

	private int countWords; // Faz o controle da contagem para não ficar repetindo a palavra!

	int ScoreInitial; // Armazena o acerto inicial em cada nível!

	bool fimJogo;

	float aux;

	int cont;


	//---------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------

	// Inicializa a tela do jogo!
	void Start () {
	/*	bancoPalavras.Instance.carrega_palavras_nivel(dadosJogo.Instance.currentUser.Nivel);

		countWords = bancoPalavras.Instance.numWordsGame; //quantidade de palavras apresentadas ao usuário

		idPalavra = Random.Range(0, bancoPalavras.Instance.qtd_Words); // Random para colocar palavra inicial aleatória

		this.setPalavra(dadosJogo.Instance.currentUser.Nivel);*/

	}


	//---------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------

	// Vai ser chamada a cada frame por segundo!
	void FixedUpdate () {
//			this.setPalavra(dadosJogo.Instance.currentUser.Nivel);
	}


	//------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------

	/*
	public void setPalavra(int nivel)
	{
		int countLoop = 0;

		setPalavra:

		++countLoop;

		int aux = bancoPalavras.Instance.palavras[nivel][idPalavra].Id - 1;

		if (bancoPalavras.Instance.palavrasAcerto [aux].acerto != true ||
		    countLoop > bancoPalavras.Instance.total_palavras) {

			txtBoardWord.text = bancoPalavras.Instance.palavras [nivel] [idPalavra].palavra;
		}
	}*/

	void pressedAudioFrase(){

		string arquivo = "audiosditado/avisaditado";
		animacaoProfessora.SetActive (true);
		clip = (AudioClip)Resources.Load (arquivo);
		AudioSource audio;
		audio = gameObject.AddComponent<AudioSource> ();
		audio.volume = 1;
		audio.clip = clip;
		audio.Play ();
		StartCoroutine ("audioEnd");
	}

	//para quandot terminar áudio, chama recursivo em pressedAudioFrase()
	IEnumerator audioEnd(){
		yield return new WaitForSeconds (clip.length);
		animacaoProfessora.SetActive (false);
	}

}
