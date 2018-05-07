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

    public palavraOpcao[] palavras;

	public palavraAcertoUser[] palavrasAcerto;

	public List<int> ListaIdPalavraAcerto = new List<int> (); 

	public int qtd_Words; //quantidade de palavras recuperadas no BD de um determinado nível (nível atual do jogador)

	public int qtd_WordsPresented; //quantidade de palavras recuperadas no BD que determinado usuário já jogou

	public int numWordsGame = 15; //número de palavras que serão apresentadas para o usuário em cada nível

	public int numWordsDitado = 7; //número de palavras que serão apresentadas para o usuário no ditado

    public DataService dataservice;

    public int acertos, erros;//armazena acertos e erros totais por usuário

    public int total_palavras_geral; //total de palavras no BD em todos os níveis. Essa variável é inicializada quando é feita a busca de palavras no banco. 

	public int[] total_palavras_Nivel = new int[]{32,32,32}; //quantidade de palavras armazenadas em cada nivel

	public int maxLevel = 3; //quantidade de níveis (mundos) implementados

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
		this.total_palavras_geral = total_palavras;
	}

	/** Função que busca no banco de dados um conjunto de palavras do nível especificado
	 * level: indica o nível das palavras que será feita a busca no BD
	 * As palavras retornadas do BD serão armazenadas no vetor palavras[][]
	 */
	public void carrega_palavras_nivel(int level) {
		int totalPalavras = total_palavras_Nivel[level];

        int aux = (level + 1);

		Debug.Log ("Selecionando palavras do nível (valor do BD) " + aux);

        //Buscando palavras do nível AUX
		var resul = _connection.Table<palavraOpcao>().Where(x => x.nivel == aux);

		//Busca palavras do nível AUX e usuário específico
        var resul2 = _connection.Table<palavraAcertoUser>().Where(x => ((x.nivelPalavra == aux) && (x.idUser == dadosJogo.Instance.currentUser.Id)));

        qtd_Words = 0;
		qtd_WordsPresented = 0;

		Debug.Log ("level " + level);
		Debug.Log ("maxlevel " + maxLevel);
		Debug.Log ("result.Count " + resul.Count());
		palavras = new palavraOpcao[resul.Count()];
		Debug.Log ("palavras.lenght " + palavras.Length);

        foreach (var p in resul)
        {

            palavras[qtd_Words] = new palavraOpcao
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
		//Setar o vetor para na recuperar lixo em gamecontroller.cs
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
			//palavrasAcerto = new palavraAcertoUser[totalPalavras];

			//for (int j = 0; j < totalPalavras; ++j) palavrasAcerto[j] = new palavraAcertoUser();

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
			palavrasAcerto = new palavraAcertoUser[totalPalavras];
			for (int j = 0; j < totalPalavras; ++j)
			{
				palavrasAcerto[j] = new palavraAcertoUser();
			}
		}*/

    }

    public void salvar_palavrasAcertoUser()
    {
		FirebaseApp.DefaultInstance.SetEditorDatabaseUrl("https://grapphia.firebaseio.com/");
		DatabaseReference reference = FirebaseDatabase.DefaultInstance.RootReference;

		//Firebase.Auth.FirebaseAuth auth = Firebase.Auth.FirebaseAuth.DefaultInstance;
		//Firebase.Auth.FirebaseUser user_fb = auth.CurrentUser;

		var key = bancoPalavras.Instance._connection.Table<keyPhone> ().FirstOrDefault().keyFireBase;

		//var nomeCurrentUser = _connection.Table<user>().Where(x => x.Id == dadosJogo.Instance.currentUser.Id);

		//string uid = user_fb.UserId;
		//string pathFireBase = "palavrasAcertoUser/" + key + "/" + uid + "/" + dadosJogo.Instance.currentUser.Name + "/";
		string auxPalavra = "";
		string pathFireBase = "palavrasAcertoUser/" + key + "/" + dadosJogo.Instance.currentUser.Name + "/";

        for(int i=0; i<palavrasAcerto.Length; ++i)
        {
			

			if (palavrasAcerto [i].idPalavra > 0 && palavrasAcerto [i].Id > 0) {
				_connection.Update (palavrasAcerto [i]);

				auxPalavra = bancoPalavras.Instance.palavras[palavrasAcerto[i].idPalavra - 1].palavra_completa;
				reference.Child (pathFireBase + auxPalavra + "/idPalavra").SetValueAsync (palavrasAcerto [i].idPalavra);
				reference.Child (pathFireBase + auxPalavra + "/acerto").SetValueAsync (palavrasAcerto [i].acerto);
				reference.Child (pathFireBase + auxPalavra + "/nivelpalavra").SetValueAsync (palavrasAcerto [i].nivelPalavra);
				reference.Child (pathFireBase + auxPalavra + "/Nome").SetValueAsync (dadosJogo.Instance.currentUser.Name);



			} else if (palavrasAcerto [i].idPalavra > 0) {
				_connection.Insert (palavrasAcerto [i]);

				auxPalavra = bancoPalavras.Instance.palavras[palavrasAcerto [i].idPalavra - 1].palavra_completa;
				reference.Child(pathFireBase + auxPalavra + "/idPalavra").SetValueAsync(palavrasAcerto[i].idPalavra);
				reference.Child(pathFireBase + auxPalavra + "/acerto").SetValueAsync(palavrasAcerto[i].acerto);
				reference.Child(pathFireBase + auxPalavra + "/nivelpalavra").SetValueAsync(palavrasAcerto[i].nivelPalavra);
				reference.Child(pathFireBase + auxPalavra + "/Nome").SetValueAsync(dadosJogo.Instance.currentUser.Name);


			}
			auxPalavra = "";
        }

    }


    public void salvar_palavras_no_banco()      //salva palavras no banco de dados  essa função é chamada apenas quando for criar o banco de dados

    {
		total_palavras_geral = 64;
		palavraOpcao[] palavrasBanco;
		palavrasBanco = new palavraOpcao[total_palavras_geral];

		DatabaseReference reference = FirebaseDatabase.DefaultInstance.RootReference;

		for (int i = 0; i < total_palavras_geral; ++i)
        	palavrasBanco[i] = new palavraOpcao();

			//NÍVEL 1 - S e Z!
			palavrasBanco[0].palavra = "PAI _ AGEM";
			palavrasBanco[0].letra_correta = "S";
			palavrasBanco[0].opcao1 = "Z";
			palavrasBanco[0].palavra_completa = "PAISAGEM"; //O palhaço é muito engraçado. 
			palavrasBanco[0].nivel = 1;
			palavrasBanco[0].nome_audio_menino = "paisagemmenino";          //atenção os arquivos de audio devem ficar na pasta Resources/audios !! 
			palavrasBanco[0].nome_audio_menina = "paisagemmenina";
			palavrasBanco[0].nome_audio_ditado = "paisagemditado";

			palavrasBanco[1].palavra = "FA _ ENDA";
			palavrasBanco[1].letra_correta = "Z";
			palavrasBanco[1].opcao1 = "S";
			palavrasBanco[1].palavra_completa = "FAZENDA"; //Eu adoro a aula de dança! 
			palavrasBanco[1].nivel = 1;
			palavrasBanco[1].nome_audio_menino = "fazendamenino";
			palavrasBanco[1].nome_audio_menina = "fazendamenina";
			palavrasBanco[1].nome_audio_ditado = "fazendaditado";

			palavrasBanco[2].palavra = "PARAÍ _ O";
			palavrasBanco[2].letra_correta = "S";
			palavrasBanco[2].opcao1 = "Z";
			palavrasBanco[2].palavra_completa = "PARAÍSO"; //Esse assunto é chato !!!
			palavrasBanco[2].nivel = 1;
			palavrasBanco[2].nome_audio_menino = "paraisomenino";
			palavrasBanco[2].nome_audio_menina = "paraisomenina";
			palavrasBanco[2].nome_audio_ditado = "paraisoditado";

			palavrasBanco[3].palavra = "NATURE _ A";
			palavrasBanco[3].letra_correta = "Z";
			palavrasBanco[3].opcao1 = "S";
			palavrasBanco[3].palavra_completa = "NATUREZA"; //Ele tosse muito?
			palavrasBanco[3].nivel = 1;
			palavrasBanco[3].nome_audio_menino = "naturezamenino";
			palavrasBanco[3].nome_audio_menina = "naturezamenina";
			palavrasBanco[3].nome_audio_ditado = "naturezaditado";

			palavrasBanco[4].palavra = "REPRE _ A";
			palavrasBanco[4].letra_correta = "S";
			palavrasBanco[4].opcao1 = "Z";
			palavrasBanco[4].palavra_completa = "REPRESA"; //Assustar as pessoas é chato!
			palavrasBanco[4].nivel = 1;
			palavrasBanco[4].nome_audio_menino = "represamenino";
			palavrasBanco[4].nome_audio_menina = "represamenina";
			palavrasBanco[4].nome_audio_ditado = "represaditado";

			palavrasBanco[5].palavra = "FANTA _ IA";
			palavrasBanco[5].letra_correta = "S";
			palavrasBanco[5].opcao1 = "Z";
			palavrasBanco[5].palavra_completa = "FANTASIA"; //O pássaro voa bem alto.
			palavrasBanco[5].nivel = 1;
			palavrasBanco[5].nome_audio_menino = "fantasiamenino";
			palavrasBanco[5].nome_audio_menina = "fantasiamenina";
			palavrasBanco[5].nome_audio_ditado = "fantasiaditado";

			palavrasBanco[6].palavra = "TE _ OURO";
			palavrasBanco[6].letra_correta = "S";
			palavrasBanco[6].opcao1 = "Z";
			palavrasBanco[6].palavra_completa = "TESOURO"; //A professora entregou a tarefa aos alunos.
			palavrasBanco[6].nivel = 1;
			palavrasBanco[6].nome_audio_menino = "tesouromenino";
			palavrasBanco[6].nome_audio_menina = "tesouromenina";
			palavrasBanco[6].nome_audio_ditado = "tesouroditado";

			palavrasBanco[7].palavra = "RAPO _ A";
			palavrasBanco[7].letra_correta = "S";
			palavrasBanco[7].opcao1 = "Z";
			palavrasBanco[7].palavra_completa = "RAPOSA"; //Gosto de girassol.
			palavrasBanco[7].nivel = 1;
			palavrasBanco[7].nome_audio_menino = "raposamenino";
			palavrasBanco[7].nome_audio_menina = "raposamenina";
			palavrasBanco[7].nome_audio_ditado = "raposaditado";

			palavrasBanco[8].palavra = "PRE _ A";
			palavrasBanco[8].letra_correta = "S";
			palavrasBanco[8].opcao1 = "Z";
			palavrasBanco[8].palavra_completa = "PRESA"; //O osso do meu cachorro é grande!
			palavrasBanco[8].nivel = 1;
			palavrasBanco[8].nome_audio_menino = "presamenino";
			palavrasBanco[8].nome_audio_menina = "presamenina";
			palavrasBanco[8].nome_audio_ditado = "presaditado";

			palavrasBanco[9].palavra = "BE _ OUROS";
			palavrasBanco[9].letra_correta = "S";
			palavrasBanco[9].opcao1 = "Z";
			palavrasBanco[9].palavra_completa = "BESOUROS"; //O frango assado da minha mãe é delicioso!
			palavrasBanco[9].nivel = 1;
			palavrasBanco[9].nome_audio_menino = "besourosmenino";
			palavrasBanco[9].nome_audio_menina = "besourosmenina";
			palavrasBanco[9].nome_audio_ditado = "besourosditado";

			palavrasBanco[10].palavra = "CA _ A";
			palavrasBanco[10].letra_correta = "S";
			palavrasBanco[10].opcao1 = "Z";
			palavrasBanco[10].palavra_completa = "CASA"; //O frango assado da minha mãe é delicioso!
			palavrasBanco[10].nivel = 1;
			palavrasBanco[10].nome_audio_menino = "casamenino";
			palavrasBanco[10].nome_audio_menina = "casamenina";
			palavrasBanco[10].nome_audio_ditado = "casaditado";

			palavrasBanco[11].palavra = "A _ UL";
			palavrasBanco[11].letra_correta = "Z";
			palavrasBanco[11].opcao1 = "S";
			palavrasBanco[11].palavra_completa = "AZUL"; //O frango assado da minha mãe é delicioso!
			palavrasBanco[11].nivel = 1;
			palavrasBanco[11].nome_audio_menino = "azulmenino";
			palavrasBanco[11].nome_audio_menina = "azulmenina";
			palavrasBanco[11].nome_audio_ditado = "azulditado";

			palavrasBanco[12].palavra = "VA _ OS";
			palavrasBanco[12].letra_correta = "S";
			palavrasBanco[12].opcao1 = "Z";
			palavrasBanco[12].palavra_completa = "VASOS"; //O frango assado da minha mãe é delicioso!
			palavrasBanco[12].nivel = 1;
			palavrasBanco[12].nome_audio_menino = "vasosmenino";
			palavrasBanco[12].nome_audio_menina = "vasosmenina";
			palavrasBanco[12].nome_audio_ditado = "vasosditado";

			palavrasBanco[13].palavra = "FA _ ENDEIRO";
			palavrasBanco[13].letra_correta = "Z";
			palavrasBanco[13].opcao1 = "S";
			palavrasBanco[13].palavra_completa = "FAZENDEIRO"; //O frango assado da minha mãe é delicioso!
			palavrasBanco[13].nivel = 1;
			palavrasBanco[13].nome_audio_menino = "fazendeiromenino";
			palavrasBanco[13].nome_audio_menina = "fazendeiromenina";
			palavrasBanco[13].nome_audio_ditado = "fazendeiroditado";

			palavrasBanco[14].palavra = "CORAJO _ O";
			palavrasBanco[14].letra_correta = "S";
			palavrasBanco[14].opcao1 = "Z";
			palavrasBanco[14].palavra_completa = "CORAJOSO"; //O frango assado da minha mãe é delicioso!
			palavrasBanco[14].nivel = 1;
			palavrasBanco[14].nome_audio_menino = "corajosomenino";
			palavrasBanco[14].nome_audio_menina = "corajosomenina";
			palavrasBanco[14].nome_audio_ditado = "corajosoditado";

			palavrasBanco[15].palavra = "PERIGO _ A";
			palavrasBanco[15].letra_correta = "S";
			palavrasBanco[15].opcao1 = "Z";
			palavrasBanco[15].palavra_completa = "PERIGOSA"; //O frango assado da minha mãe é delicioso!
			palavrasBanco[15].nivel = 1;
			palavrasBanco[15].nome_audio_menino = "perigosamenino";
			palavrasBanco[15].nome_audio_menina = "perigosamenina";
			palavrasBanco[15].nome_audio_ditado = "perigosaditado";

			palavrasBanco[16].palavra = "RO _ AS";
			palavrasBanco[16].letra_correta = "S";
			palavrasBanco[16].opcao1 = "Z";
			palavrasBanco[16].palavra_completa = "ROSAS"; //O frango assado da minha mãe é delicioso!
			palavrasBanco[16].nivel = 1;
			palavrasBanco[16].nome_audio_menino = "rosasmenino";
			palavrasBanco[16].nome_audio_menina = "rosasmenina";
			palavrasBanco[16].nome_audio_ditado = "rosasditado";

			palavrasBanco[17].palavra = "TE _ OURA";
			palavrasBanco[17].letra_correta = "S";
			palavrasBanco[17].opcao1 = "Z";
			palavrasBanco[17].palavra_completa = "TESOURA"; //O frango assado da minha mãe é delicioso!
			palavrasBanco[17].nivel = 1;
			palavrasBanco[17].nome_audio_menino = "tesouramenino";
			palavrasBanco[17].nome_audio_menina = "tesouramenina";
			palavrasBanco[17].nome_audio_ditado = "tesouraditado";

			palavrasBanco[18].palavra = "GULO _ EIMAS";
			palavrasBanco[18].letra_correta = "S";
			palavrasBanco[18].opcao1 = "Z";
			palavrasBanco[18].palavra_completa = "GULOSEIMAS"; //O frango assado da minha mãe é delicioso!
			palavrasBanco[18].nivel = 1;
			palavrasBanco[18].nome_audio_menino = "guloseimasmenino";
			palavrasBanco[18].nome_audio_menina = "guloseimasmenina";
			palavrasBanco[18].nome_audio_ditado = "guloseimasditado";

			palavrasBanco[19].palavra = "CO _ IDO";
			palavrasBanco[19].letra_correta = "Z";
			palavrasBanco[19].opcao1 = "S";
			palavrasBanco[19].palavra_completa = "COZIDO"; //O frango assado da minha mãe é delicioso!
			palavrasBanco[19].nivel = 1;
			palavrasBanco[19].nome_audio_menino = "cozidomenino";
			palavrasBanco[19].nome_audio_menina = "cozidomenina";
			palavrasBanco[19].nome_audio_ditado = "cozidoditado";

			palavrasBanco[20].palavra = "CA _ AMENTO";
			palavrasBanco[20].letra_correta = "S";
			palavrasBanco[20].opcao1 = "Z";
			palavrasBanco[20].palavra_completa = "CASAMENTO"; //O frango assado da minha mãe é delicioso!
			palavrasBanco[20].nivel = 1;
			palavrasBanco[20].nome_audio_menino = "casamentomenino";
			palavrasBanco[20].nome_audio_menina = "casamentomenina";
			palavrasBanco[20].nome_audio_ditado = "casamentoditado";

			palavrasBanco[21].palavra = "PRE _ ENTES";
			palavrasBanco[21].letra_correta = "S";
			palavrasBanco[21].opcao1 = "Z";
			palavrasBanco[21].palavra_completa = "PRESENTES"; //O frango assado da minha mãe é delicioso!
			palavrasBanco[21].nivel = 1;
			palavrasBanco[21].nome_audio_menino = "presentesmenino";
			palavrasBanco[21].nome_audio_menina = "presentesmenina";
			palavrasBanco[21].nome_audio_ditado = "presentesditado";

			palavrasBanco[22].palavra = "RE _ AS";
			palavrasBanco[22].letra_correta = "Z";
			palavrasBanco[22].opcao1 = "S";
			palavrasBanco[22].palavra_completa = "REZAS"; //O frango assado da minha mãe é delicioso!
			palavrasBanco[22].nivel = 1;
			palavrasBanco[22].nome_audio_menino = "rezasmenino";
			palavrasBanco[22].nome_audio_menina = "rezasmenina";
			palavrasBanco[22].nome_audio_ditado = "rezasditado";

			palavrasBanco[23].palavra = "SURPRE _ AS";
			palavrasBanco[23].letra_correta = "S";
			palavrasBanco[23].opcao1 = "Z";
			palavrasBanco[23].palavra_completa = "SURPRESAS"; //O frango assado da minha mãe é delicioso!
			palavrasBanco[23].nivel = 1;
			palavrasBanco[23].nome_audio_menino = "surpresasmenino";
			palavrasBanco[23].nome_audio_menina = "surpresasmenina";
			palavrasBanco[23].nome_audio_ditado = "surpresasditado";

			palavrasBanco[24].palavra = "VI _ INHANÇA";
			palavrasBanco[24].letra_correta = "Z";
			palavrasBanco[24].opcao1 = "S";
			palavrasBanco[24].palavra_completa = "VIZINHANÇA"; //O frango assado da minha mãe é delicioso!
			palavrasBanco[24].nivel = 1;
			palavrasBanco[24].nome_audio_menino = "vizinhancamenino";
			palavrasBanco[24].nome_audio_menina = "vizinhancamenina";
			palavrasBanco[24].nome_audio_ditado = "vizinhancaditado";

			palavrasBanco[25].palavra = "VI _ ITAR";
			palavrasBanco[25].letra_correta = "S";
			palavrasBanco[25].opcao1 = "Z";
			palavrasBanco[25].palavra_completa = "VISITAR"; //O frango assado da minha mãe é delicioso!
			palavrasBanco[25].nivel = 1;
			palavrasBanco[25].nome_audio_menino = "visitarmenino";
			palavrasBanco[25].nome_audio_menina = "visitarmenina";
			palavrasBanco[25].nome_audio_ditado = "visitarditado";

			palavrasBanco[26].palavra = "BU _ INA";
			palavrasBanco[26].letra_correta = "Z";
			palavrasBanco[26].opcao1 = "S";
			palavrasBanco[26].palavra_completa = "BUZINA"; //O frango assado da minha mãe é delicioso!
			palavrasBanco[26].nivel = 1;
			palavrasBanco[26].nome_audio_menino = "buzinamenino";
			palavrasBanco[26].nome_audio_menina = "buzinamenina";
			palavrasBanco[26].nome_audio_ditado = "buzinaditado";

			palavrasBanco[27].palavra = "AVI _ A";
			palavrasBanco[27].letra_correta = "S";
			palavrasBanco[27].opcao1 = "Z";
			palavrasBanco[27].palavra_completa = "AVISA"; //O frango assado da minha mãe é delicioso!
			palavrasBanco[27].nivel = 1;
			palavrasBanco[27].nome_audio_menino = "avisamenino";
			palavrasBanco[27].nome_audio_menina = "avisamenina";
			palavrasBanco[27].nome_audio_ditado = "avisaditado";

			palavrasBanco[28].palavra = "BRA _ A";
			palavrasBanco[28].letra_correta = "S";
			palavrasBanco[28].opcao1 = "Z";
			palavrasBanco[28].palavra_completa = "BRASA"; //O frango assado da minha mãe é delicioso!
			palavrasBanco[28].nivel = 1;
			palavrasBanco[28].nome_audio_menino = "brasamenino";
			palavrasBanco[28].nome_audio_menina = "brasamenina";
			palavrasBanco[28].nome_audio_ditado = "brasaditado";

			palavrasBanco[29].palavra = "ME _ A";
			palavrasBanco[29].letra_correta = "S";
			palavrasBanco[29].opcao1 = "Z";
			palavrasBanco[29].palavra_completa = "MESA"; //O frango assado da minha mãe é delicioso!
			palavrasBanco[29].nivel = 1;
			palavrasBanco[29].nome_audio_menino = "mesamenino";
			palavrasBanco[29].nome_audio_menina = "mesamenina";
			palavrasBanco[29].nome_audio_ditado = "mesaditado";

			palavrasBanco[30].palavra = "MÚ _ ICA";
			palavrasBanco[30].letra_correta = "S";
			palavrasBanco[30].opcao1 = "Z";
			palavrasBanco[30].palavra_completa = "MÚSICA"; //O frango assado da minha mãe é delicioso!
			palavrasBanco[30].nivel = 1;
			palavrasBanco[30].nome_audio_menino = "musicamenino";
			palavrasBanco[30].nome_audio_menina = "musicamenina";
			palavrasBanco[30].nome_audio_ditado = "musicaditado";

			palavrasBanco[31].palavra = "AVI _ E";
			palavrasBanco[31].letra_correta = "S";
			palavrasBanco[31].opcao1 = "Z";
			palavrasBanco[31].palavra_completa = "AVISE"; //O frango assado da minha mãe é delicioso!
			palavrasBanco[31].nivel = 1;
			palavrasBanco[31].nome_audio_menino = "avisemenino";
			palavrasBanco[31].nome_audio_menina = "avisemenina";
			palavrasBanco[31].nome_audio_ditado = "aviseditado";

			//NÍVEL 2 - U e L

			// Palavra: Natal
			palavrasBanco[32].palavra = "NATA_";
			palavrasBanco[32].letra_correta = "L";
			palavrasBanco[32].opcao1 = "U";
			palavrasBanco[32].palavra_completa = "NATAL"; //Finalmente as férias chegaram! E junto com ela o Natal! Que legal!. 
			palavrasBanco[32].nivel = 2;
			palavrasBanco[32].nome_audio_menino = "natalmenino"; 
			palavrasBanco[32].nome_audio_menina = "natalmenina";
			palavrasBanco[32].nome_audio_ditado = "natalditado";

			// Palavra: céu
			palavrasBanco[33].palavra = "CÉ_";
			palavrasBanco[33].letra_correta = "U";
			palavrasBanco[33].opcao1 = "L";
			palavrasBanco[33].palavra_completa = "CÉU"; //O céu fica azul no verão e o sol brilha. 
			palavrasBanco[33].nivel = 2;
			palavrasBanco[33].nome_audio_menino = "céumenino"; 
			palavrasBanco[33].nome_audio_menina = "céumenina";
			palavrasBanco[33].nome_audio_ditado = "céuditado";

			// Palavra: especial
			palavrasBanco[34].palavra = "ESPECIA_";
			palavrasBanco[34].letra_correta = "L";
			palavrasBanco[34].opcao1 = "U";
			palavrasBanco[34].palavra_completa = "ESPECIAL"; //Tudo isso é muito especial!. 
			palavrasBanco[34].nivel = 2;
			palavrasBanco[34].nome_audio_menino = "especialmenino"; 
			palavrasBanco[34].nome_audio_menina = "especialmenina";
			palavrasBanco[34].nome_audio_ditado = "especialditado";

			// Palavra: varal
			palavrasBanco[35].palavra = "VARA_";
			palavrasBanco[35].letra_correta = "L";
			palavrasBanco[35].opcao1 = "U";
			palavrasBanco[35].palavra_completa = "VARAL"; //Os irmãos fotografam a praia todos os dias, revelam as fotos e as colocam em um varal. 
			palavrasBanco[35].nivel = 2;
			palavrasBanco[35].nome_audio_menino = "varalmenino"; 
			palavrasBanco[35].nome_audio_menina = "varalmenina";
			palavrasBanco[35].nome_audio_ditado = "varalditado";

			// Palavra: brasil
			palavrasBanco[36].palavra = "BRASI_";
			palavrasBanco[36].letra_correta = "L";
			palavrasBanco[36].opcao1 = "U";
			palavrasBanco[36].palavra_completa = "BRASIL"; //Marcos e Julinha moram no Brasil. 
			palavrasBanco[36].nivel = 2;
			palavrasBanco[36].nome_audio_menino = "brasilmenino"; 
			palavrasBanco[36].nome_audio_menina = "brasilmenina";
			palavrasBanco[36].nome_audio_ditado = "brasilditado";

			// Palavra: quintal
			palavrasBanco[37].palavra = "QUINTA_";
			palavrasBanco[37].letra_correta = "L";
			palavrasBanco[37].opcao1 = "U";
			palavrasBanco[37].palavra_completa = "QUINTAL"; //No verão, a praia vira um quintal sensacional para eles. 
			palavrasBanco[37].nivel = 2;
			palavrasBanco[37].nome_audio_menino = "quintalmenino"; 
			palavrasBanco[37].nome_audio_menina = "quintalmenina";
			palavrasBanco[37].nome_audio_ditado = "quintalditado";

			// Palavra: gentil
			palavrasBanco[38].palavra = "GENTI_";
			palavrasBanco[38].letra_correta = "L";
			palavrasBanco[38].opcao1 = "U";
			palavrasBanco[38].palavra_completa = "GENTIL"; //O vendedor de picolé é sempre gentil e dócil. 
			palavrasBanco[38].nivel = 2;
			palavrasBanco[38].nome_audio_menino = "gentilmenino"; 
			palavrasBanco[38].nome_audio_menina = "gentilmenina";
			palavrasBanco[38].nome_audio_ditado = "gentilditado";

			// Palavra: dócil
			palavrasBanco[39].palavra = "DÓCI_";
			palavrasBanco[39].letra_correta = "L";
			palavrasBanco[39].opcao1 = "U";
			palavrasBanco[39].palavra_completa = "DÓCIL"; //O vendedor de picolé é sempre gentil e dócil. 
			palavrasBanco[39].nivel = 2;
			palavrasBanco[39].nome_audio_menino = "dócilmenino"; 
			palavrasBanco[39].nome_audio_menina = "dócilmenina";
			palavrasBanco[39].nome_audio_ditado = "dócilditado";

			// Palavra: farol
			palavrasBanco[40].palavra = "FARO_";
			palavrasBanco[40].letra_correta = "L";
			palavrasBanco[40].opcao1 = "U";
			palavrasBanco[40].palavra_completa = "FAROL"; //O farol brilha à noite no mar. 
			palavrasBanco[40].nivel = 2;
			palavrasBanco[40].nome_audio_menino = "farolmenino"; 
			palavrasBanco[40].nome_audio_menina = "farolmenina";
			palavrasBanco[40].nome_audio_ditado = "farolditado";

			// Palavra: fácil
			palavrasBanco[41].palavra = "FÁCI_";
			palavrasBanco[41].letra_correta = "L";
			palavrasBanco[41].opcao1 = "U";
			palavrasBanco[41].palavra_completa = "FÁCIL"; //Mas a diversão principal é muito fácil. 
			palavrasBanco[41].nivel = 2;
			palavrasBanco[41].nome_audio_menino = "fácilmenino"; 
			palavrasBanco[41].nome_audio_menina = "fácilmenina";
			palavrasBanco[41].nome_audio_ditado = "fácilditado";

			// Palavra: túnel
			palavrasBanco[42].palavra = "TÚNE_";
			palavrasBanco[42].letra_correta = "L";
			palavrasBanco[42].opcao1 = "U";
			palavrasBanco[42].palavra_completa = "TÚNEL"; //Fazer túnel e castelo de areia. 
			palavrasBanco[42].nivel = 2;
			palavrasBanco[42].nome_audio_menino = "túnelmenino"; 
			palavrasBanco[42].nome_audio_menina = "túnelmenina";
			palavrasBanco[42].nome_audio_ditado = "túnelditado";

			// Palavra: sinal
			palavrasBanco[43].palavra = "SINA_";
			palavrasBanco[43].letra_correta = "L";
			palavrasBanco[43].opcao1 = "U";
			palavrasBanco[43].palavra_completa = "SINAL"; //Hoje não havia nuvens no céu, e céu sem nuvens é sinal de muito sol. 
			palavrasBanco[43].nivel = 2;
			palavrasBanco[43].nome_audio_menino = "sinalmenino"; 
			palavrasBanco[43].nome_audio_menina = "sinalmenina";
			palavrasBanco[43].nome_audio_ditado = "sinalditado";

			// Palavra: chapéu
			palavrasBanco[44].palavra = "CHAPÉ_";
			palavrasBanco[44].letra_correta = "U";
			palavrasBanco[44].opcao1 = "L";
			palavrasBanco[44].palavra_completa = "CHAPÉU"; //Usar chapéu e roupas leves é uma recomendação. 
			palavrasBanco[44].nivel = 2;
			palavrasBanco[44].nome_audio_menino = "chapéumenino"; 
			palavrasBanco[44].nome_audio_menina = "chapéumenina";
			palavrasBanco[44].nome_audio_ditado = "chapéuditado";

			// Palavra: roupas
			palavrasBanco[45].palavra = "RO_PAS";
			palavrasBanco[45].letra_correta = "U";
			palavrasBanco[45].opcao1 = "L";
			palavrasBanco[45].palavra_completa = "ROUPAS"; //Usar chapéu e roupas leves é uma recomendação. 
			palavrasBanco[45].nivel = 2;
			palavrasBanco[45].nome_audio_menino = "roupasmenino"; 
			palavrasBanco[45].nome_audio_menina = "roupasmenina";
			palavrasBanco[45].nome_audio_ditado = "roupasditado";

			// Palavra: seu
			palavrasBanco[46].palavra = "SE_";
			palavrasBanco[46].letra_correta = "U";
			palavrasBanco[46].opcao1 = "L";
			palavrasBanco[46].palavra_completa = "SEU"; //Ela tomou tanto sol, que seu corpo ficou febril. 
			palavrasBanco[46].nivel = 2;
			palavrasBanco[46].nome_audio_menino = "seumenino"; 
			palavrasBanco[46].nome_audio_menina = "seumenina";
			palavrasBanco[46].nome_audio_ditado = "seuditado";

			// Palavra: animal
			palavrasBanco[47].palavra = "ANIMA_";
			palavrasBanco[47].letra_correta = "L";
			palavrasBanco[47].opcao1 = "U";
			palavrasBanco[47].palavra_completa = "ANIMAL"; //É preciso oferecer água até pro seu animalzinho 
			palavrasBanco[47].nivel = 2;
			palavrasBanco[47].nome_audio_menino = "animalmenino"; 
			palavrasBanco[47].nome_audio_menina = "animalmenina";
			palavrasBanco[47].nome_audio_ditado = "animalditado";

			// Palavra: sal
			palavrasBanco[48].palavra = "SA_";
			palavrasBanco[48].letra_correta = "L";
			palavrasBanco[48].opcao1 = "U";
			palavrasBanco[48].palavra_completa = "SAL"; //Não pode ser água do mar porque contém muito sal. 
			palavrasBanco[48].nivel = 2;
			palavrasBanco[48].nome_audio_menino = "salmenino"; 
			palavrasBanco[48].nome_audio_menina = "salmenina";
			palavrasBanco[48].nome_audio_ditado = "salditado";

			// Palavra: calça
			palavrasBanco[49].palavra = "CA_ÇA";
			palavrasBanco[49].letra_correta = "L";
			palavrasBanco[49].opcao1 = "U";
			palavrasBanco[49].palavra_completa = "CALÇA"; //Nesse dia, Marcos nadou de calça comprida. 
			palavrasBanco[49].nivel = 2;
			palavrasBanco[49].nome_audio_menino = "calçamenino"; 
			palavrasBanco[49].nome_audio_menina = "calçamenina";
			palavrasBanco[49].nome_audio_ditado = "calçaditado";

			// Palavra: difícil
			palavrasBanco[50].palavra = "DIFÍCI_";
			palavrasBanco[50].letra_correta = "L";
			palavrasBanco[50].opcao1 = "U";
			palavrasBanco[50].palavra_completa = "DIFÍCIL"; //Foi muito difícil pular as ondas altas. 
			palavrasBanco[50].nivel = 2;
			palavrasBanco[50].nome_audio_menino = "difícilmenino"; 
			palavrasBanco[50].nome_audio_menina = "difícilmenina";
			palavrasBanco[50].nome_audio_ditado = "difícilditado";

			// Palavra: altas
			palavrasBanco[51].palavra = "A_TAS";
			palavrasBanco[51].letra_correta = "L";
			palavrasBanco[51].opcao1 = "U";
			palavrasBanco[51].palavra_completa = "ALTAS"; //Foi muito difícil pular as ondas altas. 
			palavrasBanco[51].nivel = 2;
			palavrasBanco[51].nome_audio_menino = "altasmenino"; 
			palavrasBanco[51].nome_audio_menina = "altasmenina";
			palavrasBanco[51].nome_audio_ditado = "altasditado";

			// Palavra: anzol
			palavrasBanco[52].palavra = "ANZO_";
			palavrasBanco[52].letra_correta = "L";
			palavrasBanco[52].opcao1 = "U";
			palavrasBanco[52].palavra_completa = "DIFÍCIL"; //Ele foi pescar e levou: anzol, iscas e um barquinho que não era de papel. 
			palavrasBanco[52].nivel = 2;
			palavrasBanco[52].nome_audio_menino = "anzolmenino"; 
			palavrasBanco[52].nome_audio_menina = "anzolmenina";
			palavrasBanco[52].nome_audio_ditado = "anzolditado";

			// Palavra: papel
			palavrasBanco[53].palavra = "PAPE_";
			palavrasBanco[53].letra_correta = "L";
			palavrasBanco[53].opcao1 = "U";
			palavrasBanco[53].palavra_completa = "DIFÍCIL"; //Ele foi pescar e levou: anzol, iscas e um barquinho que não era de papel. 
			palavrasBanco[53].nivel = 2;
			palavrasBanco[53].nome_audio_menino = "papelmenino"; 
			palavrasBanco[53].nome_audio_menina = "papelmenina";
			palavrasBanco[53].nome_audio_ditado = "papelditado";

			// Palavra: pneu
			palavrasBanco[54].palavra = "PNE_";
			palavrasBanco[54].letra_correta = "U";
			palavrasBanco[54].opcao1 = "L";
			palavrasBanco[54].palavra_completa = "PNEU"; //Marcos imaginou que o pneu era seu grande barco que quase virou nas ondas e bateu em um grande coral. 
			palavrasBanco[54].nivel = 2;
			palavrasBanco[54].nome_audio_menino = "pneumenino"; 
			palavrasBanco[54].nome_audio_menina = "pneumenina";
			palavrasBanco[54].nome_audio_ditado = "pneuditado";

			// Palavra: coral
			palavrasBanco[55].palavra = "CORA_";
			palavrasBanco[55].letra_correta = "L";
			palavrasBanco[55].opcao1 = "U";
			palavrasBanco[55].palavra_completa = "CORAL"; //Marcos imaginou que o pneu era seu grande barco que quase virou nas ondas e bateu em um grande coral. 
			palavrasBanco[55].nivel = 2;
			palavrasBanco[55].nome_audio_menino = "coralmenino"; 
			palavrasBanco[55].nome_audio_menina = "coralmenina";
			palavrasBanco[55].nome_audio_ditado = "coralditado";

			// Palavra: pau
			palavrasBanco[56].palavra = "PA_";
			palavrasBanco[56].letra_correta = "U";
			palavrasBanco[56].opcao1 = "L";
			palavrasBanco[56].palavra_completa = "PAU"; //Estava brincando de "pirata de perna de pau, de olho de vidro e da cara de mau". 
			palavrasBanco[56].nivel = 2;
			palavrasBanco[56].nome_audio_menino = "paumenino"; 
			palavrasBanco[56].nome_audio_menina = "paumenina";
			palavrasBanco[56].nome_audio_ditado = "pauditado";

			// Palavra: mau
			palavrasBanco[57].palavra = "MA_";
			palavrasBanco[57].letra_correta = "U";
			palavrasBanco[57].opcao1 = "L";
			palavrasBanco[57].palavra_completa = "MAU"; //Marcos imaginou que o pneu era seu grande barco que quase virou nas ondas e bateu em um grande coral. 
			palavrasBanco[57].nivel = 2;
			palavrasBanco[57].nome_audio_menino = "maumenino"; 
			palavrasBanco[57].nome_audio_menina = "maumenina";
			palavrasBanco[57].nome_audio_ditado = "mauditado";

			// Palavra: almanaque
			palavrasBanco[58].palavra = "A_MANAQUE";
			palavrasBanco[58].letra_correta = "L";
			palavrasBanco[58].opcao1 = "U";
			palavrasBanco[58].palavra_completa = "MAU"; //Marcos e Julinha possuem um almanaque que explica tudo sobre o fundo do mar. 
			palavrasBanco[58].nivel = 2;
			palavrasBanco[58].nome_audio_menino = "almanaquemenino"; 
			palavrasBanco[58].nome_audio_menina = "almanaquemenina";
			palavrasBanco[58].nome_audio_ditado = "almanaqueditado";

			// Palavra: degraus
			palavrasBanco[59].palavra = "DEGRA_S";
			palavrasBanco[59].letra_correta = "U";
			palavrasBanco[59].opcao1 = "L";
			palavrasBanco[59].palavra_completa = "DEGRAUS"; //Quando anoitecer, os irmãos costumam subir os degraus para o andar superior. 
			palavrasBanco[59].nivel = 2;
			palavrasBanco[59].nome_audio_menino = "degrausmenino"; 
			palavrasBanco[59].nome_audio_menina = "degrausmenina";
			palavrasBanco[59].nome_audio_ditado = "degrausditado";

			// Palavra: pouco
			palavrasBanco[60].palavra = "PO_CO";
			palavrasBanco[60].letra_correta = "U";
			palavrasBanco[60].opcao1 = "L";
			palavrasBanco[60].palavra_completa = "POUCO"; //pena que tudo que é bom, dura pouco... 
			palavrasBanco[60].nivel = 2;
			palavrasBanco[60].nome_audio_menino = "poucomenino"; 
			palavrasBanco[60].nome_audio_menina = "poucomenina";
			palavrasBanco[60].nome_audio_ditado = "poucoditado";

			// Palavra: mingau
			palavrasBanco[61].palavra = "MINGA_";
			palavrasBanco[61].letra_correta = "U";
			palavrasBanco[61].opcao1 = "L";
			palavrasBanco[61].palavra_completa = "MINGAU"; //Agora está na hora de dormir, tomar mingau adoçadinho com mel 
			palavrasBanco[61].nivel = 2;
			palavrasBanco[61].nome_audio_menino = "mingaumenino"; 
			palavrasBanco[61].nome_audio_menina = "mingaumenina";
			palavrasBanco[61].nome_audio_ditado = "mingauditado";

			// Palavra: mel
			palavrasBanco[62].palavra = "ME_";
			palavrasBanco[62].letra_correta = "L";
			palavrasBanco[62].opcao1 = "U";
			palavrasBanco[62].palavra_completa = "MEL"; //Agora está na hora de dormir, tomar mingau adoçadinho com mel 
			palavrasBanco[62].nivel = 2;
			palavrasBanco[62].nome_audio_menino = "melmenino"; 
			palavrasBanco[62].nome_audio_menina = "melmenina";
			palavrasBanco[62].nome_audio_ditado = "melditado";

			// Palavra: final
			palavrasBanco[63].palavra = "FINA_";
			palavrasBanco[63].letra_correta = "L";
			palavrasBanco[63].opcao1 = "U";
			palavrasBanco[63].palavra_completa = "FINAL"; //acordar bem cedo para aproveitar o dia antes que as férias cheguem ao final
			palavrasBanco[63].nivel = 2;
			palavrasBanco[63].nome_audio_menino = "finalmenino"; 
			palavrasBanco[63].nome_audio_menina = "finalmenina";
			palavrasBanco[63].nome_audio_ditado = "finalditado";

			for (int i = 0; i < total_palavras_geral; ++i)
			{
				_connection.Insert (palavrasBanco[i]);

				reference.Child ("palavras").Child (palavrasBanco[i].Id.ToString() + "/palavra").SetValueAsync (palavrasBanco[i].palavra);
				reference.Child ("palavras").Child (palavrasBanco[i].Id.ToString() + "/letra_correta").SetValueAsync (palavrasBanco[i].letra_correta);
				reference.Child ("palavras").Child (palavrasBanco[i].Id.ToString() + "/opcao1").SetValueAsync (palavrasBanco[i].opcao1);
				reference.Child ("palavras").Child (palavrasBanco[i].Id.ToString() + "/palavra_completa").SetValueAsync (palavrasBanco[i].palavra_completa);
				reference.Child ("palavras").Child (palavrasBanco[i].Id.ToString() + "/nivel").SetValueAsync (palavrasBanco[i].nivel);
				reference.Child ("palavras").Child (palavrasBanco[i].Id.ToString() + "/nome_audio_menino").SetValueAsync (palavrasBanco[i].nome_audio_menino);
				reference.Child ("palavras").Child (palavrasBanco[i].Id.ToString() + "/nome_audio_menina").SetValueAsync (palavrasBanco[i].nome_audio_menina);
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

	//Função para selecionar o nível correspondente para que sejam carregadas apenas as palavras referentes àquele nível
	public void seleciona_nivel(int nivel){
		dadosJogo.Instance.currentUser.Nivel = nivel;

		if (nivel == 0) {
			SceneManager.LoadScene ("telaLivroMundo1");
		} else if (nivel == 1) {
			SceneManager.LoadScene ("telaLivroMundo2");
		} else {
			SceneManager.LoadScene ("telaLivroMundo3");
		}
	}
	
	//para que serve essa função? nenhuma está comentada
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


}


