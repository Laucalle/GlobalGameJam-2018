using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AguaScript : MonoBehaviour {
	public bool mortal;
	private bool mojado;
	private GameObject textura;

	// Use this for initialization
	void Start () {
		mortal = false;
		mojado = true;
	}
	
	// Update is called once per frame
	void Update () {
		//print (mortal.ToString ());
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

	public void OnTriggerStay2D(Collider2D collider2D){
		/*print (mortal.ToString ());

		if (mortal && collider2D.gameObject.tag == "Guard")
			collider2D.gameObject.SendMessage ("Die");
		print (collider2D.gameObject.tag.ToString ());*/
	}
}
