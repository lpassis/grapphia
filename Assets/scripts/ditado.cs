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
	/*void Start () {

		ScoreInitial = dadosJogo.Instance.currentUser.Score; // Recebendo do banco de dados o score do usuário corrente!

	//	Score.text = "ACERTOS: " + dadosJogo.Instance.currentUser.Score;

		//bancoPalavras.Instance.carrega_palavras_nivel(dadosJogo.Instance.currentUser.Nivel);

		countWords = bancoPalavras.Instance.numWordsDitado; //quantidade de palavras apresentadas ao usuário no ditado


		idPalavra = Random.Range(0, bancoPalavras.Instance.qtd_WordsPresented); // Random para colocar palavra inicial aleatória
		Debug.Log("idPalavra" + idPalavra);
//		indicePalavra = getPalavra(dadosJogo.Instance.currentUser.Nivel, idPalavra); //busca a palavra no banco de palavras
		Debug.Log("indicePalavra" + indicePalavra);
		Debug.Log("idPalavra" + idPalavra + " e palavra " + bancoPalavras.Instance.palavras[dadosJogo.Instance.currentUser.Nivel][indicePalavra].palavra_completa);

		this.setPalavra(dadosJogo.Instance.currentUser.Nivel);
	}


	//Função que retorna o índice da palavra a ser apresetada no ditado no vetor de palavras do BD
	//Criado por Luciana
	private int getPalavra(int level, int idPalavra){
		Debug.Log ("Imprimindo idPalavras");
		for (int p = 0; p < bancoPalavras.Instance.palavras.Length; p++) { 
			Debug.Log (bancoPalavras.Instance.palavras [level] [p].Id + " ");	
			if (bancoPalavras.Instance.palavras [level] [p].Id == idPalavra)
				return p;
		}
		return 0;
	}


	//---------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------

	// Vai ser chamada a cada frame por segundo!
	void FixedUpdate () {

		// IF para verificar se usuário escolheu a opção!
		if (respondido)
		{
			idPalavra = Random.Range(0, bancoPalavras.Instance.qtd_WordsPresented);

			if (countWords <= 0)
			{
				//mensagem_fim_jogo.SetActive(true);
				SceneManager.LoadScene("fimJogo");
				Debug.Log ("3) countWords: " + countWords);
				fimJogo = true;
				inteligencia.Instance.seleciona_nivel();  // SELECIONADO O NÍVEL
				dadosJogo.Instance.salvar_dados();
				return;
			}

			this.setPalavra(dadosJogo.Instance.currentUser.Nivel);
		}

	}


	//------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------

	// Função para mudar palavra na caixa!
	public void setPalavra(int nivel)
	{
		int countLoop = 0;

		setPalavra:

		++countLoop;

		int aux = bancoPalavras.Instance.palavras[nivel][idPalavra].Id - 1;

		if (bancoPalavras.Instance.palavrasAcerto[aux].acerto != true || 
			countLoop > bancoPalavras.Instance.total_palavras)
		{

/*			txtBoardWord.text = bancoPalavras.Instance.palavras[nivel][idPalavra].palavra;

			if (randNum_blocos > 1.5f)
			{
				txtBoardLetter.text = bancoPalavras.Instance.palavras[nivel][idPalavra].letra_correta;
				txtBoardLetter2.text = bancoPalavras.Instance.palavras[nivel][idPalavra].opcao1;
			}
			else
			{
				txtBoardLetter2.text = bancoPalavras.Instance.palavras[nivel][idPalavra].letra_correta;
				txtBoardLetter.text = bancoPalavras.Instance.palavras[nivel][idPalavra].opcao1;

			}
		}
		else {

			idPalavra = Random.Range(0, bancoPalavras.Instance.qtd_Words);
			goto setPalavra;
		}
		--countWords;
		Debug.Log ("1) countWords: " + countWords);
	}*/



	// ----------------------------------------------------------------------------------------------- funções que implementam eventos de botões da interface 
	// Quando pressiona a caixa 1!
	/*public void pressedButtonLetter1()
	{
		//mensagem_audio_frase.SetActive(false);

		int auxIdpalavra = bancoPalavras.Instance.palavras[dadosJogo.Instance.currentUser.Nivel][idPalavra].Id - 1;

		/*if (yboard_letter >= -1.7f && 
			txtBoardLetter.text == bancoPalavras.Instance.palavras[dadosJogo.Instance.currentUser.Nivel][idPalavra].letra_correta)
		{

			respondido = true;

			if (dadosJogo.Instance.currentPesonagem == 2)
			{
				cowboy_moving_rope.SetActive(false);
				cowboy_lacando1.SetActive(true);
			}
			else
			{
				cowgirl_moving_rope.SetActive(false);
				cowgirl_lacando1.SetActive(true);
			}

			if (bancoPalavras.Instance.palavrasAcerto[auxIdpalavra].acerto != true)   //verificando se palavra já foi respondida corretamente!
			{
				bancoPalavras.Instance.palavrasAcerto[auxIdpalavra]= new palavraAcertoUser
				{

					idPalavra = bancoPalavras.Instance.palavras[dadosJogo.Instance.currentUser.Nivel][idPalavra].Id,
					idUser = dadosJogo.Instance.currentUser.Id,
					acerto = true,
					nivelPalavra = (dadosJogo.Instance.currentUser.Nivel+1)
				};

				++dadosJogo.Instance.currentUser.Score;
				++bancoPalavras.Instance.acertos;
				calcula_porcentagem_casa();
				Debug.Log("Acertos: " + bancoPalavras.Instance.acertos);
			}

			message_hit.SetActive(true);
			sound_won.GetComponent<AudioSource>().Play();
			txtBoardWord.text = bancoPalavras.Instance.palavras[dadosJogo.Instance.currentUser.Nivel][idPalavra].palavra_completa;
			Score.text = "ACERTOS: " + dadosJogo.Instance.currentUser.Score; 

		}
		else if (yboard_letter >= -1.7f)
		{

			respondido = true;



			if (bancoPalavras.Instance.palavrasAcerto[auxIdpalavra].acerto != false)
			{
				bancoPalavras.Instance.palavrasAcerto[auxIdpalavra] = new palavraAcertoUser
				{

					idPalavra = bancoPalavras.Instance.palavras[dadosJogo.Instance.currentUser.Nivel][idPalavra].Id,
					idUser = dadosJogo.Instance.currentUser.Id,
					acerto = false,
					nivelPalavra = (dadosJogo.Instance.currentUser.Nivel+1)
				};

			}


			txtBoardWord.text = bancoPalavras.Instance.palavras[dadosJogo.Instance.currentUser.Nivel][idPalavra].palavra_completa;

			++dadosJogo.Instance.erros[dadosJogo.Instance.currentUser.Nivel];

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
