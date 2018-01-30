using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour {
	private GameObject current;
	private GameObject nueva;
	private float restartTimer, restartDelay;
	private Animator animator;

	public Canvas canvas;
	public GameObject inicial;
	public AudioSource backgroundSource;
	public float SilenceTime;
	private AudioSource gameOverSource;
	public AudioClip gameOver;
	private float OverTimer;

	void Start () {
		gameOverSource = canvas.GetComponent<AudioSource> ();
		current = inicial;
		backgroundSource = GetComponent<AudioSource> ();
		restartTimer = 0f;
		restartDelay = 8.5f;
		animator = canvas.GetComponent<Animator> ();
	}

	public void Aumentar(){
		StartCoroutine (CoAumentar ());
	}

	IEnumerator CoAumentar(){

		for (float i = 0; i <= SilenceTime; i += Time.deltaTime) {
			backgroundSource.volume = i / SilenceTime;
			yield return 0;
		}
		backgroundSource.volume = 1.0f;

	}

	public void Silenciar(){
		StartCoroutine (CoSilenciar ());
	}
	IEnumerator CoSilenciar(){

		for (float i = 0; i <= SilenceTime; i += Time.deltaTime) {
			backgroundSource.volume = 1 - (i / SilenceTime);
			yield return 0;
		}
		backgroundSource.volume = 0.0f;

	}
	// Update is called once per frame
	void Update () {
		if (Input.GetMouseButtonDown (0)) {
			Vector2 punto = Camera.main.ScreenToWorldPoint (Input.mousePosition);

			RaycastHit2D hit = Physics2D.Raycast (punto, Vector2.zero);

			if (hit.collider != null && hit.transform.gameObject != current) {
				if (hit.transform.tag != "Untagged" && hit.transform.tag != "Guard") {
					nueva = hit.transform.gameObject;
					nueva.SendMessage ("Activar");
					current.SendMessage ("Desactivar");
					current = hit.transform.gameObject;
				}
			}
		}

		if (current.tag == "Player" && current.GetComponent<Player>().Dead()) {
			animator.Play ("GameOver");
			if (restartTimer == 0) {
				Silenciar ();
				gameOverSource.Play ();
			}
			restartTimer += Time.deltaTime;

			if (restartTimer >= restartDelay) {
				SceneManager.LoadScene(SceneManager.GetActiveScene().name);
			}

		}
	}


}
