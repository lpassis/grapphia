using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.SceneManagement;

public class createUser : MonoBehaviour {


    public InputField nome; // Campo onde o usuário insere seu nome!

    public GameObject somOn; // Opção ativar e desativar som!
    public GameObject somOff;



    // Função para criar usuário!
    public void create()
    {
        // Conexão com o banco de dados grapphia!
        DataService data = new DataService("grapphia");

        if (nome.text == "") return;

		var users = data._connection.Table<user>().Where(x => x.Name == nome.text);

		if (users.Count () >= 1) {;
			SceneManager.LoadScene ("telaUsuarioCadastrado");
			return;
		};

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
