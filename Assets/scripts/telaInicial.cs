using UnityEngine;
using UnityEngine.UI;
using System.Collections;

//Classe da tela inicial!
public class telaInicial : MonoBehaviour {

    public GameObject somOn; // opção ativar e desativar som!
    public GameObject somOff;
    // Inicialização da tela início!
    void Start () {
		//Se não tiver nenhuma palavra no banco você chama a função e preenche as palavras
		if(bancoPalavras.Instance.total_palavras < 1 ){
      	  bancoPalavras.Instance.salvar_palavras_no_banco();
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
