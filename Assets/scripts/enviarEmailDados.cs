using UnityEngine;
using System;
using System.Net;
using System.Net.Mail;
using System.Collections;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;

public class enviarEmailDados : MonoBehaviour {

	void Start()
	{
		
	}

	public void EnviarEmail(){
		MailMessage mail = new MailMessage();

		mail.From = new MailAddress("projetograpphia@gmail.com");
		mail.To.Add("projetograpphia@gmail.com");
		mail.Subject = "Test Mail";
		mail.Body = "This is for testing SMTP mail from GMAIL";

		SmtpClient smtpServer = new SmtpClient("smtp.gmail.com");
		smtpServer.Port = 587;
		smtpServer.Credentials = (ICredentialsByHost) new System.Net.NetworkCredential("projetograpphia@gmail.com", "grapphia2017");
		smtpServer.EnableSsl = true;
		ServicePointManager.ServerCertificateValidationCallback = 
			delegate(object s, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors) 
		{ return true; };
		smtpServer.Send(mail);
	}

}
