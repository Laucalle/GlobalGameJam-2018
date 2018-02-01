using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SceneChange : MonoBehaviour {
	//public GameObject uiIdle;
	public string nombreEscena;
	[Range(0f, 0.20f)]
	public float parallaxSpeed = 0.02f;
	public RawImage farIntro;
	public AudioSource audioSource;
	public AudioClip intro, introLoop, button;
	public float SilenceTime;


	// Use this for initialization
	void Start () {
		audioSource = GetComponent<AudioSource> ();
		StartCoroutine(PlayMusic ());
	}
	
	// Update is called once per frame
	void Update () {
		/*if (gameState == GameState.Idle && Input.GetKeyDown("up")){
			gameState = GameState.Playing;
			System.Console.WriteLine ("Pilla la tecla");
			uiIdle.SetActive(false);
		}*/

		if(Input.anyKey){
			StartCoroutine(PlayButton ());

		}
		MoverEscena ();
	}

	public void MoverEscena(){
		
		float finalSpeed = parallaxSpeed * Time.deltaTime;
		farIntro.uvRect = new Rect(farIntro.uvRect.x + finalSpeed, 0f, 1f, 1f);
	}

	IEnumerator PlayMusic(){
		audioSource.PlayOneShot (intro, 0.4f);
		yield return new WaitForSeconds (intro.length);
		audioSource.clip = introLoop;
		audioSource.volume = 0.4f;
		audioSource.Play();
		audioSource.loop = true;
	}

	IEnumerator PlayButton(){
		audioSource.PlayOneShot (button, 1f);
		yield return new WaitForSeconds (button.length);
		Silenciar ();
	}

	public void Silenciar(){
		StartCoroutine (CoSilenciar ());
	}
	IEnumerator CoSilenciar(){

		print (SilenceTime);
		for (float i = 0; i <= SilenceTime; i += Time.deltaTime) {
			audioSource.volume = 1 - (i / SilenceTime);
			yield return 0;
		}
		audioSource.volume = 0.0f;

		SceneManager.LoadScene (nombreEscena);

	}

}
