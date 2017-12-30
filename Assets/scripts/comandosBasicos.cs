using UnityEngine;
using System.Collections;
using SQLite4Unity3d;
using UnityEngine.SceneManagement;

// Classe Singleton onde armazena as palavras!
public class bancoPalavras
{
    // Matriz das palavras, cada linha da matriz é um nível e cada coluna é uma palavra!

    private static bancoPalavras instance;

    public palavraOpcao[][] palavras;

    public palavraAcertoUser[] palavrasAcerto;

	public int qtd_Words; //quantidade de palavras recuperadas no BD de um determinado nível (nível atual do jogador)

	public int qtd_WordsPresented; //quantidade de palavras recuperadas no BD que determinado usuário já jogou

	public int numWordsGame = 15; //número de palavras que serão apresentadas para o usuário em cada nível

	public int numWordsDitado = 5; //número de palavras que serão apresentadas para o usuário no ditado

    public DataService dataservice;

    public int acertos, erros;//armazena acertos e erros totais por usuário

    public int total_palavras; //total de palavras no BD em todos os níveis

	public int maxLevel = 1; //quantidade de níveis (mundos) implementados



    // Construtor da classe!
    private bancoPalavras()
    {

        dataservice = new DataService("grapphia");
        var words2 = dataservice._connection.Table<palavraOpcao>();

        total_palavras = words2.Count();

		Debug.Log ("Total de palavras no BD: " + total_palavras);

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
		var resul = dataservice._connection.Table<palavraOpcao>().Where(x => x.nivel == aux);

		//Busca palavras do nível AUX e usuário específico
        var resul2 = dataservice._connection.Table<palavraAcertoUser>().Where(x => ((x.nivelPalavra == aux) && (x.idUser == dadosJogo.Instance.currentUser.Id)));

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
                nome_audio_menina = p.nome_audio_menina
            };

            ++qtd_Words;

        }

		Debug.Log("Foram recuperadas " + resul.Count() + " palavras do BD referentes ao nível " + level);

        acertos = resul2.Count();

        if (resul2.Count() > 0)
        {
            palavrasAcerto = new palavraAcertoUser[(32*aux)];

            for (int j = 0; j < (32 * aux); ++j) palavrasAcerto[j] = new palavraAcertoUser();
            foreach (var p in resul2)
            {
                palavrasAcerto[(p.idPalavra-1)] = new palavraAcertoUser
                {
                    Id = p.Id,
                    idPalavra = p.idPalavra,
                    idUser = p.idUser,
                    acerto = p.acerto,
                    nivelPalavra = p.nivelPalavra,
					acertoDitado = p.acertoDitado
                };
				qtd_WordsPresented++;
            }
        }
        else
        {
            palavrasAcerto = new palavraAcertoUser[(32*aux)];
            for (int j = 0; j < (32*aux); ++j)
            {
                palavrasAcerto[j] = new palavraAcertoUser();
            }
        }

    }

    public void salvar_palavrasAcertoUser()
    {
        for(int i=0; i<palavrasAcerto.Length; ++i)
        {
            if (palavrasAcerto[i].idPalavra > 0 && palavrasAcerto[i].Id > 0)
            {
               dataservice._connection.Update(palavrasAcerto[i]);
            }
            else if (palavrasAcerto[i].idPalavra > 0) dataservice._connection.Insert(palavrasAcerto[i]);
        }

    }


    public void salvar_palavras_no_banco()      //salva palavras no banco de dados  essa função é chamada apenas quando for criar o banco de dados

    {
		int numWordsLevel1 = 32;

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

		/** NIVEIS AINDA DESATIVADOS **/

		// NÍVEL 2 - Palavras com U e L!
        /*palavras[1][0].palavra = "PAPE _";
        palavras[1][0].letra_correta = "L";
        palavras[1][0].opcao1 = "U";
        palavras[1][0].palavra_completa = "PAPEL"; //O papel molhou?
        palavras[1][0].nivel = 2;
        palavras[1][0].nome_audio_menino = "";
        palavras[1][0].nome_audio_menina = "";

          palavras[1][1].palavra = "DESAGRADÁVE _";
          palavras[1][1].letra_correta = "L";
          palavras[1][1].opcao1 = "U";
          palavras[1][1].palavra_completa = "DESAGRADÁVEL"; //Isso é desagradável!!!
          palavras[1][1].nivel = 2;
          palavras[1][1].nome_audio_menino = "";
          palavras[1][1].nome_audio_menina = "";

          palavras[1][2].palavra = "LEGA _";
          palavras[1][2].letra_correta = "L";
          palavras[1][2].opcao1 = "U";
          palavras[1][2].palavra_completa = "LEGAL"; //Isso é legal, adoro o carnaval!!!
          palavras[1][2].nivel = 2;
          palavras[1][2].nome_audio_menino = "";
          palavras[1][2].nome_audio_menina = "";

          palavras[1][3].palavra = "CARACO _";
          palavras[1][3].letra_correta = "L";
          palavras[1][3].opcao1 = "U";
          palavras[1][3].palavra_completa = "CARACOL"; //O caracol é nojento! Eca!!!
          palavras[1][3].nivel = 2;
          palavras[1][3].nome_audio_menino = "";
          palavras[1][3].nome_audio_menina = "";

          palavras[1][4].palavra = "ANE _";
          palavras[1][4].letra_correta = "L";
          palavras[1][4].opcao1 = "U";
          palavras[1][4].palavra_completa = "ANEL"; //Nossa! Que anel lindo!!!
          palavras[1][4].nivel = 2;
          palavras[1][4].nome_audio_menino = "";
          palavras[1][4].nome_audio_menina = "";

          palavras[1][5].palavra = "ÁLCOO _";
          palavras[1][5].letra_correta = "L";
          palavras[1][5].opcao1 = "U";
          palavras[1][5].palavra_completa = "ÁLCOOL"; //Que cheiro do álcool ruim!
          palavras[1][5].nivel = 2;
          palavras[1][5].nome_audio_menino = "";
          palavras[1][5].nome_audio_menina = "";

          palavras[1][6].palavra = "GO_";
          palavras[1][6].letra_correta = "L";
          palavras[1][6].opcao1 = "U";
          palavras[1][6].palavra_completa = "GOL"; //E é gol do Atlético Mineiro!
          palavras[1][6].nivel = 2;
          palavras[1][6].nome_audio_menino = "";
          palavras[1][6].nome_audio_menina = "";

          palavras[1][7].palavra = "SO _";
          palavras[1][7].letra_correta = "L";
          palavras[1][7].opcao1 = "U";
          palavras[1][7].palavra_completa = "SOL"; //A Terra gira em torno do Sol.
          palavras[1][7].nivel = 2;
          palavras[1][7].nome_audio_menino = "";
          palavras[1][7].nome_audio_menina = "";

          palavras[1][8].palavra = "CANA _";
          palavras[1][8].letra_correta = "L";
          palavras[1][8].opcao1 = "U";
          palavras[1][8].palavra_completa = "CANAL"; //Mudei de canal para ver o filme.
          palavras[1][8].nivel = 2;
          palavras[1][8].nome_audio_menino = "";
          palavras[1][8].nome_audio_menina = "";

          palavras[1][9].palavra = "PA _OS";
          palavras[1][9].letra_correta = "SS";
          palavras[1][9].opcao1 = "Ç";
          palavras[1][9].palavra_completa = "PASSOS"; //Os meus passos são grandes!    10 %
          palavras[1][9].nivel = 2;
          palavras[1][9].nome_audio_menino = "";
          palavras[1][9].nome_audio_menina = "";

        // NÍVEL 3 - G e J!
        palavras[2][0].palavra = " _ IRAFA";
          palavras[2][0].letra_correta = "G";
          palavras[2][0].opcao1 = "J";
          palavras[2][0].palavra_completa = "GIRAFA"; //Eu vi uma girafa.
          palavras[2][0].nivel = 3;
          palavras[2][0].nome_audio_menino = "";
          palavras[2][0].nome_audio_menina = "";

        palavras[2][1].palavra = "HO _ E";
          palavras[2][1].letra_correta = "J";
          palavras[2][1].opcao1 = "G";
          palavras[2][1].palavra_completa = "HOJE"; //Hoje está super gelado. Brrrrrr!!!
          palavras[2][1].nivel = 3;
          palavras[2][1].nome_audio_menino = "";
          palavras[2][1].nome_audio_menina = "";

        palavras[2][2].palavra = "MA _ ESTADE";
          palavras[2][2].letra_correta = "J";
          palavras[2][2].opcao1 = "G";
          palavras[2][2].palavra_completa = "MAJESTADE"; //Vossa majestade, tu mandas em tudo? Podes ordenar um pôr do Sol?
          palavras[2][2].nivel = 3;
          palavras[2][2].nome_audio_menino = "";
          palavras[2][2].nome_audio_menina = "";

        palavras[2][3].palavra = "MÁ _ ICA";
          palavras[2][3].letra_correta = "G";
          palavras[2][3].opcao1 = "J";
          palavras[2][3].palavra_completa = "MÁGICA"; //A festa de ontem foi mágica.
          palavras[2][3].nivel = 3;
          palavras[2][3].nome_audio_menino = "";
          palavras[2][3].nome_audio_menina = "";

        palavras[2][4].palavra = "ZOOLÓ _ ICO";
          palavras[2][4].letra_correta = "G";
          palavras[2][4].opcao1 = "J";
          palavras[2][4].palavra_completa = "ZOOLÓGICO"; //O zoológico é maneiro!
          palavras[2][4].nivel = 3;
          palavras[2][4].nome_audio_menino = "";
          palavras[2][4].nome_audio_menina = "";

        palavras[2][5].palavra = "PÁ _ INA";
          palavras[2][5].letra_correta = "G";
          palavras[2][5].opcao1 = "J";
          palavras[2][5].palavra_completa = "PÁGINA"; //Essa página está rasgada.
          palavras[2][5].nivel = 3;
          palavras[2][5].nome_audio_menino = "";
          palavras[2][5].nome_audio_menina = "";

        palavras[2][6].palavra = "_ ENTE";
          palavras[2][6].letra_correta = "G";
          palavras[2][6].opcao1 = "J";
          palavras[2][6].palavra_completa = "GENTE"; //Ei, gente! Vocês não vão dançar? Isso é um baile.
          palavras[2][6].nivel = 3;
          palavras[2][6].nome_audio_menino = "";
          palavras[2][6].nome_audio_menina = "";

        palavras[2][7].palavra = " _ ILÓ";
          palavras[2][7].letra_correta = "J";
          palavras[2][7].opcao1 = "G";
          palavras[2][7].palavra_completa = "JILÓ"; //Eu comi jiló.
          palavras[2][7].nivel = 3;
          palavras[2][7].nome_audio_menino = "";
          palavras[2][7].nome_audio_menina = "";

        palavras[2][8].palavra = "PO _ O";
          palavras[2][8].letra_correta = "Ç";
          palavras[2][8].opcao1 = "SS";
          palavras[2][8].palavra_completa = "POÇO"; //Vamos procurar um poço? 10%
          palavras[2][8].nivel = 3;
          palavras[2][8].nome_audio_menino = "";
          palavras[2][8].nome_audio_menina = "";

        palavras[2][9].palavra = "SO _";
          palavras[2][9].letra_correta = "L";
          palavras[2][9].opcao1 = "U";
          palavras[2][9].palavra_completa = "SOL"; //A Terra gira em torno do Sol. 10%
          palavras[2][9].nivel = 3;
          palavras[2][9].nome_audio_menino = "";
          palavras[2][9].nome_audio_menina = "";

        // NÍVEL 4 - Palavras com S e Z!
        palavras[3][0].palavra = "TRE _ E"; // Palavra a ser completada!
        palavras[3][0].letra_correta = "Z"; // Letra correta!
        palavras[3][0].opcao1 = "S"; // Segunda opção!
        palavras[3][0].palavra_completa = "TREZE"; // Palavra completa! Eu tenho treze anos.
        palavras[3][0].nivel = 4; // Nível da palavra!
        palavras[3][0].nome_audio_menino = "";
        palavras[3][0].nome_audio_menina = "";


        palavras[3][1].palavra = "BLU _ A";
        palavras[3][1].letra_correta = "S";
        palavras[3][1].opcao1 = "Z";
        palavras[3][1].palavra_completa = "BLUSA"; // Essa blusa é sua?
        palavras[3][1].nivel = 4;
        palavras[3][1].nome_audio_menino = "";
        palavras[3][1].nome_audio_menina = "";

        palavras[3][2].palavra = "DO _ E";
        palavras[3][2].letra_correta = "Z";
        palavras[3][2].opcao1 = "S";
        palavras[3][2].palavra_completa = "DOZE"; // Você vai fazer doze anos amanhã?
        palavras[3][2].nivel = 4;
        palavras[3][2].nome_audio_menino = "";
        palavras[3][2].nome_audio_menina = "";

        palavras[3][3].palavra = "DU _ ENTOS";
        palavras[3][3].letra_correta = "Z";
        palavras[3][3].opcao1 = "S";
        palavras[3][3].palavra_completa = "DUZENTOS"; //Eu quero duzentos balões para a festa.
        palavras[3][3].nivel = 4;
        palavras[3][3].nome_audio_menino = "";
        palavras[3][3].nome_audio_menina = "";

        palavras[3][4].palavra = "A _ A";
        palavras[3][4].letra_correta = "S";
        palavras[3][4].opcao1 = "Z";
        palavras[3][4].palavra_completa = "ASA"; //A asa do passarinho quebrou!
        palavras[3][4].nivel = 4;
        palavras[3][4].nome_audio_menino = "";
        palavras[3][4].nome_audio_menina = "";

        palavras[3][5].palavra = "RO _ A";
        palavras[3][5].letra_correta = "S";
        palavras[3][5].opcao1 = "Z";
        palavras[3][5].palavra_completa = "ROSA"; //Vamos colocar a rosa no lugar dela.
        palavras[3][5].nivel = 4;
        palavras[3][5].nome_audio_menino = "";
        palavras[3][5].nome_audio_menina = "";

        palavras[3][6].palavra = "LOU _ A";
        palavras[3][6].letra_correta = "S";
        palavras[3][6].opcao1 = "Z";
        palavras[3][6].palavra_completa = "LOUSA"; //A lousa é branca?
        palavras[3][6].nivel = 4;
        palavras[3][6].nome_audio_menino = "";
        palavras[3][6].nome_audio_menina = "";

        palavras[3][7].palavra = "A _ OPRAR";
        palavras[3][7].letra_correta = "SS";
        palavras[3][7].opcao1 = "Ç";
        palavras[3][7].palavra_completa = "ASSOPRAR"; //Eu vou assoprar a sua casinha de palha! 10%
        palavras[3][7].nivel = 4;
        palavras[3][7].nome_audio_menino = "";
        palavras[3][7].nome_audio_menina = "";

        palavras[3][8].palavra = "ÁLCOO _";
        palavras[3][8].letra_correta = "L";
        palavras[3][8].opcao1 = "U";
        palavras[3][8].palavra_completa = "ÁLCOOL"; //Que cheiro do álcool ruim! 10%
        palavras[3][8].nivel = 4;
        palavras[3][8].nome_audio_menino = "";
        palavras[3][8].nome_audio_menina = "";

        palavras[3][9].palavra = "PÁ _ INA";
        palavras[3][9].letra_correta = "G";
        palavras[3][9].opcao1 = "J";
        palavras[3][9].palavra_completa = "PÁGINA"; //Essa página está rasgada. 10%
        palavras[3][9].nivel = 4;
        palavras[3][9].nome_audio_menino = "";
        palavras[3][9].nome_audio_menina = "";
*/

		for (int i = 0; i < maxLevel; ++i)
          {

			for (int j = 0; j < numWordsLevel1; ++j) dataservice._connection.Insert(palavras[i][j]);


          }

    }


}


public class comandosBasicos : MonoBehaviour {


    // Carregar cena!
    public void loadScene(string nameScene)
    {

        //Application.LoadLevel(nameScene);
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
        Application.LoadLevel(nameScene);


    }

    // Fechar jogo!
    public void exit()
    {
        dadosJogo.Instance.salvar_dados();
        Application.Quit();

    }
    public void exitInicial()
    {
        
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

	public int acertoDitado { get; set; }


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
        bancoPalavras.Instance.dataservice._connection.Update(currentUser);
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


