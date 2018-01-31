using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LamparaScript : MonoBehaviour {

	public GameObject charco;
	private AudioSource lampSource;
	public AudioClip buzzClip;
	public AudioClip switchClip;
	public AudioClip boomClip;
	private bool activated;
	public Animator animator;
	public Text controles;

	// Use this for initialization
	void Start () {
		activated = false;
		animator = GetComponent<Animator> ();
		lampSource = GetComponent<AudioSource> ();
		lampSource.volume = 0.15f;

		float side_size = GetComponent<SpriteRenderer> ().bounds.size.x;
		GetComponent<BoxCollider2D> ().size.Set (side_size, side_size);
	}
	
	// Update is called once per frame
	void Update () {
		if (activated) {
			if(Input.GetKeyDown("up")){
				Electrocute ();
			}
		}
	}

	private void Electrocute() {
		UpdateState ("LampTrap");
		GameObject child;

		lampSource.PlayOneShot (boomClip, 1f);

		for (int i = 0; i < charco.transform.childCount; i++) {
			child = charco.transform.GetChild (i).gameObject;
			child.SendMessage ("ActivarMortalidad");
		}

	}

	public void Desactivar(){
		activated = false;
		UpdateState ("LampSignal");
		lampSource.Stop ();
		lampSource.PlayOneShot (switchClip, 1f);
	}

	public IEnumerator Activar(){
		activated = true;
		UpdateState ("LampOn");
		lampSource.PlayOneShot (switchClip, 1f);
		controles.text = "ARROWS: intensity\n CLICK: transfer";
		yield return new WaitForSeconds (switchClip.length);
		//Camera.main.GetComponent<GameManager> ().Silenciar ();
		lampSource.clip = buzzClip;
		lampSource.loop = true;
		lampSource.Play ();
	}

	private void UpdateState(string AnimationName){
		if (AnimationName != null)
			animator.Play (AnimationName);
	}
}
