﻿using UnityEngine;
using UnityEngine.UI;
using System.IO;
using SQLite4Unity3d;
using System.Collections;
using Firebase;
using Firebase.Unity.Editor;
using Firebase.Auth;
using Firebase.Database;

//Classe da tela inicial!
public class telaInicial : MonoBehaviour {

    public GameObject somOn; // opção ativar e desativar som!
    public GameObject somOff;
    // Inicialização da tela início!
    void Start () {
		
		// Conexão com o banco de dados grapphia!
		var filepath = string.Format("{0}/{1}", Application.persistentDataPath, "grapphia");
		FirebaseApp.DefaultInstance.SetEditorDatabaseUrl("https://grapphia.firebaseio.com/");
		DatabaseReference reference = FirebaseDatabase.DefaultInstance.RootReference;
		if (!File.Exists(filepath))
		{

			var loadDb = new WWW("jar:file://" + Application.dataPath + "!/assets/" + "grapphia");  

			while (!loadDb.isDone) { }  


			File.WriteAllBytes(filepath, loadDb.bytes);



		}
		var dbPath = filepath;

		SQLiteConnection _connection = new SQLiteConnection(dbPath, SQLiteOpenFlags.ReadWrite | SQLiteOpenFlags.Create);


		Debug.Log("Final PATH: " + dbPath);
		//Criando tabela usuário e tabela opção!
		_connection.CreateTable<user>();
		_connection.CreateTable<palavraOpcao>();
		_connection.CreateTable<palavraAcertoUser>();
		//Tabela keyPhone criada para gerar uma chave aleatoria no FireBase
		_connection.CreateTable<keyPhone>();

		var words2 = _connection.Table<palavraOpcao>();
		bancoPalavras.Instance._connection = _connection;
		bancoPalavras.Instance.total_palavras_geral = words2.Count();

		Debug.Log ("Total de palavras no BD: " + bancoPalavras.Instance.total_palavras_geral);


		//Se não tiver nenhuma palavra no banco você chama a função e preenche as palavras
		if(bancoPalavras.Instance.total_palavras_geral < 1 ){
			bancoPalavras.Instance.salvar_palavras_no_banco();
			string pushKey = reference.Child("users").Push().Key;

			var k = new keyPhone{
				keyFireBase = pushKey,
			};

			_connection.Insert(k);
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
