using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class FadeScript : MonoBehaviour {

	Image im;
	float FadeTime = 0.5f;
	Color negroTransparente = new Color(0,0,0,0);

	void Start () {
		im = GetComponent<Image> ();
		im.enabled = true;
		FadeIn();
	}
	
	public void FadeIn(){
		StartCoroutine (CoFadeIn ());
	}

	IEnumerator CoFadeIn(){
		for(float i =0; i<=FadeTime; i+=Time.deltaTime){
			im.color = Color.Lerp (Color.black, negroTransparente, i/FadeTime);
			yield return 0;
		}
		im.color = negroTransparente;
	}

	void Update () {
	}
}
