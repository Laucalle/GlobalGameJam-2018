using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Player : MonoBehaviour {

	public int stride = 1;
	public float stepDelay = 0.5f;
	public float moveTime = 0.1f;
	public LayerMask blockingLayer;
	public LayerMask waterLayer;

	public AudioClip playerStep;
	public AudioClip playerDie;
	public AudioClip playerWave;
	public AudioClip gameOver;
	public AudioClip exitClip;
	public Text controles;
	public float SilenceTime;
	public Canvas canvas;

	private bool death;
	private BoxCollider2D boxCollider;
	private Rigidbody2D rb2D;
	private float inverseMoveTime;
	private Animator animator;
	private float delayCounter = 0.0f;
	private bool activated;
	private AudioSource audioSource;
	private float side_size;

	// Use this for initialization
	void Start () {
		boxCollider = GetComponent<BoxCollider2D> ();
		rb2D = GetComponent<Rigidbody2D> ();
		inverseMoveTime = 1.0f / moveTime;
		animator = GetComponent<Animator> ();
		activated = true;
		death = false;
		audioSource = GetComponent<AudioSource> ();
		controles.text = "ARROWS: move\n CLICK: transfer";

		side_size = GetComponent<SpriteRenderer> ().bounds.size.x;
		float collider_size = (float) (side_size - 0.1);
		GetComponent<BoxCollider2D> ().size.Set (collider_size, collider_size);
	}

	void Activar() {
		activated = true;
		UpdateState ("PlayerStandUp");
		controles.text = "ARROWS: move\n CLICK: transfer";
	}

	IEnumerator Desactivar() {
		audioSource.PlayOneShot (playerWave, 1.0f);
		yield return new WaitForSeconds (playerWave.length);
		UpdateState ("PlayerSignal");
		activated = false;
	}

	void Move(int xDir, int yDir, out RaycastHit2D hit) {
		Vector2 start = transform.position;
		Vector2 end = start + new Vector2(xDir,yDir);
		boxCollider.enabled = false;
		hit = Physics2D.Linecast(start, end, blockingLayer);
		boxCollider.enabled = true;

		if (hit.transform == null) {
			Turn (xDir);
			UpdateState("PlayerMovement");
			audioSource.PlayOneShot (playerStep, 0.1f);
			StartCoroutine (SmoothMovement (end));
		} else
			UpdateState ("PlayerHit");
	}

	protected IEnumerator SmoothMovement (Vector3 end) {
		float sqrReaminingDistance = (transform.position - end).sqrMagnitude;

		while (sqrReaminingDistance > float.Epsilon) {
			Vector3 newPosition = Vector3.MoveTowards (rb2D.position, end, inverseMoveTime * Time.deltaTime);
			rb2D.MovePosition (newPosition);
			sqrReaminingDistance = (transform.position - end).sqrMagnitude;
			yield return null;
		}
		transform.position = end;
		//yield return new WaitForSeconds (0.5f);
	}

	// Update is called once per frame
	void Update () {
		if (activated) {
			if (delayCounter >= stepDelay) {
				int horizontal = 0;
				int vertical = 0;

				horizontal = (int)(Input.GetAxisRaw ("Horizontal"));
				vertical = (int)(Input.GetAxisRaw ("Vertical"));

				if (horizontal != 0) {
					vertical = 0;
				}

				RaycastHit2D hit;

				if (horizontal != 0 || vertical != 0) {
					horizontal *= (int) side_size;
					vertical *= (int) side_size;
					Move (horizontal, vertical, out hit);
					delayCounter = 0.0f;
				}
			} else
				delayCounter += Time.deltaTime;
		}
	}

	private void die(){
		UpdateState ("PlayerElectrocute");
		audioSource.PlayOneShot (playerDie, 0.7f);
		activated = false;
		StartCoroutine (Wait (2f));

	}

	public IEnumerator Wait(float seconds) {
		yield return new WaitForSeconds (seconds);
		death = true;
	} 

	private void UpdateState(string AnimationName){
		if (AnimationName != null)
			animator.Play (AnimationName);
	}

	public void Turn (int xDir) {
		if (xDir > 0) {
			GetComponent<SpriteRenderer> ().flipX = true;
		} else if (xDir < 0) {
			GetComponent<SpriteRenderer> ().flipX = false;
		}
	}

	public bool Dead () {
		return death;
	}

	public void OnTriggerEnter2D(Collider2D collider2D){
		if (collider2D.gameObject.layer == 4) { // La 4 es la capa de agua
			die ();
		} else if (collider2D.gameObject.name == "Gate") {
			audioSource.PlayOneShot (exitClip, 1f);
			activated = false;
			canvas.GetComponent<Animator>().Play ("FadeEnd");
			Silenciar ();
		}
	}

	public void Silenciar(){
		StartCoroutine (CoSilenciar ());
	}

	IEnumerator CoSilenciar(){

		yield return new WaitForSeconds (exitClip.length - 0.6f);
		SceneManager.LoadSceneAsync (SceneManager.GetActiveScene().buildIndex +1 );
	}
}
