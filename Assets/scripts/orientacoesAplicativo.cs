using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(AudioSource))]
public class orientacoesAplicativo : MonoBehaviour {
	private string movie = "video_entrada.mp4";

	void Start () 
	{
		StartCoroutine(streamVideo(movie));
	}

	private IEnumerator streamVideo(string video)
	{
		
		Handheld.PlayFullScreenMovie(video, Color.black, FullScreenMovieControlMode.Hidden, FullScreenMovieScalingMode.AspectFill);
		yield return new WaitForEndOfFrame();
		//retornar do video antes de terminar a lógica de 
		SceneManager.LoadScene ("telaInicialOpcoes");
	}
}
