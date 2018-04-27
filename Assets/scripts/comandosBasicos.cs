using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using SQLite4Unity3d;
using UnityEngine.SceneManagement;
using Firebase;
using Firebase.Auth;
using Firebase.Unity.Editor;
using Firebase.Database;


// Classe Singleton onde armazena as palavras!
public class bancoPalavras 
{
    // Matriz das palavras, cada linha da matriz é um nível e cada coluna é uma palavra!

    private static bancoPalavras instance;

    public palavraOpcao[][] palavras;

	public palavraAcertoUser[] palavrasAcerto;

	public List<int> ListaIdPalavraAcerto = new List<int> (); 

	public int qtd_Words; //quantidade de palavras recuperadas no BD de um determinado nível (nível atual do jogador)

	public int qtd_WordsPresented; //quantidade de palavras recuperadas no BD que determinado usuário já jogou

	public int numWordsGame = 15; //número de palavras que serão apresentadas para o usuário em cada nível

	public int numWordsDitado = 7; //número de palavras que serão apresentadas para o usuário no ditado

    public DataService dataservice;

    public int acertos, erros;//armazena acertos e erros totais por usuário

    public int total_palavras; //total de palavras no BD em todos os níveis

	public int maxLevel = 1; //quantidade de níveis (mundos) implementados

	public SQLiteConnection _connection;

    // Construtor da classe!
    private bancoPalavras()
    {
		
    }

    // Atributo estático!
	public static bancoPalavras Instance
    {
		get
        {
            if (instance == null)
            {
				instance = new bancoPalavras();
            }
            return instance;
        }

    }

	public void setTotPalavras(int total_palavras){
		this.total_palavras = total_palavras;
	}

	/** Função que busca no banco de dados um conjunto de palavras do nível especificado
	 * level: indica o nível das palavras que será feita a busca no BD
	 * As palavras retornadas do BD serão armazenadas no vetor palavras[][]
	 */
	public void carrega_palavras_nivel(int level) {


        int aux = (level + 1);

		if (aux >= maxLevel)
			aux = 1;

		Debug.Log ("Selecionando palavras do nível (valor do BD) " + aux);

        //Buscando palavras do nível AUX
		var resul = _connection.Table<palavraOpcao>().Where(x => x.nivel == aux);

		//Busca palavras do nível AUX e usuário específico
        var resul2 = _connection.Table<palavraAcertoUser>().Where(x => ((x.nivelPalavra == aux) && (x.idUser == dadosJogo.Instance.currentUser.Id)));

        qtd_Words = 0;
		qtd_WordsPresented = 0;

		palavras = new palavraOpcao[maxLevel][];

		Debug.Log ("palavras.lenght " + palavras.Length);
		Debug.Log ("level " + level);
		Debug.Log ("maxlevel " + maxLevel);
		Debug.Log ("result.Count " + resul.Count());
		palavras[level] = new palavraOpcao[resul.Count()];
		Debug.Log ("palavras[level].lenght " + palavras[level].Length);

        foreach (var p in resul)
        {

            palavras[level][qtd_Words] = new palavraOpcao
            {
                Id = p.Id,
                palavra_completa = p.palavra_completa,
                palavra = p.palavra,
                letra_correta = p.letra_correta,
                opcao1 = p.opcao1,
                nivel = p.nivel,
                nome_audio_menino = p.nome_audio_menino,
                nome_audio_menina = p.nome_audio_menina,
				nome_audio_ditado = p.nome_audio_ditado
            };

            ++qtd_Words;

        }

		Debug.Log("Foram recuperadas " + resul.Count() + " palavras do BD referentes ao nível " + level);

        acertos = resul2.Count();

		palavrasAcerto = new palavraAcertoUser[(32*aux)];
		for (int i = 0; i < palavrasAcerto.Length; i++) {
			palavrasAcerto [i] = new palavraAcertoUser(){
				Id = -1,
				idPalavra = -1,
				idUser = -1,
				acerto = false,
				nivelPalavra = -1
			};
		}

        if (resul2.Count() > 0)
        {
            //palavrasAcerto = new palavraAcertoUser[(32*aux)];

            //for (int j = 0; j < (32 * aux); ++j) palavrasAcerto[j] = new palavraAcertoUser();
            
			foreach (var p in resul2)
            {
                palavrasAcerto[(p.idPalavra-1)] = new palavraAcertoUser
                {
                    Id = p.Id,
                    idPalavra = p.idPalavra,
                    idUser = p.idUser,
                    acerto = p.acerto,
                    nivelPalavra = p.nivelPalavra
	            };
				qtd_WordsPresented++;
				ListaIdPalavraAcerto.Add(p.idPalavra-1);
            }
        }
        /*else
        {
            palavrasAcerto = new palavraAcertoUser[(32*aux)];
            for (int j = 0; j < (32*aux); ++j)
            {
                palavrasAcerto[j] = new palavraAcertoUser();
            }
        }*/

    }

    public void salvar_palavrasAcertoUser()
    {
		FirebaseApp.DefaultInstance.SetEditorDatabaseUrl("https://grapphia.firebaseio.com/");
		DatabaseReference reference = FirebaseDatabase.DefaultInstance.RootReference;

		Firebase.Auth.FirebaseAuth auth = Firebase.Auth.FirebaseAuth.DefaultInstance;
		Firebase.Auth.FirebaseUser user_fb = auth.CurrentUser;

		var key = bancoPalavras.Instance._connection.Table<keyPhone> ().FirstOrDefault().keyFireBase;

		//var nomeCurrentUser = _connection.Table<user>().Where(x => x.Id == dadosJogo.Instance.currentUser.Id);

		string uid = user_fb.UserId;
		//string pathFireBase = "palavrasAcertoUser/" + key + "/" + uid + "/" + dadosJogo.Instance.currentUser.Name + "/";
		string auxPalavra = "";
		string pathFireBase = "palavrasAcertoUser/" + key + "/" + dadosJogo.Instance.currentUser.Name + "/";

        for(int i=0; i<palavrasAcerto.Length; ++i)
        {
			

			if (palavrasAcerto [i].idPalavra > 0 && palavrasAcerto [i].Id > 0) {
				_connection.Update (palavrasAcerto [i]);

				auxPalavra = bancoPalavras.Instance.palavras [dadosJogo.Instance.currentUser.Nivel] [palavrasAcerto [i].idPalavra - 1].palavra_completa;
				reference.Child (pathFireBase + auxPalavra + "/idPalavra").SetValueAsync (palavrasAcerto [i].idPalavra);
				reference.Child (pathFireBase + auxPalavra + "/acerto").SetValueAsync (palavrasAcerto [i].acerto);
				reference.Child (pathFireBase + auxPalavra + "/nivelpalavra").SetValueAsync (palavrasAcerto [i].nivelPalavra);
				reference.Child (pathFireBase + auxPalavra + "/Nome").SetValueAsync (dadosJogo.Instance.currentUser.Name);
				reference.Child (pathFireBase + auxPalavra + "/FireBase UID").SetValueAsync (uid);


			} else if (palavrasAcerto [i].idPalavra > 0) {
				_connection.Insert (palavrasAcerto [i]);

				auxPalavra = bancoPalavras.Instance.palavras [dadosJogo.Instance.currentUser.Nivel] [palavrasAcerto [i].idPalavra - 1].palavra_completa;
				reference.Child(pathFireBase + auxPalavra + "/idPalavra").SetValueAsync(palavrasAcerto[i].idPalavra);
				reference.Child(pathFireBase + auxPalavra + "/acerto").SetValueAsync(palavrasAcerto[i].acerto);
				reference.Child(pathFireBase + auxPalavra + "/nivelpalavra").SetValueAsync(palavrasAcerto[i].nivelPalavra);
				reference.Child(pathFireBase + auxPalavra + "/Nome").SetValueAsync(dadosJogo.Instance.currentUser.Name);
				reference.Child(pathFireBase + auxPalavra+ "/FireBase UID").SetValueAsync(uid);

			}
			auxPalavra = "";
        }

    }


    public void salvar_palavras_no_banco()      //salva palavras no banco de dados  essa função é chamada apenas quando for criar o banco de dados

    {
		int numWordsLevel1 = 32;
		DatabaseReference reference = FirebaseDatabase.DefaultInstance.RootReference;

		palavras = new palavraOpcao[maxLevel][];

        for (int i = 0; i < maxLevel; ++i)
        {
			palavras[i] = new palavraOpcao[numWordsLevel1];

			for (int j = 0; j < numWordsLevel1; ++j) palavras[i][j] = new palavraOpcao();
        }

        //NÍVEL 1 - S e Z!
        palavras[0][0].palavra = "PAI _ AGEM";
        palavras[0][0].letra_correta = "S";
        palavras[0][0].opcao1 = "Z";
        palavras[0][0].palavra_completa = "PAISAGEM"; //O palhaço é muito engraçado. 
        palavras[0][0].nivel = 1;
		palavras[0][0].nome_audio_menino = "paisagemmenino";          //atenção os arquivos de audio devem ficar na pasta Resources/audios !! 
        palavras[0][0].nome_audio_menina = "paisagemmenina";
		palavras[0][0].nome_audio_ditado = "paisagemditado";

        palavras[0][1].palavra = "FA _ ENDA";
        palavras[0][1].letra_correta = "Z";
        palavras[0][1].opcao1 = "S";
        palavras[0][1].palavra_completa = "FAZENDA"; //Eu adoro a aula de dança! 
        palavras[0][1].nivel = 1;
		palavras[0][1].nome_audio_menino = "fazendamenino";
		palavras[0][1].nome_audio_menina = "fazendamenina";
		palavras[0][1].nome_audio_ditado = "fazendaditado";

        palavras[0][2].palavra = "PARAÍ _ O";
        palavras[0][2].letra_correta = "S";
        palavras[0][2].opcao1 = "Z";
        palavras[0][2].palavra_completa = "PARAÍSO"; //Esse assunto é chato !!!
        palavras[0][2].nivel = 1;
		palavras[0][2].nome_audio_menino = "paraisomenino";
		palavras[0][2].nome_audio_menina = "paraisomenina";
		palavras[0][2].nome_audio_ditado = "paraisoditado";

        palavras[0][3].palavra = "NATURE _ A";
        palavras[0][3].letra_correta = "Z";
        palavras[0][3].opcao1 = "S";
        palavras[0][3].palavra_completa = "NATUREZA"; //Ele tosse muito?
        palavras[0][3].nivel = 1;
		palavras[0][3].nome_audio_menino = "naturezamenino";
		palavras[0][3].nome_audio_menina = "naturezamenina";
		palavras[0][3].nome_audio_ditado = "naturezaditado";

        palavras[0][4].palavra = "REPRE _ A";
        palavras[0][4].letra_correta = "S";
        palavras[0][4].opcao1 = "Z";
        palavras[0][4].palavra_completa = "REPRESA"; //Assustar as pessoas é chato!
        palavras[0][4].nivel = 1;
		palavras[0][4].nome_audio_menino = "represamenino";
		palavras[0][4].nome_audio_menina = "represamenina";
		palavras[0][4].nome_audio_ditado = "represaditado";

        palavras[0][5].palavra = "FANTA _ IA";
        palavras[0][5].letra_correta = "S";
        palavras[0][5].opcao1 = "Z";
        palavras[0][5].palavra_completa = "FANTASIA"; //O pássaro voa bem alto.
        palavras[0][5].nivel = 1;
		palavras[0][5].nome_audio_menino = "fantasiamenino";
        palavras[0][5].nome_audio_menina = "fantasiamenina";
		palavras[0][5].nome_audio_ditado = "fantasiaditado";

        palavras[0][6].palavra = "TE _ OURO";
        palavras[0][6].letra_correta = "S";
        palavras[0][6].opcao1 = "Z";
        palavras[0][6].palavra_completa = "TESOURO"; //A professora entregou a tarefa aos alunos.
        palavras[0][6].nivel = 1;
		palavras[0][6].nome_audio_menino = "tesouromenino";
        palavras[0][6].nome_audio_menina = "tesouromenina";
		palavras[0][6].nome_audio_ditado = "tesouroditado";

        palavras[0][7].palavra = "RAPO _ A";
        palavras[0][7].letra_correta = "S";
        palavras[0][7].opcao1 = "Z";
        palavras[0][7].palavra_completa = "RAPOSA"; //Gosto de girassol.
        palavras[0][7].nivel = 1;
		palavras[0][7].nome_audio_menino = "raposamenino";
        palavras[0][7].nome_audio_menina = "raposamenina";
		palavras[0][7].nome_audio_ditado = "raposaditado";

        palavras[0][8].palavra = "PRE _ A";
        palavras[0][8].letra_correta = "S";
        palavras[0][8].opcao1 = "Z";
        palavras[0][8].palavra_completa = "PRESA"; //O osso do meu cachorro é grande!
        palavras[0][8].nivel = 1;
		palavras[0][8].nome_audio_menino = "presamenino";
        palavras[0][8].nome_audio_menina = "presamenina";
		palavras[0][8].nome_audio_ditado = "presaditado";

        palavras[0][9].palavra = "BE _ OUROS";
        palavras[0][9].letra_correta = "S";
        palavras[0][9].opcao1 = "Z";
        palavras[0][9].palavra_completa = "BESOUROS"; //O frango assado da minha mãe é delicioso!
        palavras[0][9].nivel = 1;
		palavras[0][9].nome_audio_menino = "besourosmenino";
        palavras[0][9].nome_audio_menina = "besourosmenina";
		palavras[0][9].nome_audio_ditado = "besourosditado";

		palavras[0][10].palavra = "CA _ A";
		palavras[0][10].letra_correta = "S";
		palavras[0][10].opcao1 = "Z";
		palavras[0][10].palavra_completa = "CASA"; //O frango assado da minha mãe é delicioso!
		palavras[0][10].nivel = 1;
		palavras[0][10].nome_audio_menino = "casamenino";
		palavras[0][10].nome_audio_menina = "casamenina";
		palavras[0][10].nome_audio_ditado = "casaditado";

		palavras[0][11].palavra = "A _ UL";
		palavras[0][11].letra_correta = "Z";
		palavras[0][11].opcao1 = "S";
		palavras[0][11].palavra_completa = "AZUL"; //O frango assado da minha mãe é delicioso!
		palavras[0][11].nivel = 1;
		palavras[0][11].nome_audio_menino = "azulmenino";
		palavras[0][11].nome_audio_menina = "azulmenina";
		palavras[0][11].nome_audio_ditado = "azulditado";

		palavras[0][12].palavra = "VA _ OS";
		palavras[0][12].letra_correta = "S";
		palavras[0][12].opcao1 = "Z";
		palavras[0][12].palavra_completa = "VASOS"; //O frango assado da minha mãe é delicioso!
		palavras[0][12].nivel = 1;
		palavras[0][12].nome_audio_menino = "vasosmenino";
		palavras[0][12].nome_audio_menina = "vasosmenina";
		palavras[0][12].nome_audio_ditado = "vasosditado";

		palavras[0][13].palavra = "FA _ ENDEIRO";
		palavras[0][13].letra_correta = "Z";
		palavras[0][13].opcao1 = "S";
		palavras[0][13].palavra_completa = "FAZENDEIRO"; //O frango assado da minha mãe é delicioso!
		palavras[0][13].nivel = 1;
		palavras[0][13].nome_audio_menino = "fazendeiromenino";
		palavras[0][13].nome_audio_menina = "fazendeiromenina";
		palavras[0][13].nome_audio_ditado = "fazendeiroditado";

		palavras[0][14].palavra = "CORAJO _ O";
		palavras[0][14].letra_correta = "S";
		palavras[0][14].opcao1 = "Z";
		palavras[0][14].palavra_completa = "CORAJOSO"; //O frango assado da minha mãe é delicioso!
		palavras[0][14].nivel = 1;
		palavras[0][14].nome_audio_menino = "corajosomenino";
		palavras[0][14].nome_audio_menina = "corajosomenina";
		palavras[0][14].nome_audio_ditado = "corajosoditado";

		palavras[0][15].palavra = "PERIGO _ A";
		palavras[0][15].letra_correta = "S";
		palavras[0][15].opcao1 = "Z";
		palavras[0][15].palavra_completa = "PERIGOSA"; //O frango assado da minha mãe é delicioso!
		palavras[0][15].nivel = 1;
		palavras[0][15].nome_audio_menino = "perigosamenino";
		palavras[0][15].nome_audio_menina = "perigosamenina";
		palavras[0][15].nome_audio_ditado = "perigosaditado";

		palavras[0][16].palavra = "RO _ AS";
		palavras[0][16].letra_correta = "S";
		palavras[0][16].opcao1 = "Z";
		palavras[0][16].palavra_completa = "ROSAS"; //O frango assado da minha mãe é delicioso!
		palavras[0][16].nivel = 1;
		palavras[0][16].nome_audio_menino = "rosasmenino";
		palavras[0][16].nome_audio_menina = "rosasmenina";
		palavras[0][16].nome_audio_ditado = "rosasditado";

		palavras[0][17].palavra = "TE _ OURA";
		palavras[0][17].letra_correta = "S";
		palavras[0][17].opcao1 = "Z";
		palavras[0][17].palavra_completa = "TESOURA"; //O frango assado da minha mãe é delicioso!
		palavras[0][17].nivel = 1;
		palavras[0][17].nome_audio_menino = "tesouramenino";
		palavras[0][17].nome_audio_menina = "tesouramenina";
		palavras[0][17].nome_audio_ditado = "tesouraditado";

		palavras[0][18].palavra = "GULO _ EIMAS";
		palavras[0][18].letra_correta = "S";
		palavras[0][18].opcao1 = "Z";
		palavras[0][18].palavra_completa = "GULOSEIMAS"; //O frango assado da minha mãe é delicioso!
		palavras[0][18].nivel = 1;
		palavras[0][18].nome_audio_menino = "guloseimasmenino";
		palavras[0][18].nome_audio_menina = "guloseimasmenina";
		palavras[0][18].nome_audio_ditado = "guloseimasditado";

		palavras[0][19].palavra = "CO _ IDO";
		palavras[0][19].letra_correta = "Z";
		palavras[0][19].opcao1 = "S";
		palavras[0][19].palavra_completa = "COZIDO"; //O frango assado da minha mãe é delicioso!
		palavras[0][19].nivel = 1;
		palavras[0][19].nome_audio_menino = "cozidomenino";
		palavras[0][19].nome_audio_menina = "cozidomenina";
		palavras[0][19].nome_audio_ditado = "cozidoditado";

		palavras[0][20].palavra = "CA _ AMENTO";
		palavras[0][20].letra_correta = "S";
		palavras[0][20].opcao1 = "Z";
		palavras[0][20].palavra_completa = "CASAMENTO"; //O frango assado da minha mãe é delicioso!
		palavras[0][20].nivel = 1;
		palavras[0][20].nome_audio_menino = "casamentomenino";
		palavras[0][20].nome_audio_menina = "casamentomenina";
		palavras[0][20].nome_audio_ditado = "casamentoditado";

		palavras[0][21].palavra = "PRE _ ENTES";
		palavras[0][21].letra_correta = "S";
		palavras[0][21].opcao1 = "Z";
		palavras[0][21].palavra_completa = "PRESENTES"; //O frango assado da minha mãe é delicioso!
		palavras[0][21].nivel = 1;
		palavras[0][21].nome_audio_menino = "presentesmenino";
		palavras[0][21].nome_audio_menina = "presentesmenina";
		palavras[0][21].nome_audio_ditado = "presentesditado";

		palavras[0][22].palavra = "RE _ AS";
		palavras[0][22].letra_correta = "Z";
		palavras[0][22].opcao1 = "S";
		palavras[0][22].palavra_completa = "REZAS"; //O frango assado da minha mãe é delicioso!
		palavras[0][22].nivel = 1;
		palavras[0][22].nome_audio_menino = "rezasmenino";
		palavras[0][22].nome_audio_menina = "rezasmenina";
		palavras[0][22].nome_audio_ditado = "rezasditado";

		palavras[0][23].palavra = "SURPRE _ AS";
		palavras[0][23].letra_correta = "S";
		palavras[0][23].opcao1 = "Z";
		palavras[0][23].palavra_completa = "SURPRESAS"; //O frango assado da minha mãe é delicioso!
		palavras[0][23].nivel = 1;
		palavras[0][23].nome_audio_menino = "surpresasmenino";
		palavras[0][23].nome_audio_menina = "surpresasmenina";
		palavras[0][23].nome_audio_ditado = "surpresasditado";

		palavras[0][24].palavra = "VI _ INHANÇA";
		palavras[0][24].letra_correta = "Z";
		palavras[0][24].opcao1 = "S";
		palavras[0][24].palavra_completa = "VIZINHANÇA"; //O frango assado da minha mãe é delicioso!
		palavras[0][24].nivel = 1;
		palavras[0][24].nome_audio_menino = "vizinhancamenino";
		palavras[0][24].nome_audio_menina = "vizinhancamenina";
		palavras[0][24].nome_audio_ditado = "vizinhancaditado";

		palavras[0][25].palavra = "VI _ ITAR";
		palavras[0][25].letra_correta = "S";
		palavras[0][25].opcao1 = "Z";
		palavras[0][25].palavra_completa = "VISITAR"; //O frango assado da minha mãe é delicioso!
		palavras[0][25].nivel = 1;
		palavras[0][25].nome_audio_menino = "visitarmenino";
		palavras[0][25].nome_audio_menina = "visitarmenina";
		palavras[0][25].nome_audio_ditado = "visitarditado";

		palavras[0][26].palavra = "BU _ INA";
		palavras[0][26].letra_correta = "Z";
		palavras[0][26].opcao1 = "S";
		palavras[0][26].palavra_completa = "BUZINA"; //O frango assado da minha mãe é delicioso!
		palavras[0][26].nivel = 1;
		palavras[0][26].nome_audio_menino = "buzinamenino";
		palavras[0][26].nome_audio_menina = "buzinamenina";
		palavras[0][26].nome_audio_ditado = "buzinaditado";

		palavras[0][27].palavra = "AVI _ A";
		palavras[0][27].letra_correta = "S";
		palavras[0][27].opcao1 = "Z";
		palavras[0][27].palavra_completa = "AVISA"; //O frango assado da minha mãe é delicioso!
		palavras[0][27].nivel = 1;
		palavras[0][27].nome_audio_menino = "avisamenino";
		palavras[0][27].nome_audio_menina = "avisamenina";
		palavras[0][27].nome_audio_ditado = "avisaditado";

		palavras[0][28].palavra = "BRA _ A";
		palavras[0][28].letra_correta = "S";
		palavras[0][28].opcao1 = "Z";
		palavras[0][28].palavra_completa = "BRASA"; //O frango assado da minha mãe é delicioso!
		palavras[0][28].nivel = 1;
		palavras[0][28].nome_audio_menino = "brasamenino";
		palavras[0][28].nome_audio_menina = "brasamenina";
		palavras[0][28].nome_audio_ditado = "brasaditado";

		palavras[0][29].palavra = "ME _ A";
		palavras[0][29].letra_correta = "S";
		palavras[0][29].opcao1 = "Z";
		palavras[0][29].palavra_completa = "MESA"; //O frango assado da minha mãe é delicioso!
		palavras[0][29].nivel = 1;
		palavras[0][29].nome_audio_menino = "mesamenino";
		palavras[0][29].nome_audio_menina = "mesamenina";
		palavras[0][29].nome_audio_ditado = "mesaditado";

		palavras[0][30].palavra = "MÚ _ ICA";
		palavras[0][30].letra_correta = "S";
		palavras[0][30].opcao1 = "Z";
		palavras[0][30].palavra_completa = "MÚSICA"; //O frango assado da minha mãe é delicioso!
		palavras[0][30].nivel = 1;
		palavras[0][30].nome_audio_menino = "musicamenino";
		palavras[0][30].nome_audio_menina = "musicamenina";
		palavras[0][30].nome_audio_ditado = "musicaditado";

		palavras[0][31].palavra = "AVI _ E";
		palavras[0][31].letra_correta = "S";
		palavras[0][31].opcao1 = "Z";
		palavras[0][31].palavra_completa = "AVISE"; //O frango assado da minha mãe é delicioso!
		palavras[0][31].nivel = 1;
		palavras[0][31].nome_audio_menino = "avisemenino";
		palavras[0][31].nome_audio_menina = "avisemenina";
		palavras[0][31].nome_audio_ditado = "aviseditado";



		/*

		//NÍVEL 2 - S e Z!

		// Palavra: Natal
		palavras[0][32].palavra = "NATA_";
		palavras[0][32].letra_correta = "L";
		palavras[0][32].opcao1 = "U";
		palavras[0][32].palavra_completa = "NATAL"; //Finalmente as férias chegaram! E junto com ela o Natal! Que legal!. 
		palavras[0][32].nivel = 3;
		palavras[0][32].nome_audio_menino = "natalmenino"; 
		palavras[0][32].nome_audio_menina = "natalmenina";
		palavras[0][32].nome_audio_ditado = "natalditado";

		// Palavra: céu
		palavras[0][33].palavra = "CÉ_";
		palavras[0][33].letra_correta = "U";
		palavras[0][33].opcao1 = "L";
		palavras[0][33].palavra_completa = "CÉU"; //O céu fica azul no verão e o sol brilha. 
		palavras[0][33].nivel = 3;
		palavras[0][33].nome_audio_menino = "céumenino"; 
		palavras[0][33].nome_audio_menina = "céumenina";
		palavras[0][33].nome_audio_ditado = "céuditado";

		// Palavra: especial
		palavras[0][34].palavra = "ESPECIA_";
		palavras[0][34].letra_correta = "L";
		palavras[0][34].opcao1 = "U";
		palavras[0][34].palavra_completa = "ESPECIAL"; //Tudo isso é muito especial!. 
		palavras[0][34].nivel = 3;
		palavras[0][34].nome_audio_menino = "especialmenino"; 
		palavras[0][34].nome_audio_menina = "especialmenina";
		palavras[0][34].nome_audio_ditado = "especialditado";

		// Palavra: varal
		palavras[0][35].palavra = "VARA_";
		palavras[0][35].letra_correta = "L";
		palavras[0][35].opcao1 = "U";
		palavras[0][35].palavra_completa = "VARAL"; //Os irmãos fotografam a praia todos os dias, revelam as fotos e as colocam em um varal. 
		palavras[0][35].nivel = 3;
		palavras[0][35].nome_audio_menino = "varalmenino"; 
		palavras[0][35].nome_audio_menina = "varalmenina";
		palavras[0][35].nome_audio_ditado = "varalditado";

		// Palavra: brasil
		palavras[0][36].palavra = "BRASI_";
		palavras[0][36].letra_correta = "L";
		palavras[0][36].opcao1 = "U";
		palavras[0][36].palavra_completa = "BRASIL"; //Marcos e Julinha moram no Brasil. 
		palavras[0][36].nivel = 3;
		palavras[0][36].nome_audio_menino = "brasilmenino"; 
		palavras[0][36].nome_audio_menina = "brasilmenina";
		palavras[0][36].nome_audio_ditado = "brasilditado";

		// Palavra: quintal
		palavras[0][37].palavra = "QUINTA_";
		palavras[0][37].letra_correta = "L";
		palavras[0][37].opcao1 = "U";
		palavras[0][37].palavra_completa = "QUINTAL"; //No verão, a praia vira um quintal sensacional para eles. 
		palavras[0][37].nivel = 3;
		palavras[0][37].nome_audio_menino = "quintalmenino"; 
		palavras[0][37].nome_audio_menina = "quintalmenina";
		palavras[0][37].nome_audio_ditado = "quintalditado";

		// Palavra: gentil
		palavras[0][38].palavra = "GENTI_";
		palavras[0][38].letra_correta = "L";
		palavras[0][38].opcao1 = "U";
		palavras[0][38].palavra_completa = "GENTIL"; //O vendedor de picolé é sempre gentil e dócil. 
		palavras[0][38].nivel = 3;
		palavras[0][38].nome_audio_menino = "gentilmenino"; 
		palavras[0][38].nome_audio_menina = "gentilmenina";
		palavras[0][38].nome_audio_ditado = "gentilditado";

		// Palavra: dócil
		palavras[0][39].palavra = "DÓCI_";
		palavras[0][39].letra_correta = "L";
		palavras[0][39].opcao1 = "U";
		palavras[0][39].palavra_completa = "DÓCIL"; //O vendedor de picolé é sempre gentil e dócil. 
		palavras[0][39].nivel = 3;
		palavras[0][39].nome_audio_menino = "dócilmenino"; 
		palavras[0][39].nome_audio_menina = "dócilmenina";
		palavras[0][39].nome_audio_ditado = "dócilditado";

		// Palavra: farol
		palavras[0][40].palavra = "FARO_";
		palavras[0][40].letra_correta = "L";
		palavras[0][40].opcao1 = "U";
		palavras[0][40].palavra_completa = "FAROL"; //O farol brilha à noite no mar. 
		palavras[0][40].nivel = 3;
		palavras[0][40].nome_audio_menino = "farolmenino"; 
		palavras[0][40].nome_audio_menina = "farolmenina";
		palavras[0][40].nome_audio_ditado = "farolditado";

		// Palavra: fácil
		palavras[0][41].palavra = "FÁCI_";
		palavras[0][41].letra_correta = "L";
		palavras[0][41].opcao1 = "U";
		palavras[0][41].palavra_completa = "FÁCIL"; //Mas a diversão principal é muito fácil. 
		palavras[0][41].nivel = 3;
		palavras[0][41].nome_audio_menino = "fácilmenino"; 
		palavras[0][41].nome_audio_menina = "fácilmenina";
		palavras[0][41].nome_audio_ditado = "fácilditado";

		// Palavra: túnel
		palavras[0][42].palavra = "TÚNE_";
		palavras[0][42].letra_correta = "L";
		palavras[0][42].opcao1 = "U";
		palavras[0][42].palavra_completa = "TÚNEL"; //Fazer túnel e castelo de areia. 
		palavras[0][42].nivel = 3;
		palavras[0][42].nome_audio_menino = "túnelmenino"; 
		palavras[0][42].nome_audio_menina = "túnelmenina";
		palavras[0][42].nome_audio_ditado = "túnelditado";

		// Palavra: sinal
		palavras[0][43].palavra = "SINA_";
		palavras[0][43].letra_correta = "L";
		palavras[0][43].opcao1 = "U";
		palavras[0][43].palavra_completa = "SINAL"; //Hoje não havia nuvens no céu, e céu sem nuvens é sinal de muito sol. 
		palavras[0][43].nivel = 3;
		palavras[0][43].nome_audio_menino = "sinalmenino"; 
		palavras[0][43].nome_audio_menina = "sinalmenina";
		palavras[0][43].nome_audio_ditado = "sinalditado";

		// Palavra: chapéu
		palavras[0][44].palavra = "CHAPÉ_";
		palavras[0][44].letra_correta = "U";
		palavras[0][44].opcao1 = "L";
		palavras[0][44].palavra_completa = "CHAPÉU"; //Usar chapéu e roupas leves é uma recomendação. 
		palavras[0][44].nivel = 3;
		palavras[0][44].nome_audio_menino = "chapéumenino"; 
		palavras[0][44].nome_audio_menina = "chapéumenina";
		palavras[0][44].nome_audio_ditado = "chapéuditado";

		// Palavra: roupas
		palavras[0][45].palavra = "RO_PAS";
		palavras[0][45].letra_correta = "U";
		palavras[0][45].opcao1 = "L";
		palavras[0][45].palavra_completa = "ROUPAS"; //Usar chapéu e roupas leves é uma recomendação. 
		palavras[0][45].nivel = 3;
		palavras[0][45].nome_audio_menino = "roupasmenino"; 
		palavras[0][45].nome_audio_menina = "roupasmenina";
		palavras[0][45].nome_audio_ditado = "roupasditado";

		// Palavra: seu
		palavras[0][46].palavra = "SE_";
		palavras[0][46].letra_correta = "U";
		palavras[0][46].opcao1 = "L";
		palavras[0][46].palavra_completa = "SEU"; //Ela tomou tanto sol, que seu corpo ficou febril. 
		palavras[0][46].nivel = 3;
		palavras[0][46].nome_audio_menino = "seumenino"; 
		palavras[0][46].nome_audio_menina = "seumenina";
		palavras[0][46].nome_audio_ditado = "seuditado";

		// Palavra: animal
		palavras[0][47].palavra = "ANIMA_";
		palavras[0][47].letra_correta = "L";
		palavras[0][47].opcao1 = "U";
		palavras[0][47].palavra_completa = "ANIMAL"; //É preciso oferecer água até pro seu animalzinho 
		palavras[0][47].nivel = 3;
		palavras[0][47].nome_audio_menino = "animalmenino"; 
		palavras[0][47].nome_audio_menina = "animalmenina";
		palavras[0][47].nome_audio_ditado = "animalditado";

		// Palavra: sal
		palavras[0][48].palavra = "SA_";
		palavras[0][48].letra_correta = "L";
		palavras[0][48].opcao1 = "U";
		palavras[0][48].palavra_completa = "SAL"; //Não pode ser água do mar porque contém muito sal. 
		palavras[0][48].nivel = 3;
		palavras[0][48].nome_audio_menino = "salmenino"; 
		palavras[0][48].nome_audio_menina = "salmenina";
		palavras[0][48].nome_audio_ditado = "salditado";

		// Palavra: calça
		palavras[0][49].palavra = "CA_ÇA";
		palavras[0][49].letra_correta = "L";
		palavras[0][49].opcao1 = "U";
		palavras[0][49].palavra_completa = "CALÇA"; //Nesse dia, Marcos nadou de calça comprida. 
		palavras[0][49].nivel = 3;
		palavras[0][49].nome_audio_menino = "calçamenino"; 
		palavras[0][49].nome_audio_menina = "calçamenina";
		palavras[0][49].nome_audio_ditado = "calçaditado";

		// Palavra: difícil
		palavras[0][50].palavra = "DIFÍCI_";
		palavras[0][50].letra_correta = "L";
		palavras[0][50].opcao1 = "U";
		palavras[0][50].palavra_completa = "DIFÍCIL"; //Foi muito difícil pular as ondas altas. 
		palavras[0][50].nivel = 3;
		palavras[0][50].nome_audio_menino = "difícilmenino"; 
		palavras[0][50].nome_audio_menina = "difícilmenina";
		palavras[0][50].nome_audio_ditado = "difícilditado";

		// Palavra: altas
		palavras[0][51].palavra = "A_TAS";
		palavras[0][51].letra_correta = "L";
		palavras[0][51].opcao1 = "U";
		palavras[0][51].palavra_completa = "ALTAS"; //Foi muito difícil pular as ondas altas. 
		palavras[0][51].nivel = 3;
		palavras[0][51].nome_audio_menino = "altasmenino"; 
		palavras[0][51].nome_audio_menina = "altasmenina";
		palavras[0][51].nome_audio_ditado = "altasditado";

		// Palavra: anzol
		palavras[0][52].palavra = "ANZO_";
		palavras[0][52].letra_correta = "L";
		palavras[0][52].opcao1 = "U";
		palavras[0][52].palavra_completa = "DIFÍCIL"; //Ele foi pescar e levou: anzol, iscas e um barquinho que não era de papel. 
		palavras[0][52].nivel = 3;
		palavras[0][52].nome_audio_menino = "anzolmenino"; 
		palavras[0][52].nome_audio_menina = "anzolmenina";
		palavras[0][52].nome_audio_ditado = "anzolditado";

		// Palavra: papel
		palavras[0][53].palavra = "PAPE_";
		palavras[0][53].letra_correta = "L";
		palavras[0][53].opcao1 = "U";
		palavras[0][53].palavra_completa = "DIFÍCIL"; //Ele foi pescar e levou: anzol, iscas e um barquinho que não era de papel. 
		palavras[0][53].nivel = 3;
		palavras[0][53].nome_audio_menino = "papelmenino"; 
		palavras[0][53].nome_audio_menina = "papelmenina";
		palavras[0][53].nome_audio_ditado = "papelditado";

		// Palavra: pneu
		palavras[0][54].palavra = "PNE_";
		palavras[0][54].letra_correta = "U";
		palavras[0][54].opcao1 = "L";
		palavras[0][54].palavra_completa = "PNEU"; //Marcos imaginou que o pneu era seu grande barco que quase virou nas ondas e bateu em um grande coral. 
		palavras[0][54].nivel = 3;
		palavras[0][54].nome_audio_menino = "pneumenino"; 
		palavras[0][54].nome_audio_menina = "pneumenina";
		palavras[0][54].nome_audio_ditado = "pneuditado";

		// Palavra: coral
		palavras[0][55].palavra = "CORA_";
		palavras[0][55].letra_correta = "L";
		palavras[0][55].opcao1 = "U";
		palavras[0][55].palavra_completa = "CORAL"; //Marcos imaginou que o pneu era seu grande barco que quase virou nas ondas e bateu em um grande coral. 
		palavras[0][55].nivel = 3;
		palavras[0][55].nome_audio_menino = "coralmenino"; 
		palavras[0][55].nome_audio_menina = "coralmenina";
		palavras[0][55].nome_audio_ditado = "coralditado";

		// Palavra: pau
		palavras[0][56].palavra = "PA_";
		palavras[0][56].letra_correta = "U";
		palavras[0][56].opcao1 = "L";
		palavras[0][56].palavra_completa = "PAU"; //Estava brincando de "pirata de perna de pau, de olho de vidro e da cara de mau". 
		palavras[0][56].nivel = 3;
		palavras[0][56].nome_audio_menino = "paumenino"; 
		palavras[0][56].nome_audio_menina = "paumenina";
		palavras[0][56].nome_audio_ditado = "pauditado";

		// Palavra: mau
		palavras[0][57].palavra = "MA_";
		palavras[0][57].letra_correta = "U";
		palavras[0][57].opcao1 = "L";
		palavras[0][57].palavra_completa = "MAU"; //Marcos imaginou que o pneu era seu grande barco que quase virou nas ondas e bateu em um grande coral. 
		palavras[0][57].nivel = 3;
		palavras[0][57].nome_audio_menino = "maumenino"; 
		palavras[0][57].nome_audio_menina = "maumenina";
		palavras[0][57].nome_audio_ditado = "mauditado";

		// Palavra: almanaque
		palavras[0][58].palavra = "A_MANAQUE";
		palavras[0][58].letra_correta = "L";
		palavras[0][58].opcao1 = "U";
		palavras[0][58].palavra_completa = "MAU"; //Marcos e Julinha possuem um almanaque que explica tudo sobre o fundo do mar. 
		palavras[0][58].nivel = 3;
		palavras[0][58].nome_audio_menino = "almanaquemenino"; 
		palavras[0][58].nome_audio_menina = "almanaquemenina";
		palavras[0][58].nome_audio_ditado = "almanaqueditado";

		// Palavra: degraus
		palavras[0][59].palavra = "DEGRA_S";
		palavras[0][59].letra_correta = "U";
		palavras[0][59].opcao1 = "L";
		palavras[0][59].palavra_completa = "DEGRAUS"; //Quando anoitecer, os irmãos costumam subir os degraus para o andar superior. 
		palavras[0][59].nivel = 3;
		palavras[0][59].nome_audio_menino = "degrausmenino"; 
		palavras[0][59].nome_audio_menina = "degrausmenina";
		palavras[0][59].nome_audio_ditado = "degrausditado";

		// Palavra: pouco
		palavras[0][60].palavra = "PO_CO";
		palavras[0][60].letra_correta = "U";
		palavras[0][60].opcao1 = "L";
		palavras[0][60].palavra_completa = "POUCO"; //pena que tudo que é bom, dura pouco... 
		palavras[0][60].nivel = 3;
		palavras[0][60].nome_audio_menino = "poucomenino"; 
		palavras[0][60].nome_audio_menina = "poucomenina";
		palavras[0][60].nome_audio_ditado = "poucoditado";

		// Palavra: mingau
		palavras[0][61].palavra = "MINGA_";
		palavras[0][61].letra_correta = "U";
		palavras[0][61].opcao1 = "L";
		palavras[0][61].palavra_completa = "MINGAU"; //Agora está na hora de dormir, tomar mingau adoçadinho com mel 
		palavras[0][61].nivel = 3;
		palavras[0][61].nome_audio_menino = "mingaumenino"; 
		palavras[0][61].nome_audio_menina = "mingaumenina";
		palavras[0][61].nome_audio_ditado = "mingauditado";

		// Palavra: mel
		palavras[0][62].palavra = "ME_";
		palavras[0][62].letra_correta = "L";
		palavras[0][62].opcao1 = "U";
		palavras[0][62].palavra_completa = "MEL"; //Agora está na hora de dormir, tomar mingau adoçadinho com mel 
		palavras[0][62].nivel = 3;
		palavras[0][62].nome_audio_menino = "melmenino"; 
		palavras[0][62].nome_audio_menina = "melmenina";
		palavras[0][62].nome_audio_ditado = "melditado";

		// Palavra: final
		palavras[0][63].palavra = "FINA_";
		palavras[0][63].letra_correta = "L";
		palavras[0][63].opcao1 = "U";
		palavras[0][63].palavra_completa = "FINAL"; //acordar bem cedo para aproveitar o dia antes que as férias cheguem ao final
		palavras[0][63].nivel = 3;
		palavras[0][63].nome_audio_menino = "finalmenino"; 
		palavras[0][63].nome_audio_menina = "finalmenina";
		palavras[0][63].nome_audio_ditado = "finalditado";
		*/

		for (int i = 0; i < maxLevel; ++i)
          {

			for (int j = 0; j < numWordsLevel1; ++j) {
				_connection.Insert (palavras [i] [j]);

				reference.Child ("palavras").Child (palavras [i] [j].Id.ToString() + "/palavra").SetValueAsync (palavras [i] [j].palavra);
				reference.Child ("palavras").Child (palavras [i] [j].Id.ToString() + "/letra_correta").SetValueAsync (palavras [i] [j].letra_correta);
				reference.Child ("palavras").Child (palavras [i] [j].Id.ToString() + "/opcao1").SetValueAsync (palavras [i] [j].opcao1);
				reference.Child ("palavras").Child (palavras [i] [j].Id.ToString() + "/palavra_completa").SetValueAsync (palavras [i] [j].palavra_completa);
				reference.Child ("palavras").Child (palavras [i] [j].Id.ToString() + "/nivel").SetValueAsync (palavras [i] [j].nivel);
				reference.Child ("palavras").Child (palavras [i] [j].Id.ToString() + "/nome_audio_menino").SetValueAsync (palavras [i] [j].nome_audio_menino);
				reference.Child ("palavras").Child (palavras [i] [j].Id.ToString() + "/nome_audio_menina").SetValueAsync (palavras [i] [j].nome_audio_menina);
			}

          }

    }


}


public class comandosBasicos : MonoBehaviour {

	void Start(){
		// Conexão com o banco de dados grapphia!
		var filepath = string.Format("{0}/{1}", Application.persistentDataPath, "grapphia");



		if (!File.Exists(filepath))
		{

			var loadDb = new WWW("jar:file://" + Application.dataPath + "!/assets/" + "grapphia");  

			while (!loadDb.isDone) { }  


			File.WriteAllBytes(filepath, loadDb.bytes);



		}

		var dbPath = filepath;

		bancoPalavras.Instance._connection = new SQLiteConnection(dbPath, SQLiteOpenFlags.ReadWrite | SQLiteOpenFlags.Create);


	     Debug.Log("Final PATH: " + dbPath);
		//Criando tabela usuário e tabela opção!
		bancoPalavras.Instance._connection.CreateTable<user>();
		bancoPalavras.Instance._connection.CreateTable<palavraOpcao>();
		bancoPalavras.Instance._connection.CreateTable<palavraAcertoUser>();
	}

	public string getKey(){
		var key = bancoPalavras.Instance._connection.Table<keyPhone> ().FirstOrDefault().keyFireBase;
		return key;
	}

    // Carregar cena!
    public void loadScene(string nameScene)
    {

        //Application.LoadLevel(nameScene);
		SceneManager.LoadScene (nameScene);

    }
	public void loadScene_SignOut(string nameScene){
		FirebaseAuth.DefaultInstance.SignOut();
		SceneManager.LoadScene (nameScene);
	}

    // Carregar a cena quando selecionar o cowboy!
    public void loadSceneBoy(string nameScene)
    {

        dadosJogo.Instance.currentPesonagem = 2;

        //Application.LoadLevel(nameScene);
		SceneManager.LoadScene (nameScene);
        
    }

    // Carregar a cena quando selecionar a cowgirl!
    public void loadSceneGirl(string nameScene)
    {

        dadosJogo.Instance.currentPesonagem = 1;
		SceneManager.LoadScene(nameScene);


    }

    // Fechar jogo!
    public void exit()
    {
        dadosJogo.Instance.salvar_dados();
		FirebaseAuth.DefaultInstance.SignOut();
        Application.Quit();

    }
    public void exitInicial()
    {
		FirebaseAuth.DefaultInstance.SignOut();
        Application.Quit();

    }


}

// Classe de armazenar palavras e atributos no banco de dados!
public class palavraOpcao
{
 
    [PrimaryKey, AutoIncrement]
    public int Id { get; set; }

    public string palavra_completa { get; set; }

    public string palavra { get; set; }
    
    public string letra_correta { get; set; }
    public string opcao1 { get; set; }
    public string opcao2 { get; set; }

    public int nivel { get; set; }

    public string nome_audio_menino { get; set; }
	public string nome_audio_menina { get; set; } 

	//Para os áudios do ditado
	public string nome_audio_ditado { get; set; }

    // Função que retorna como string a palavra!
    public override string ToString()
    {
        return string.Format("{1}", palavra_completa);
    }

}


public class palavraAcertoUser
{

    [PrimaryKey, AutoIncrement]
    public int Id { get; set; }

    public int idUser { get; set; }

    public int idPalavra { get; set; }

    public bool acerto { get; set; }

    public int nivelPalavra { get; set; }

}

public class keyPhone
{
	[PrimaryKey]
	public string keyFireBase { get; set; }

}


// Classe singleton o dos dados correntes do jogo, quem é atual jogador, quem é atual personagem do jogo, quantos erros, etc.!
public class dadosJogo
{
    private static dadosJogo instance;

    public user currentUser;

    public int currentPesonagem;

    public double ultimo_desempenho;

    private dadosJogo() {
		
    }
        
    // Atributo estático!
    public static dadosJogo Instance
    {
        get
        {
            if (instance == null)
            {
                instance = new dadosJogo();
            }
            return instance;
        }
    }

    public void salvar_dados()
    {
        bancoPalavras.Instance.salvar_palavrasAcertoUser();
        bancoPalavras.Instance._connection.Update(currentUser);

    }

}

public class inteligencia
{

    private static inteligencia instance;

    private inteligencia() { }


    public static inteligencia Instance
    {
        get
        {
            if (instance == null)
            {
                instance = new inteligencia();
            }
            return instance;
        }
    }



    public double DesempenhoUsuario()
    {
        double resultado = (double)(bancoPalavras.Instance.acertos) / (bancoPalavras.Instance.qtd_Words); 
        // Calculando desempenho do usuário!
        return resultado;
    }


	//Selecionar nessa função o nível correspondente ao seu mundo
    public void seleciona_nivel()
    {

		dadosJogo.Instance.currentUser.Nivel = 0;
    }
}


