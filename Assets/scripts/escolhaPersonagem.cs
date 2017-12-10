using UnityEngine;
using System.Collections;

public class escolhaPersonagem : MonoBehaviour {


    public GameObject somOn;
    public GameObject somOff;


   
    void Start () {
	
	}
	
	
	void Update () {
	
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
