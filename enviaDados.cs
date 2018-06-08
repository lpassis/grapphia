using UnityEngine;
using UnityEngine.UI;
using System.Net;
using System.Net.Mail;
using System.Collections;
using UnityEngine.SceneManagement;
using System.IO;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;

public class enviaDados : MonoBehaviour {
	public GameObject somOn;
	public GameObject somOff;

	// Use this for initialization
	void Start () {
	}

	//Função criada por Magno
	public void SalvaDadosJogado(){
		//Salvar txt para anexo
		int idUsuario = dadosJogo.Instance.currentUser.Id;
		string nomeUsuario = dadosJogo.Instance.currentUser.Name;
		int pontuacaoUsuarioAcerto = dadosJogo.Instance.currentUser.Score;
		int pontuacaoUsuarioAcertoDitado = dadosJogo.Instance.currentUser.scoreDitado;
		int pontuacaoUsuarioErroDitado = dadosJogo.Instance.currentUser.erroDitado;

		StreamWriter sw = new StreamWriter (Application.persistentDataPath + nomeUsuario + " - Pontuacao.txt");
		sw.WriteLine ("Id do Usuário: " + idUsuario);
		sw.WriteLine ("Nome do Usuário: " + nomeUsuario);
		sw.WriteLine ("Acertos do Usuário: " + pontuacaoUsuarioAcerto);
		sw.WriteLine ("Acertos do Usuário no Ditado: " + pontuacaoUsuarioAcertoDitado);
		sw.WriteLine ("Erros do Usuário no Ditado: " + pontuacaoUsuarioErroDitado);
		sw.WriteLine ("Palavras Acerto: ");
		for(int i=0; i<bancoPalavras.Instance.palavrasAcerto.Length; ++i){
			sw.WriteLine (bancoPalavras.Instance.palavrasAcerto[i].idPalavra + " - " + bancoPalavras.Instance.palavrasAcerto[i].acerto);
		}
		sw.Close ();
	}

	//Função criada por Magno
	/*Função para envio automatico de e-mail ao clicar em salvar dados do jogo, o e-mail será enviado para projetograpphia@gmail.com*/
	public void EnviarEmail ()
	{
		//Código para enviar email utilizando o protocolo smtp
		MailMessage mail = new MailMessage ();
		string nomeUsuario = dadosJogo.Instance.currentUser.Name;

		StartCoroutine ("SalvaDadosJogado");

		try {
			mail.From = new MailAddress ("projetograpphia@gmail.com");
			mail.To.Add ("projetograpphia@gmail.com");
			mail.Subject = "Pontuação do usuário: " + nomeUsuario;
			mail.Body = "Estes são os dados de jogo do(a): " + nomeUsuario ;
			mail.Attachments.Add (new Attachment (Application.persistentDataPath + nomeUsuario + " - Pontuacao.txt"));//anexo

			SmtpClient smtpServer = new SmtpClient ();
			smtpServer.DeliveryMethod = SmtpDeliveryMethod.Network;
			smtpServer.Port = 587;
			smtpServer.Host = "smtp.gmail.com";
			smtpServer.Credentials = new System.Net.NetworkCredential ("projetograpphia@gmail.com", "grapphia2017") as ICredentialsByHost;
			smtpServer.EnableSsl = true;
			ServicePointManager.ServerCertificateValidationCallback = 
				delegate(object s, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors) {
				return true;
			};
			smtpServer.Send (mail);
		} catch (SmtpException ex) {
			Debug.Log ("Exception: " + ex);
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
