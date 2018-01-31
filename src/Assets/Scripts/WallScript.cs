using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallScript : MonoBehaviour {

	private float side_size;

	// Use this for initialization
	void Start () {
		side_size = GetComponent<SpriteRenderer> ().bounds.size.x;
		GetComponent<BoxCollider2D> ().size.Set (side_size, side_size);
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
