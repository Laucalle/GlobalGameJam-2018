using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class RadioScript : MonoBehaviour {

	public float guard_power;
	public float max_power;
	public AudioSource radioAudio;
	public AudioClip radioOnOffClip;
	public AudioClip radioActivaClip;
	public LayerMask blockingLayer;
	public Text controles;

	private bool activo;
	private Animator animator;
	private BoxCollider2D boxCollider;
	private float side_size;


	// Use this for initialization
	void Start () {
		activo = false;
		radioAudio = GetComponent<AudioSource> ();
		radioAudio.volume = 0.5f;
		animator = GetComponent<Animator> ();
		boxCollider = GetComponent<BoxCollider2D> ();

		side_size = GetComponent<SpriteRenderer> ().bounds.size.x;
		boxCollider.size.Set (side_size, side_size);
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
		UpdateState ("RadioSignal");
		radioAudio.Stop ();
		radioAudio.PlayOneShot (radioOnOffClip, 1f);
		Camera.main.GetComponent<GameManager> ().Aumentar ();
	}

	public IEnumerator Activar(){
		activo = true;
		UpdateState ("RadioOn");
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

    public void CallGuards()
    {
        List<Vector2> nodesToCheck = new List<Vector2>();
        List<Vector2> checkedNodes = new List<Vector2>();
        List<Vector2> guardsPositions = new List<Vector2>();
        Vector2 start = transform.position;
        nodesToCheck.Add(start + Vector2.right);
        nodesToCheck.Add(start + Vector2.left);
        nodesToCheck.Add(start + Vector2.up);
        nodesToCheck.Add(start + Vector2.down);
      
        while (nodesToCheck.Count != 0 )
        {
            Vector2 current = nodesToCheck[0];
            Vector2[] directions =
            {
                 Vector2.left,
                 Vector2.right,
                 Vector2.up,
                 Vector2.down
            };
         
            for (int i = 0; i < 4; i++)
            {
                Vector2 end = current + directions[i];
                RaycastHit2D hit = Physics2D.Linecast(current,end, blockingLayer);



                if (!checkedNodes.Contains(end))
                {
                    if (hit.transform == null)
                    {
                        if ((end.x > 0) && (end.y > 0) && (end.x < 9) && (end.y < 9))
                        {
                            nodesToCheck.Add(end);
                        }
                    }
                    else if (hit.transform.tag == "Guard" && (!hit.transform.gameObject.GetComponent<GuardiaScript>().isDead()) && (!guardsPositions.Contains(end)))
                    {
                        /*
                        boxCollider.enabled = true;
                        Vector3 param = new Vector3(transform.position.x, transform.position.y, radioAudio.volume);
                        hit.transform.gameObject.SendMessage("Move", param);
                        */
                        guardsPositions.Add(end);
                        Debug.Log("Guard found at " + end.x.ToString() + " , " + end.y.ToString());
                    }
                }
            }
            nodesToCheck.Remove(current);
            checkedNodes.Add(current);
        }
    }
}