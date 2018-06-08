using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System;
using SQLite4Unity3d;
using UnityEngine.SceneManagement;
using Firebase;
using Firebase.Database;
using Firebase.Unity.Editor;
using Firebase.Auth;


public class DataService       // Classe de serviço do banco de dados!
{

    public SQLiteConnection _connection; // Criando conexão com SQLite!

	public DataService(SQLiteConnection connection){
		_connection = connection;
	}
		

    // Inicializando classe DataService!
    public void EstabeleceConexao(string dbName)
    {

        var filepath = string.Format("{0}/{1}", Application.persistentDataPath, dbName);



        if (!File.Exists(filepath))
        {

            var loadDb = new WWW("jar:file://" + Application.dataPath + "!/assets/" + dbName);  

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

	//Tabela alterada por Magno
    // Inserindo usuário no banco de dados!
	public user CreateUser(string newUser, int score, int erros,int nivel, int scoreDitado, int erroDitado, string keyFireBase)
    {
		FirebaseApp.DefaultInstance.SetEditorDatabaseUrl("https://grapphia.firebaseio.com/");
		DatabaseReference reference = FirebaseDatabase.DefaultInstance.RootReference;
		//Firebase.Auth.FirebaseAuth auth = Firebase.Auth.FirebaseAuth.DefaultInstance;
		//Firebase.Auth.FirebaseUser user_fb = auth.CurrentUser;

        var p = new user
        {
            Name = newUser,
            Score = score,
			Erros = erros,
            Nivel = nivel,
			scoreDitado = scoreDitado,
			erroDitado = erroDitado,
			key = keyFireBase,
        };
		//string uid = user_fb.UserId; //UID gerado pelo FireBase.Auth
		//string path = keyFireBase + "/" + uid + "/" + p.Name;

		string path = keyFireBase + "/" + p.Name;

		//reference.Child("users").Child(path + "/Nome").SetValueAsync(p.Name).IsCompleted += g();

		reference.Child("users").Child(path + "/Nome").SetValueAsync(p.Name);
		reference.Child("users").Child(path + "/Score").SetValueAsync(p.Score);
		reference.Child("users").Child(path + "/Nivel").SetValueAsync(p.Nivel);
		reference.Child("users").Child(path + "/Score Ditado").SetValueAsync(p.scoreDitado);
		reference.Child("users").Child(path + "/Erro Ditado").SetValueAsync(p.erroDitado);
		reference.Child("users").Child(path + "/Date & Time").SetValueAsync(System.DateTime.UtcNow.ToString("HH:mm dd MMMM, yyyy"));
        
		_connection.Insert(p);
        return p;
    }

    // Remove usuário do banco de dados!


    public void removeUser(int id)
    {
        var words = _connection.Table<palavraAcertoUser>().Where(x => x.idUser == id);
        foreach (var word in words)
        {
            _connection.Delete<palavraAcertoUser>(word.Id);
        }
        var u = new user
        {
            Id = id
        };
        _connection.Delete(u);
    }
		
}


public class user
{
    // Classe de usuário do jogo!
    [PrimaryKey, AutoIncrement]
    public int Id { get; set; }
    [Unique]
    public string Name { get; set; }

    public int Score { get; set; }

	public int Erros { get; set; }

    public int Nivel { get; set; }

	public int scoreDitado { get; set; }

	public int erroDitado { get; set; }

	public string key { get; set; }
    
	public override string ToString()
    {
        return string.Format("{1}", Id, Name);
    }


}


public class usuariosJogo : MonoBehaviour {
    // Classe de controle dos usuários!
    public Dropdown Users;

	public SQLiteConnection _connection;

    public GameObject somOn;
    public GameObject somOff;

	DatabaseReference reference;
	Firebase.Auth.FirebaseAuth auth;
	Firebase.Auth.FirebaseUser user_fb;

    // Inicialização da tela usuariosJogo!
    void Start () {

		InitializeFirebase ();

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

        IEnumerable<user> users = _connection.Table<user>(); 


        var words = _connection.Table<palavraAcertoUser>();

        var words2 = _connection.Table<palavraOpcao>();

        int index = 0;

        // buscando todos os usuários que estão cadastrados e jogando no select!

        foreach (var user in users)
        {
            Dropdown.OptionData a = new Dropdown.OptionData();
            a.text = user.Name;

            Users.options.Insert(index, a);
            ++index;
        }
        Debug.Log("Palavras User: " + words.Count() + " opções:" + words2.Count() + " Users: " +index);
    }

	void InitializeFirebase() {
		FirebaseApp.DefaultInstance.SetEditorDatabaseUrl("https://grapphia.firebaseio.com/");
		reference = FirebaseDatabase.DefaultInstance.RootReference;
		auth = Firebase.Auth.FirebaseAuth.DefaultInstance;
		user_fb = auth.CurrentUser;
	}
    // função para remover usuários!
    public void removeUser()
    {
		DataService data = new DataService(_connection);
	   //data.EstabeleceConexao ("grapphia");

       	var users = _connection.Table<user>().Where(x => x.Name == Users.captionText.text);
		var key = bancoPalavras.Instance._connection.Table<keyPhone> ().FirstOrDefault().keyFireBase;
        
        foreach (var user in users)
        {

            data.removeUser(user.Id);
			reference.Child ("users/" + key).Child ("/" + user.Name).RemoveValueAsync ();
        }
            Users.options.RemoveAt(Users.value);

            Users.captionText.text = "";


    }

    // Função ir para próxima tela!
    public void next()
    {
		//DataService data = new DataService();
		//data.EstabeleceConexao ("grapphia");

		if (Users.captionText.text == "")
			return;
		else {
			var users = _connection.Table<user>().Where(x => x.Name == Users.captionText.text);


			foreach(var user in users)
			{

				dadosJogo.Instance.currentUser = new user
				{
					Id = user.Id,
					Name = user.Name,
					Score = user.Score,
					Erros = user.Erros,
					Nivel = user.Nivel,
					scoreDitado = user.scoreDitado,
					erroDitado = user.erroDitado,
					key = user.key,
				};

			}

			SceneManager.LoadScene ("telaEstante");
		}


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
