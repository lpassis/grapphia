using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System;
using SQLite4Unity3d;
using UnityEngine.SceneManagement;


public class DataService      // Classe de serviço do banco de dados!
{

    public SQLiteConnection _connection; // Criando conexão com SQLite!

    // Inicializando classe DataService!
    public DataService(string dbName)
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
    // Inserindo usuário no banco de dados!
	public user CreateUser(string newUser, int score, int nivel, int scoreDitado)
    {
        var p = new user
        {
            Name = newUser,
            Score = score,
            Nivel = nivel,
			scoreDitado = scoreDitado,
        };
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

    public int Nivel { get; set; }

	public int scoreDitado { get; set; }
    
	public override string ToString()
    {
        return string.Format("{1}", Id, Name);
    }


}


public class usuariosJogo : MonoBehaviour {
    // Classe de controle dos usuários!
    public Dropdown Users;

    public DataService data;

    public GameObject somOn;
    public GameObject somOff;

    // Inicialização da tela usuariosJogo!
    void Start () {

        data = new DataService("grapphia");
        IEnumerable<user> users = data._connection.Table<user>(); 


        var words = data._connection.Table<palavraAcertoUser>();

        var words2 = data._connection.Table<palavraOpcao>();

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

    // função para remover usuários!
    public void removeUser()
    {

       var users = data._connection.Table<user>().Where(x => x.Name == Users.captionText.text);

        
        foreach (var user in users)
        {

            data.removeUser(user.Id);

        }
            Users.options.RemoveAt(Users.value);

            Users.captionText.text = "";


    }

    // Função ir para próxima tela!
    public void next()
    {

        if (Users.captionText.text == "") return;
        else
        {

            var users = data._connection.Table<user>().Where(x => x.Name == Users.captionText.text);


            foreach(var user in users)
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

            SceneManager.LoadScene ("TelaEstante");

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
