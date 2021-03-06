﻿using UnityEngine;
using UnityEngine.UI;
using System.Net;
using System.Net.Mail;
using System.Collections;
using UnityEngine.SceneManagement;
using System.IO;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using Firebase;
using Firebase.Database;
using Firebase.Unity.Editor;
using Firebase.Auth;

// Classe que controla todos os elementos gráficos do jogo!
public class gameController : MonoBehaviour
{

	public GameObject cloud1;
	// Referenciando as nuvens!
	public GameObject cloud2;
	public GameObject cloud3;

	public GameObject bird1;
	// Referenciando os pássaros
	public GameObject bird2;

	public GameObject horse;
	// Referenciando os cavalos!
	public GameObject horse2;

	public GameObject board_letter;
	// Referenciando as caixas!
	public GameObject board2_letter;

	public GameObject message_hit;
	// Referenciando as mensagens de acerto e erro!
	public GameObject message_error;

	public GameObject cowgirl_moving_rope;
	// Referenciando o cowboy e a cowgirl!
	public GameObject cowboy_moving_rope;

	public GameObject sound_won;
	// Rerefenciando sons de ganhou e perdeu!
	public GameObject sound_lost;

	public GameObject sound_on;
	// Referenciando botão som ativado e desativado!
	public GameObject sound_off;

	public GameObject menu_pause;
	// Referenciando botão pause!

	public GameObject mensagem_inicial;

	public GameObject mensagem_fim_jogo;

	public GameObject mensagem_audio_frase;

	public GameObject cowboy_lacando1;

	public GameObject cowboy_lacando2;

	public GameObject cowgirl_lacando1;

	public GameObject cowgirl_lacando2;

	public Text txtBoardWord;
	// Rerenciando caixa da palavra, onde vai conter a palavra!

	public Text txtBoardLetter;
	// Referenciando caixas das letras, onde vai conter as letras a serem escolhidas!

	public Text txtBoardLetter2;

	public Text Score;
	// Referenciando acertos que mostra na tela!

	public GameObject casa0;
	public GameObject casa1;
	public GameObject casa2;
	public GameObject casa3;
	public GameObject casa4;
	public GameObject casa5;
	public GameObject casa6;
	public GameObject casa7;
	public GameObject casa8;
	public GameObject casa9;


	//Posições dos objetos!
	private float x1;
	private float y1;

	private float x2;
	private float y2;

	private float x3;
	private float y3;

	private float xBird1;
	private float yBird1;

	private float xBird2;
	private float yBird2;

	private float xHorse;
	private float yHorse;

	private float xHorse2;
	private float yHorse2;

	private float xHorseinitial;
	private float yHorseinitial;

	private float xHorse2initial;
	private float yHorse2initial;

	private float xboard_letter;
	private float yboard_letter;

	private float xboard2_letter;
	private float yboard2_letter;

	private float xboard_letter_initial;
	private float yboard_letter_initial;

	private float xboard2_letter_initial;
	private float yboard2_letter_initial;

	int[] randId;
	//private int idPalavraAnt;

	private int idPalavra;
	// Utilizado para identificar qual é a palavra corrente!

	private bool respondido;
	// Verificar se usuário já respondeu!

	private int countWords;
	// Faz o controle da contagem para não ficar repetindo a palavra!

	float randNum_blocos;
	// Randa para colocar os blocos com a letra em ordem aleatória!

	int ScoreInitial;
	// Armazena o acerto inicial em cada nível!

	bool fimJogo;
	float aux;

	DatabaseReference reference;
	//Firebase.Auth.FirebaseAuth auth;
	//Firebase.Auth.FirebaseUser user_fb;
	//string uid;

	string pathToFireBaseUser;

	//---------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------

	// Inicializa a tela do jogo!
	void Start ()
	{

		ScoreInitial = dadosJogo.Instance.currentUser.Score; // Recebendo do banco de dados o score do usuário corrente!

		if (ScoreInitial == 0) {

			mensagem_inicial.SetActive (true);
			mensagem_audio_frase.SetActive (true);

		} else
			verifica_porcentagem_casa ();

		horse.GetComponent<AudioSource> ().Pause ();
		horse2.GetComponent<AudioSource> ().Pause ();
		Score.text = "ACERTOS: " + dadosJogo.Instance.currentUser.Score;

		// IF para saber qual personagem vai ativar!
		if (dadosJogo.Instance.currentPesonagem == 1) {
			cowboy_moving_rope.SetActive (false);
		} else
			cowgirl_moving_rope.SetActive (false);

		// inicializando as posições dos objetos!
		x1 = cloud1.transform.position.x;
		y1 = cloud1.transform.position.y;

		x2 = cloud2.transform.position.x;
		y2 = cloud2.transform.position.y;

		x3 = cloud3.transform.position.x;
		y3 = cloud3.transform.position.y;

		xBird1 = bird1.transform.position.x;
		yBird1 = bird1.transform.position.y;

		xBird2 = bird2.transform.position.x;
		yBird2 = bird2.transform.position.y;

		xHorseinitial = xHorse = horse.transform.position.x;
		yHorseinitial = yHorse = horse.transform.position.y;

		xHorse2initial = xHorse2 = horse2.transform.position.x;
		yHorse2initial = yHorse2 = horse2.transform.position.y;

		xboard_letter_initial = xboard_letter = board_letter.transform.position.x;
		yboard_letter_initial = yboard_letter = board_letter.transform.position.y;

		xboard2_letter_initial = xboard2_letter = board2_letter.transform.position.x;
		yboard2_letter_initial = yboard2_letter = board2_letter.transform.position.y;


		bancoPalavras.Instance.carrega_palavras_nivel (dadosJogo.Instance.currentUser.Nivel);

		countWords = bancoPalavras.Instance.numWordsGame; //quantidade de palavras apresentadas ao usuário
		//idPalavraAnt = idPalavra;
		idPalavra = Random.Range (0, bancoPalavras.Instance.qtd_Words); // Random para colocar palavra inicial aleatória

		randNum_blocos = Random.Range (1.0f, 2.0f); // Ordem aleatória de entrada dos blocos com as letras 

		randId = new int[bancoPalavras.Instance.total_palavras_Nivel [dadosJogo.Instance.currentUser.Nivel]];
		for (int i = 0; i < bancoPalavras.Instance.total_palavras_Nivel [dadosJogo.Instance.currentUser.Nivel]; ++i) {
			randId[i] = i;
		}

		randId = ShuffleArray(randId);

		for (int i = 0; i < bancoPalavras.Instance.total_palavras_Nivel [dadosJogo.Instance.currentUser.Nivel]; ++i)
			Debug.Log ("i=" + randId[i]);


		this.setPalavra (dadosJogo.Instance.currentUser.Nivel);

		InitializeFireBase ();
	}

	//---------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------

	// Vai ser chamada a cada frame por segundo!
	void FixedUpdate ()
	{

		// limite do movimento dos objetos!
		//if (xboard_letter >= -2f) xboard_letter -= 0.065f;
		if (xboard_letter >= -2f)
			xboard_letter -= 0.125f; // velocidade da caixa 1
		else if (yboard_letter <= -1.7f) {
			yboard_letter += 0.090f;
			yHorse -= 0.06f;

		}

		//if (xboard2_letter >= 0f) xboard2_letter -= 0.065f;
		if (xboard2_letter >= 0f)
			xboard2_letter -= 0.125f; // velocidade da caixa 2
		else if (yboard2_letter <= -1.7f) {
			yboard2_letter += 0.090f;
			yHorse2 -= 0.06f;

		}
		if (x1 >= 9.52f)
			x1 = -9.52f;
		if (x2 >= 9.52f)
			x2 = -9.52f;
		if (x3 >= 9.52f)
			x3 = -9.52f;

		if (xBird1 <= -9.52f)
			xBird1 = 16.52f;
		if (xBird2 <= -9.52f)
			xBird2 = 23.52f;
		if (xHorse <= -9.52f) { 
			horse.GetComponent<AudioSource> ().Pause ();
			xHorse = 30f;

		}

		if (xHorse < 9.52f && respondido == false)
			horse.GetComponent<AudioSource> ().UnPause ();

		if (xHorse2 <= -9.52f) {
			horse2.GetComponent<AudioSource> ().Pause ();
			xHorse2 = 30f;
		}

		if (xHorse2 < 9.52f && respondido == false)
			horse2.GetComponent<AudioSource> ().UnPause ();

		// Velocidade dos movimentos dos objetos!
		x1 = x1 + 0.002f;
		x2 = x2 + 0.002f;
		x3 = x3 + 0.002f;
		xBird1 -= 0.009f;
		xBird2 -= 0.009f;
		//xHorse -= 0.065f;
		//xHorse2 -= 0.065f;
		xHorse -= 0.125f;
		xHorse2 -= 0.125f;


		// IF para verificar se usuário escolheu a opção!
		if (respondido) {

			xHorse = xHorseinitial;
			yHorse = yHorseinitial;
			xHorse2 = xHorse2initial;
			yHorse2 = yHorse2initial;


			if (aux <= 9.52f && xboard_letter < 9f) {
				aux += 0.09f;
				//yboard_letter += 0.09f;
				//yboard2_letter += 0.09f;
				//board_letter.transform.position = new Vector2(xboard_letter, yboard_letter);
				//board2_letter.transform.position = new Vector2(xboard2_letter, yboard2_letter);

			} else {

				xboard_letter = xboard_letter_initial;
				yboard_letter = yboard_letter_initial;
				aux = 0;
				xboard2_letter = xboard2_letter_initial;
				yboard2_letter = yboard2_letter_initial;
				respondido = false;
				message_hit.SetActive (false);
				message_error.SetActive (false);
				//board_letter.SetActive(true);
				//board2_letter.SetActive(true);
				board_letter.GetComponent<Button> ().enabled = true;
				board2_letter.GetComponent<Button> ().enabled = true;

				if (dadosJogo.Instance.currentPesonagem == 2) {
					cowboy_lacando1.SetActive (false);
					cowboy_lacando2.SetActive (false);
					cowboy_moving_rope.SetActive (true);

				} else {
					cowgirl_lacando1.SetActive (false);
					cowgirl_lacando2.SetActive (false);
					cowgirl_moving_rope.SetActive (true);

				}

				randNum_blocos = Random.Range (1.0f, 2.0f);

				if (countWords <= 0) {
					SceneManager.LoadScene ("telaPreDitado");
					Debug.Log ("3) countWords: " + countWords);
					horse.SetActive (false);
					horse2.SetActive (false);
					board_letter.SetActive (false);
					board2_letter.SetActive (false);
					cowboy_moving_rope.SetActive (false);
					cowgirl_moving_rope.SetActive (false);
					fimJogo = true;
					dadosJogo.Instance.salvar_dados ();
					return;

				}
				this.setPalavra (dadosJogo.Instance.currentUser.Nivel);
			}

		}

		//Setando nos objetos!
		cloud1.transform.position = new Vector2 (x1, y1);
		cloud2.transform.position = new Vector2 (x2, y2);
		cloud3.transform.position = new Vector2 (x3, y3);
		bird1.transform.position = new Vector2 (xBird1, yBird1);
		bird2.transform.position = new Vector2 (xBird2, yBird2);
		horse.transform.position = new Vector2 (xHorse, yHorse);
		horse2.transform.position = new Vector2 (xHorse2, yHorse2);
		board_letter.transform.position = new Vector2 (xboard_letter, yboard_letter);
		board2_letter.transform.position = new Vector2 (xboard2_letter, yboard2_letter);

	}

	//------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------

	// Função para mudar palavra na caixa!
	public void setPalavra (int nivel)
	{
		idPalavra = randId[countWords];

		txtBoardWord.text = bancoPalavras.Instance.palavras[idPalavra].palavra;

		if (randNum_blocos > 1.5f) {
			txtBoardLetter.text = bancoPalavras.Instance.palavras[idPalavra].letra_correta;
			txtBoardLetter2.text = bancoPalavras.Instance.palavras[idPalavra].opcao1;
		} else {
			txtBoardLetter2.text = bancoPalavras.Instance.palavras[idPalavra].letra_correta;
			txtBoardLetter.text = bancoPalavras.Instance.palavras[idPalavra].opcao1;
		}

		countWords--;
		Debug.Log ("1) countWords: " + countWords);
	}

	public void calcula_porcentagem_casa ()
	{
		
		double p = (double)dadosJogo.Instance.currentUser.Score;

		if (p == 2.0) {
			casa8.SetActive (true);
			casa8.GetComponent<AudioSource> ().Play ();
		} else if (p == 4.0) {
			casa9.SetActive (true);
			casa8.GetComponent<AudioSource> ().Play ();
		} else if (p == 6.0) {
			casa7.SetActive (true);
			casa8.GetComponent<AudioSource> ().Play ();
		} else if (p == 8.0) {
			casa6.SetActive (true);
			casa8.GetComponent<AudioSource> ().Play ();
		} else if (p == 10.0) {
			casa5.SetActive (true);
			casa8.GetComponent<AudioSource> ().Play ();
		} else if (p == 11.0) {
			casa4.SetActive (true);
			casa8.GetComponent<AudioSource> ().Play ();
		} else if (p == 12.0) {
			casa3.SetActive (true);
			casa8.GetComponent<AudioSource> ().Play ();
		} else if (p == 13.0) {
			casa2.SetActive (true);
			casa8.GetComponent<AudioSource> ().Play ();
		} else if (p == 14.0) {
			casa0.SetActive (true);
			casa8.GetComponent<AudioSource> ().Play ();
		} else if (p == 15.0) {
			casa1.SetActive (true);
			casa8.GetComponent<AudioSource> ().Play ();
		}
	}


	public void verifica_porcentagem_casa ()
	{
		double p = (double)dadosJogo.Instance.currentUser.Score;

		if (p >= 1.0 && p < 2.0) {
			casa8.SetActive (true);
		} else if (p >= 2.0 && p < 3.0) {

			casa8.SetActive (true);
			casa9.SetActive (true);
		} else if (p >= 3.0 && p < 4.0) {

			casa8.SetActive (true);
			casa9.SetActive (true);
			casa7.SetActive (true);
		} else if (p >= 4.0 && p < 5.0) {
			casa8.SetActive (true);
			casa9.SetActive (true);
			casa7.SetActive (true);
			casa6.SetActive (true);
		} else if (p >= 5.0 && p < 6.0) {
			casa8.SetActive (true);
			casa9.SetActive (true);
			casa7.SetActive (true);
			casa6.SetActive (true);
			casa5.SetActive (true);
		} else if (p >= 6.0 && p < 7.0) {
			casa8.SetActive (true);
			casa9.SetActive (true);
			casa7.SetActive (true);
			casa6.SetActive (true);
			casa5.SetActive (true);
			casa4.SetActive (true);
		} else if (p >= 7.0 && p < 8.0) {
			casa8.SetActive (true);
			casa9.SetActive (true);
			casa7.SetActive (true);
			casa6.SetActive (true);
			casa5.SetActive (true);
			casa4.SetActive (true);
			casa3.SetActive (true);
		} else if (p >= 8.0 && p < 9.0) {
			casa8.SetActive (true);
			casa9.SetActive (true);
			casa7.SetActive (true);
			casa6.SetActive (true);
			casa5.SetActive (true);
			casa4.SetActive (true);
			casa3.SetActive (true);
			casa2.SetActive (true);
		} else if (p >= 9.0 && p < 10.0) {
			casa8.SetActive (true);
			casa9.SetActive (true);
			casa7.SetActive (true);
			casa6.SetActive (true);
			casa5.SetActive (true);
			casa4.SetActive (true);
			casa3.SetActive (true);
			casa2.SetActive (true);
			casa0.SetActive (true);
		} else if (p == 10.0) {
			casa8.SetActive (true);
			casa9.SetActive (true);
			casa7.SetActive (true);
			casa6.SetActive (true);
			casa5.SetActive (true);
			casa4.SetActive (true);
			casa3.SetActive (true);
			casa2.SetActive (true);
			casa0.SetActive (true);
			casa1.SetActive (true);
		}
	}


	void InitializeFireBase(){
		FirebaseApp.DefaultInstance.SetEditorDatabaseUrl ("https://grapphia.firebaseio.com/");
		reference= FirebaseDatabase.DefaultInstance.RootReference;
		//auth = Firebase.Auth.FirebaseAuth.DefaultInstance;
		//user_fb = auth.CurrentUser;
		//uid = user_fb.UserId;
	}
	//--------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------


	// ----------------------------------------------------------------------------------------------- funções que implementam eventos de botões da interface
	// Quando pressiona a caixa 1!
	public void pressedButtonLetter1 ()
	{
		pathToFireBaseUser = dadosJogo.Instance.currentUser.key + "/"+ dadosJogo.Instance.currentUser.Name;
		//pathToFireBaseUser = dadosJogo.Instance.currentUser.key + "/" + uid +"/"+ dadosJogo.Instance.currentUser.Name;
		//string uid = user_fb.UserId;

		horse.GetComponent<AudioSource> ().Pause ();
		horse.GetComponent<AudioSource> ().time = 0.0f;
		horse2.GetComponent<AudioSource> ().Pause ();
		horse2.GetComponent<AudioSource> ().time = 0.0f;
		mensagem_inicial.SetActive (false);
		mensagem_audio_frase.SetActive (false);
		board2_letter.GetComponent<Button> ().enabled = false;

		int auxIdpalavra = bancoPalavras.Instance.palavras[idPalavra].Id - 1;

		if (yboard_letter >= -1.7f &&
			txtBoardLetter.text == bancoPalavras.Instance.palavras[idPalavra].letra_correta) {

			respondido = true;

			if (dadosJogo.Instance.currentPesonagem == 2) {
				cowboy_moving_rope.SetActive (false);
				cowboy_lacando1.SetActive (true);
			} else {
				cowgirl_moving_rope.SetActive (false);
				cowgirl_lacando1.SetActive (true);
			}
			Debug.Log ("acertou");

			++dadosJogo.Instance.currentUser.Score;
			//++bancoPalavras.Instance.acertos;
			calcula_porcentagem_casa ();
			reference.Child ("users/" + pathToFireBaseUser + "/Score").SetValueAsync (dadosJogo.Instance.currentUser.Score);

			if (bancoPalavras.Instance.palavrasAcerto [auxIdpalavra].Id == -1) {
				bancoPalavras.Instance.palavrasAcerto [auxIdpalavra] = new palavraAcertoUser {

					idPalavra = bancoPalavras.Instance.palavras[idPalavra].Id,
					idUser = dadosJogo.Instance.currentUser.Id,
					acerto = true,
					nivelPalavra = (dadosJogo.Instance.currentUser.Nivel + 1)
				};
				bancoPalavras.Instance.qtd_WordsPresented++;
				bancoPalavras.Instance.ListaIdPalavraAcerto.Add (idPalavra);

			} else if (bancoPalavras.Instance.palavrasAcerto [auxIdpalavra].acerto == false) {
				bancoPalavras.Instance.palavrasAcerto[auxIdpalavra].acerto= true;
				//nao está incremetando o currentUser.Score
			} 
		} else if (yboard_letter >= -1.7f) {

			respondido = true;
			message_error.SetActive (true);

			if (dadosJogo.Instance.currentPesonagem == 2) {
				cowboy_moving_rope.SetActive (false);
				cowboy_lacando1.SetActive (true);
			} else {
				cowgirl_moving_rope.SetActive (false);
				cowgirl_lacando1.SetActive (true);
			}
			Debug.Log ("ERROU.... hahaha");
			Debug.Log ("auxidpalavra" + auxIdpalavra);

			Debug.Log ("LU:" +bancoPalavras.Instance.palavrasAcerto [auxIdpalavra].Id);

			if (bancoPalavras.Instance.palavrasAcerto [auxIdpalavra].Id == -1) {
				bancoPalavras.Instance.palavrasAcerto [auxIdpalavra] = new palavraAcertoUser {

					idPalavra = bancoPalavras.Instance.palavras[idPalavra].Id,
					idUser = dadosJogo.Instance.currentUser.Id,
					acerto = false,
					nivelPalavra = (dadosJogo.Instance.currentUser.Nivel + 1)
				};
				bancoPalavras.Instance.qtd_WordsPresented++;
				bancoPalavras.Instance.ListaIdPalavraAcerto.Add (idPalavra);
				reference.Child("users/" + pathToFireBaseUser + "/Erros").SetValueAsync(bancoPalavras.Instance.qtd_WordsPresented - dadosJogo.Instance.currentUser.Score);
			} else if (bancoPalavras.Instance.palavrasAcerto [auxIdpalavra].acerto == true) {
				bancoPalavras.Instance.palavrasAcerto [auxIdpalavra].acerto = false;
			}

			sound_lost.GetComponent<AudioSource> ().Play ();

			txtBoardWord.text = bancoPalavras.Instance.palavras[idPalavra].palavra_completa;
		}
	}

	// Quando pressiona a caixa 2!
	public void pressedButtonLetter2 ()
	{
		pathToFireBaseUser = dadosJogo.Instance.currentUser.key + "/"+ dadosJogo.Instance.currentUser.Name;
		//pathToFireBaseUser = dadosJogo.Instance.currentUser.key + "/" + uid +"/"+ dadosJogo.Instance.currentUser.Name;
		//string uid = user_fb.UserId;

		horse.GetComponent<AudioSource> ().Pause ();
		horse.GetComponent<AudioSource> ().time = 0.0f;
		horse2.GetComponent<AudioSource> ().Pause ();
		horse2.GetComponent<AudioSource> ().time = 0.0f;
		mensagem_inicial.SetActive (false);
		mensagem_audio_frase.SetActive (false);

		board_letter.GetComponent<Button> ().enabled = false;

		int auxIdpalavra = bancoPalavras.Instance.palavras[idPalavra].Id - 1;

		if (yboard2_letter >= -1.7f
			&& txtBoardLetter2.text == bancoPalavras.Instance.palavras[idPalavra].letra_correta) {

			respondido = true;

			if (dadosJogo.Instance.currentPesonagem == 2) {
				cowboy_moving_rope.SetActive (false);
				cowboy_lacando2.SetActive (true);
			} else {
				cowgirl_moving_rope.SetActive (false);
				cowgirl_lacando2.SetActive (true);
			}

			++dadosJogo.Instance.currentUser.Score;
			//++bancoPalavras.Instance.acertos;
			calcula_porcentagem_casa ();
			reference.Child ("users/" + pathToFireBaseUser + "/Score").SetValueAsync (dadosJogo.Instance.currentUser.Score);

			if (bancoPalavras.Instance.palavrasAcerto [auxIdpalavra].Id == -1) {
				bancoPalavras.Instance.palavrasAcerto [auxIdpalavra] = new palavraAcertoUser {

					idPalavra = bancoPalavras.Instance.palavras [idPalavra].Id,
					idUser = dadosJogo.Instance.currentUser.Id,
					acerto = true,
					nivelPalavra = (dadosJogo.Instance.currentUser.Nivel + 1)
				};
				bancoPalavras.Instance.qtd_WordsPresented++;
				bancoPalavras.Instance.ListaIdPalavraAcerto.Add (idPalavra);

			} else if (bancoPalavras.Instance.palavrasAcerto [auxIdpalavra].acerto == false) {
				bancoPalavras.Instance.palavrasAcerto[auxIdpalavra].acerto= true;
				//nao está incremetando o currentUser.Score
			}


			message_hit.SetActive (true);
			sound_won.GetComponent<AudioSource> ().Play ();
			txtBoardWord.text = bancoPalavras.Instance.palavras[idPalavra].palavra_completa;

			Score.text = "ACERTOS: " + dadosJogo.Instance.currentUser.Score;
		} else if (yboard2_letter >= -1.7f) {
			horse.GetComponent<AudioSource> ().Pause ();
			respondido = true;
			message_error.SetActive (true);

			if (dadosJogo.Instance.currentPesonagem == 2) {
				cowboy_moving_rope.SetActive (false);
				cowboy_lacando2.SetActive (true);
			} else {
				cowgirl_moving_rope.SetActive (false);
				cowgirl_lacando2.SetActive (true);
			}

			Debug.Log ("ERROU....");


			if (bancoPalavras.Instance.palavrasAcerto [auxIdpalavra].Id == -1) {
				bancoPalavras.Instance.palavrasAcerto [auxIdpalavra] = new palavraAcertoUser {

					idPalavra = bancoPalavras.Instance.palavras[idPalavra].Id,
					idUser = dadosJogo.Instance.currentUser.Id,
					acerto = false,
					nivelPalavra = (dadosJogo.Instance.currentUser.Nivel + 1)
				};
				bancoPalavras.Instance.qtd_WordsPresented++;
				bancoPalavras.Instance.ListaIdPalavraAcerto.Add (idPalavra);
				reference.Child("users/" + pathToFireBaseUser + "/Erros").SetValueAsync(bancoPalavras.Instance.qtd_WordsPresented - dadosJogo.Instance.currentUser.Score);

			} else if (bancoPalavras.Instance.palavrasAcerto [auxIdpalavra].acerto == true) {
				bancoPalavras.Instance.palavrasAcerto [auxIdpalavra].acerto = false;
			}

			sound_lost.GetComponent<AudioSource> ().Play ();
			txtBoardWord.text = bancoPalavras.Instance.palavras[idPalavra].palavra_completa;
		}

	}


	public void pressedAudioFrase ()
	{
		if (dadosJogo.Instance.currentPesonagem == 2) {	

			string arquivo = "audios/" + bancoPalavras.Instance.palavras [idPalavra].nome_audio_menino;

			AudioClip clip = (AudioClip)Resources.Load (arquivo);
			AudioSource audio;
			audio = gameObject.AddComponent<AudioSource> ();
			audio.volume = 1;
			audio.clip = clip;
			audio.Play ();
		}
		else{
			string arquivo2 = "audios/" + bancoPalavras.Instance.palavras[idPalavra].nome_audio_menina;

			AudioClip clip2 = (AudioClip)Resources.Load (arquivo2);
			AudioSource audio2;
			audio2 = gameObject.AddComponent<AudioSource> ();
			audio2.volume = 1;
			audio2.clip = clip2;
			audio2.Play ();
			Debug.Log (arquivo2);
		}

	}


	// Função de quando pressiona o botão pause!
	public void pressedPause ()
	{

		if (fimJogo)
			menu_pause.SetActive (true);
		else {
			if (Time.timeScale == 0f) {
				Time.timeScale = 1f;
				bird1.GetComponent<AudioSource> ().Play ();
				if (dadosJogo.Instance.currentPesonagem == 1)
					cowgirl_moving_rope.SetActive (true);
				else
					cowboy_moving_rope.SetActive (true);

				menu_pause.SetActive (false);
				board_letter.SetActive (true);
				board2_letter.SetActive (true);
			} else {
				Time.timeScale = 0f;
				bird1.GetComponent<AudioSource> ().Pause ();
				horse.GetComponent<AudioSource> ().Pause ();
				horse2.GetComponent<AudioSource> ().Pause ();
				cowboy_moving_rope.SetActive (false);
				menu_pause.SetActive (true);
				board_letter.SetActive (false);
				board2_letter.SetActive (false);

			}
		}

	}

	// Retornar o jogo quando o jogo está pausado!
	public void btnClick_Return ()
	{

		if (fimJogo)
			menu_pause.SetActive (false);
		else {
			Time.timeScale = 1f;
			bird1.GetComponent<AudioSource> ().Play ();

			if (dadosJogo.Instance.currentPesonagem == 1)
				cowgirl_moving_rope.SetActive (true);
			else
				cowboy_moving_rope.SetActive (true);

			menu_pause.SetActive (false);
			board_letter.SetActive (true);
			board2_letter.SetActive (true);
		}

	}

	// Ir para o menu inicial!
	public void btnClick_MenuInicial ()
	{
		Time.timeScale = 1f;

		dadosJogo.Instance.salvar_dados ();
		SceneManager.LoadScene ("TelaInicialOpcoes");
	}


	public void btnSoundClick ()
	{
		// Função que bloqueia e ativa o som!
		var audio = bird1.GetComponent<AudioSource> ();
		if (audio.isPlaying) {
			sound_on.SetActive (false);
			audio.Pause ();
			horse.GetComponent<AudioSource> ().Stop ();
			horse2.GetComponent<AudioSource> ().Stop ();
			sound_off.SetActive (true);
			sound_won.SetActive (false);
			sound_lost.SetActive (false);


		} else {
			sound_on.SetActive (true);
			audio.UnPause ();
			horse.GetComponent<AudioSource> ().Play ();
			horse2.GetComponent<AudioSource> ().Play ();

			if (xHorse > 9.52f)
				horse.GetComponent<AudioSource> ().Pause ();
			if (xHorse2 > 9.52f)
				horse2.GetComponent<AudioSource> ().Pause ();
			sound_off.SetActive (false);
			sound_won.SetActive (true);
			sound_lost.SetActive (true);

		}
	}

	//Função utilizada para embaralhar as palavras que irão aparecer para o usuário
	private int[] ShuffleArray(int[] array)
	{
		System.Random r = new System.Random();

		for (int i = array.Length; i > 0; i--)
		{
			int j = r.Next(i);
			int k = array[j];
			array[j] = array[i - 1];
			array[i - 1]  = k;
		}
		return array;
	}
}
