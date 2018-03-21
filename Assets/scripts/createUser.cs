using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.IO;
using SQLite4Unity3d;
using UnityEngine.SceneManagement;

public class createUser : MonoBehaviour {


    public InputField nome; // Campo onde o usuário insere seu nome!

    public GameObject somOn; // Opção ativar e desativar som!
    public GameObject somOff;
	public SQLiteConnection _connection;

	public void Start(){
		// Conexão com o banco de dados grapphia!
		var filepath = string.Format("{0}/{1}", Application.persistentDataPath, "grapphia");

		if (!File.Exists(filepath))
		{

			var loadDb = new WWW("jar:file://" + Application.dataPath + "!/assets/" + "grapphia");  

			while (!loadDb.isDone) { }  

			File.WriteAllBytes(filepath, loadDb.bytes);
		}

		var dbPath = filepath;

		_connection = new SQLiteConnection(dbPath, SQLiteOpenFlags.ReadWrite | SQLiteOpenFlags.Create);

		Debug.Log("Final PATH: " + dbPath);
		//Criando tabela usuário e tabela opção!
		_connection.CreateTable<user>();
		_connection.CreateTable<palavraOpcao>();
		_connection.CreateTable<palavraAcertoUser>();
	}

    // Função para criar usuário!
    public void create()
    {
        if (nome.text == "") return;

		var users = _connection.Table<user>().Where(x => x.Name == nome.text);

		if (users.Count () >= 1) {;
			SceneManager.LoadScene ("telaUsuarioCadastrado");
			return;
		};

		DataService data = new DataService(_connection);
        data.CreateUser(nome.text, 0,0,0);
	
		users = null;

        users = data._connection.Table<user>().Where(x => x.Name == nome.text);
   
        // Pega o usuário que ta sendo criado e seta como usuário que esta jogando!
        foreach (var user in users)
        {

            dadosJogo.Instance.currentUser = new user
            {
                Id = user.Id,
                Name = user.Name,
                Score = user.Score,
                Nivel = user.Nivel,
				scoreDitado = user.scoreDitado,
            };

        }

		SceneManager.LoadScene ("telaEstante");

    }


    public void click_Sound()
    {
        // Função que bloqueia e ativa o som!
        var compaudio = somOn.GetComponent<AudioSource>();

        if (compaudio.isPlaying)
        {
            somOn.SetActive(false);
            somOn.GetComponent<AudioSource>().Pause();
            somOff.SetActive(true);
        }
        else
        {
            somOn.SetActive(true);
            somOn.GetComponent<AudioSource>().UnPause();
            somOff.SetActive(false);
        }

    }


}
