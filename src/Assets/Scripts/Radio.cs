using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class Radio : MonoBehaviour {

	public float guard_power;
	public float max_power;
	public GameObject SonSprite;
	public AudioSource radioAudio;
	public AudioClip radioOnOffClip;
	public AudioClip radioActivaClip;
	public LayerMask blockingLayer;

	private bool activo;
	private Animator animator;
	private BoxCollider2D boxCollider;
	private bool MensajeEnviado;
	public Text controles;


	// Use this for initialization
	void Start () {
		MensajeEnviado = false;
		activo = false;
		radioAudio = GetComponent<AudioSource> ();
		radioAudio.volume = 0.5f;
		animator = SonSprite.GetComponent<Animator> ();
		boxCollider = GetComponent<BoxCollider2D> ();
	}
	
	// Update is called once per frame
	void Update () {
		bool setting_volume = false;
		if(activo == true){
			//Aumenta la potencia
			if(Input.GetKeyDown("up")){
				//No puede superar el tope de potencia
				if (radioAudio.volume < max_power) {
					radioAudio.volume += 0.1f;
					setting_volume = true;
				}
			}
			//Bajar la potencia
			if (Input.GetKeyDown ("down")) {
				//No puede bajar de 0
				if (radioAudio.volume >= 0.1f) {
					radioAudio.volume -= 0.1f;
					setting_volume = true;
				}
			}

			if (setting_volume)
				CallGuards ();
		}
	}

	public void Desactivar(){
		activo = false;
		UpdateState ("DistractionChange");
		radioAudio.Stop ();
		radioAudio.PlayOneShot (radioOnOffClip, 1f);
		Camera.main.GetComponent<GameManager> ().Aumentar ();
	}

	public IEnumerator Activar(){
		activo = true;
		UpdateState ("DistractionActive");
		radioAudio.PlayOneShot (radioOnOffClip, 1f);
		yield return new WaitForSeconds (radioOnOffClip.length);
		Camera.main.GetComponent<GameManager> ().Silenciar ();
		radioAudio.PlayOneShot (radioActivaClip,1f);
		controles.text = "ARROWS: set volume\n CLICK: transfer";
		CallGuards ();
	}

	public void UpdateState(string state){
		if (state != null)
			animator.Play (state);
	}

	public void ShutDown () {
		max_power = guard_power;
		if (radioAudio.volume > max_power) {
			radioAudio.volume = max_power;
		}
	}

	public void CallGuards() {
		bool[] direcciones_libres = {true, true, true, true};
		bool found = false;
		int dis = 1, dis_limit = 7;
		Vector2 start = transform.position, pos_relativa, end;
		RaycastHit2D hit;

		boxCollider.enabled = false;

		while (dis < dis_limit && !found) {
			for (int i = 0; i < 4; i++) {
				if (direcciones_libres[i]) {
					
					if (i == 0) {
						pos_relativa = new Vector2 (0, dis);
					} else if (i == 1) {
						pos_relativa = new Vector2 (dis, 0);
					} else if (i == 2) {
						pos_relativa = new Vector2 (0, -dis);
					} else {
						pos_relativa = new Vector2 (-dis, 0);
					}

					end = start + pos_relativa;
					hit = Physics2D.Linecast(start, end, blockingLayer);

					if (hit.transform != null) {
						if (hit.transform.tag == "Guard" && (!hit.transform.gameObject.GetComponent<Guardia> ().isDead ()) ) {
							found = true;
							boxCollider.enabled = true;
							pos_relativa.Set (-pos_relativa.x, -pos_relativa.y);
							Vector3 param = new Vector3 (pos_relativa.x, pos_relativa.y, radioAudio.volume);
							hit.transform.gameObject.SendMessage ("Move", param);
						} else {
							direcciones_libres [i] = false;
						}
					}

				}
			}
			dis++;
		}

		boxCollider.enabled = true;
	}
}