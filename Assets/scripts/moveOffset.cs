using UnityEngine;
using System.Collections;

// Classe que move o fundo da tela de usuários cadastrados e tela de criar usuários
public class moveOffset : MonoBehaviour {


    private Material currentMaterial;
    public float speed;
    private float offset;

	// Inicialização da classe!
	void Start () {
        currentMaterial = GetComponent<Renderer>().material;
	}

    // Vai ser chamada a cada frame por segundo!
    void FixedUpdate () {

        offset += 0.001f;

        currentMaterial.SetTextureOffset("_MainTex", new Vector2(offset * speed, 0));

	}
}
