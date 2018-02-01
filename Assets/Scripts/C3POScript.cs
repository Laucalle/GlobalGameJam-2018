using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class C3POScript : MonoBehaviour {
	private bool activo;
	public GameObject agua;

	// Use this for initialization
	void Start () {
		activo = false;
	}
	
	// Update is called once per frame
	void Update () {
		if (Input.GetKeyDown ("w"))
			SecarAgua ();
	}

	public void SecarAgua(){
		agua.SendMessage ("Secar");
	}

	public void Activar(){
		activo = true;
	}

	public void Desactivar(){
		activo = false;
	}
}
