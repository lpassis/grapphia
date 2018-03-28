using UnityEngine;
using System.Collections;

// Classe que move o fundo da tela de usuários cadastrados e tela de criar usuários
public class moveOffset : MonoBehaviour {
	private Material currentMaterial;
	public float velocidadeX;
	private float escalaMovimento;

	// Inicialização da classe!
	void Start () {
		
        currentMaterial = GetComponent<Renderer>().material;
	}

    // Vai ser chamada a cada frame por segundo!
    void Update () {

		escalaMovimento += 0.01f;

		currentMaterial.SetTextureOffset("_MainTex", new Vector2(escalaMovimento * velocidadeX, 0));

	}
}
