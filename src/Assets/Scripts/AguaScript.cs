using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AguaScript : MonoBehaviour {
	public bool mortal;
	private bool mojado;
	private GameObject textura;

	void Start () {
		mortal = false;
		mojado = true;
		float side_size = GetComponent<SpriteRenderer> ().bounds.size.x;
		GetComponent<BoxCollider2D> ().size.Set (side_size, side_size);
	}

	void Update () {
	}

	public void Secar(){
		textura = GameObject.Find("Charco");
		textura.SetActive (false);
		mojado = false;
	}

	public void ActivarMortalidad(){
		mortal = true;
	}

	public bool getMotalidad() {
		return mortal;
	}
}
