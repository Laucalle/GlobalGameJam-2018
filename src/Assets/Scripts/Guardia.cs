using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Guardia : MonoBehaviour {
	public BoxCollider2D boxCollider;
	public LayerMask blockingLayer;
	public Rigidbody2D rb2D;
	public float moveTime = 0.1f;
	public Animator animator;
	public float umbral;
	public AudioClip GuardStepClip;

	public Vector2[] positions;
	public GameObject[] radios;

	AguaScript charcoActual;

	private AudioSource GuardStepSource;
	private float inverseMoveTime;
	private bool dead;
	private bool cambiandoDeCharco;

	// Use this for initialization
	void Start () {
		boxCollider = GetComponent<BoxCollider2D> ();
		rb2D = GetComponent<Rigidbody2D> ();
		inverseMoveTime = 1.0f / moveTime;
		animator = GetComponent<Animator> ();
		GuardStepSource = GetComponent<AudioSource> ();
		dead = false;
		charcoActual = null;
		cambiandoDeCharco = false;
	}
		
	void Update () {
		if(charcoActual != null && charcoActual.mortal){
			Die ();
		}
	}

	public void Move(Vector3 param){
		if (!dead) {
			Vector2 dir = new Vector2 (param.x, param.y);
			float volume = param.z;
			if (volume >= umbral) {
				
				GuardStepSource.PlayOneShot (GuardStepClip, 0.7f);
				Turn ((int)dir.x);
				UpdateState ("GuardMovement");

				Vector2 dir_norm = dir;
				dir_norm.Normalize ();
				Vector2 dir_opuesta = new Vector2 (-dir_norm.x, -dir_norm.y);

				Vector2 start = transform.position;
				Vector2 end = start + dir;

				dir += dir_opuesta;
				MoveStep (dir);

				boxCollider.enabled = false;
				RaycastHit2D hit = Physics2D.Linecast (start, end, blockingLayer);
				boxCollider.enabled = true;

				hit.transform.gameObject.SendMessage ("ShutDown");

				MoveForSeconds (1f);

				if (dir_norm.y > 0) {
					//UpdateState ("GuardPausa");
					UpdateState ("GuardIdleBackwards");
				} else {
					UpdateState ("GuardPausa");
				}
					
			}
		}
	}

	// Espera "seconds" y apaga el sonido
	private IEnumerator MoveForSeconds(float seconds) {
		yield return new WaitForSeconds (1f);
		GuardStepSource.Stop ();
	}

	// dir es un vector con una componente a 0 y la otra 1 o -1
	void MoveStep (Vector2 dir) {
		Vector2 start = transform.position;
		Vector2 end = start + dir;

		StartCoroutine (SmoothMovement (end));
	}

	protected IEnumerator SmoothMovement (Vector3 end) {
		float sqrReaminingDistance = (transform.position - end).sqrMagnitude;

		while (sqrReaminingDistance > float.Epsilon) {
			Vector3 newPosition = Vector3.MoveTowards (rb2D.position, end, inverseMoveTime * Time.deltaTime);
			rb2D.MovePosition (newPosition);
			sqrReaminingDistance = (transform.position - end).sqrMagnitude;
			yield return null;
		}
	}

	public void Turn (int xDir) {
		if (xDir > 0) {
			GetComponent<SpriteRenderer> ().flipX = true;
		} else if (xDir < 0) {
			GetComponent<SpriteRenderer> ().flipX = false;
		}
	}

	private void UpdateState(string AnimationName){
		if (!dead && AnimationName != null)
			animator.Play (AnimationName);
	}

	public void Die() {
		if (!dead) {
			UpdateState ("GuardElectrocute");
			dead = true;
		}
	}

	public void OnTriggerEnter2D(Collider2D col){
		if (charcoActual != null) {
			cambiandoDeCharco = true;
		}
		if (col.transform.gameObject.layer == 4) {
			charcoActual = col.gameObject.GetComponent<AguaScript> ();
		}
	}

	public void OnTriggerExit2D(Collider2D col){
		if (!cambiandoDeCharco) {
			charcoActual = null;
		}
		cambiandoDeCharco = false;
	}

	public bool isDead() {
		return dead;
	}
}
