using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.IO;
using SQLite4Unity3d;
using UnityEngine.SceneManagement;
using Firebase;
using Firebase.Auth;
using Firebase.Unity.Editor;
using Firebase.Database;

public class createUser : MonoBehaviour {


    public InputField nome; // Campo onde o usuário insere seu nome!

    public GameObject somOn; // Opção ativar e desativar som!
    public GameObject somOff;
	public SQLiteConnection _connection;

	Firebase.Auth.FirebaseAuth auth;
	Firebase.Auth.FirebaseUser fb_user;
	DatabaseReference reference;

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

		//Tabela keyPhone criada para gerar uma chave aleatoria no FireBase
		_connection.CreateTable<keyPhone>();

		InitializeFirebase ();

		if(bancoPalavras.Instance.total_palavras_geral < 1 ){

			string key = reference.Child("users").Push().Key;

			var k = new keyPhone{
				keyFireBase = key,
			};

			_connection.Insert(k);
		}
	}

	public string getKey(){
		var key = _connection.Table<keyPhone> ().FirstOrDefault().keyFireBase;
		return key;
	}

	void InitializeFirebase() {
		auth = Firebase.Auth.FirebaseAuth.DefaultInstance;
		auth.StateChanged += AuthStateChanged;
		AuthStateChanged(this, null);
		FirebaseApp.DefaultInstance.SetEditorDatabaseUrl("https://grapphia.firebaseio.com/");
		reference = FirebaseDatabase.DefaultInstance.RootReference;
	}

	void AuthStateChanged(object sender, System.EventArgs eventArgs) {
		if (auth.CurrentUser != fb_user) {
			bool signedIn = fb_user != auth.CurrentUser && auth.CurrentUser != null;
			if (!signedIn && fb_user != null) {
				Debug.Log("Signed out " + fb_user.UserId);
			}
			fb_user = auth.CurrentUser;
			if (signedIn) {
				Debug.Log("Signed in " + fb_user.UserId);
			}
		}
	}

    // Função para criar usuário!
    public void create()
    {
		if (nome.text == "") return;

		var users = _connection.Table<user>().Where(x => x.Name == nome.text);

		DataService data = new DataService(_connection);

		//string password = "12345678";
		//string email = nome.text + "@grapphia.com";

		auth.SignInAnonymouslyAsync().ContinueWith(task => {
			if (task.IsCanceled) {
				Debug.LogError("SignInAnonymouslyAsync was canceled.");
				return;
			}
			if (task.IsFaulted) {
				Debug.LogError("SignInAnonymouslyAsync encountered an error: " + task.Exception);
				return;
			}
			if (users.Count () >= 1 ) {;
				SceneManager.LoadScene ("telaUsuarioCadastrado");
				return;
			}
			Firebase.Auth.FirebaseUser newUser = task.Result;
			Debug.LogFormat("User signed in successfully: {0} ({1})",
				newUser.DisplayName, newUser.UserId);

			reference.Child("admin/" + newUser.UserId + "/Date & Time").SetValueAsync(System.DateTime.UtcNow.ToString("HH:mm dd MMMM, yyyy"));

			data.CreateUser(nome.text, 0,0,0, getKey());

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
					key = getKey(),
				};
			}
			nome.text = null;
			SceneManager.LoadScene ("telaEstante");
		});

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

	public void SignInemail(string email, string password){
		//exists = true;
		auth.CreateUserWithEmailAndPasswordAsync(email, password).ContinueWith(task => {

			if (task.IsCanceled) {
				Debug.LogError("CreateUserWithEmailAndPasswordAsync was canceled.");

				return;
			}
			if (task.IsFaulted) {
				Debug.LogError("CreateUserWithEmailAndPasswordAsync encountered an error: " + task.Exception);
				Debug.Log("usuario cadastrado FB ");
				SceneManager.LoadScene ("telaUsuarioCadastrado");
				return ;
			}

			// Firebase user has been created.
			Firebase.Auth.FirebaseUser newUser = task.Result;
			Debug.LogFormat("Firebase user created successfully: {0} ({1})", newUser.DisplayName, newUser.UserId);


		});

	}

}
