using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Node: IComparable<Node> {

	public Node parent;
	public Vector2 pos;
	public int g;
	public int h;

	public Node(Node parent, Vector2 pos, int g, int h){
		this.parent = parent;
		this.pos = pos;
		this.g = g;
		this.h = h;
	}

	public int Score(){
		return g + h;
	}

	public int CompareTo(Node other){
		if (other == null)
			return 1;
		return Score().CompareTo(other.Score());
	}
}

public class DuplicateKeyComparer<TKey>: IComparer<TKey> where TKey : IComparable{
	#region IComparer<TKey> Members

	public int Compare(TKey x, TKey y)
	{
		int result = x.CompareTo(y);

		if (result == 0)
			return 1;
		else
			return result;
	}

	#endregion
}

public class GuardiaScript : MonoBehaviour {
	public LayerMask blockingLayer;
	public float moveTime = 0.1f;
	public float umbral;
	public AudioClip GuardStepClip;

	public Vector2[] positions;
	public GameObject[] radios;

	WaterScript charcoActual;
	 
	private AudioSource GuardStepSource;
	private float inverseMoveTime;
	private bool dead;
	private bool cambiandoDeCharco;
	private float side_size;
	private Rigidbody2D rb2D;
	private BoxCollider2D boxCollider;
	private Animator animator;

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
		side_size = GetComponent<SpriteRenderer> ().bounds.size.x;
		float collider_size = (float) (side_size - 0.1);
		GetComponent<BoxCollider2D> ().size.Set (collider_size, collider_size);
		//List<Node> path = FindPath (new Vector2((int)transform.position.x, (int)transform.position.y), new Vector2 (5, 4));
		//foreach (Node n in path)
		//	print (n.pos);
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
				Vector2 dir_opuesta = new Vector2 (-dir_norm.x * side_size, -dir_norm.y * side_size);

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
					UpdateState ("GuardIdleBackwards");
				} else {
					UpdateState ("GuardPausa");
				}
					
			}
		}
	}

	private int ManhattanDistance (Vector2 src, Vector2 dst){
		return (int)Math.Abs (src.x - dst.x) + (int)Math.Abs (src.y - dst.y);
	}

	private List<Node> GetNeighbors(Node n, Node dst, Vector2[] moves){
		List<Node> neighbors = new List<Node> ();
		foreach(Vector2 m in moves){
			RaycastHit2D hit = Physics2D.Linecast(n.pos, n.pos+m, blockingLayer);
			if (hit.transform == null)
				neighbors.Add(new Node(n, n.pos+m,n.g+1,ManhattanDistance(n.pos+m,dst.pos)));
		}
		return neighbors;
	}

	private List<Node> FindPath(Vector2 src, Vector2 dst){
		Vector2[] moves = { // No diagonal steps allowed
			new Vector2 (0, 1),
			new Vector2 (1, 0),
			new Vector2 (0, -1),
			new Vector2 (-1, 0)
		};
		boxCollider.enabled = false;
		List<Node> open = new List<Node> ();
		List<Node> closed = new List<Node> ();
		Node current = new Node (null, src, 0, ManhattanDistance(src, dst)); // Start position
		Node dst_node = new Node (null, dst, 0, 0); // End position
		open.Add(current);

		while (open.Count > 0) {
			current = open [0]; // The best f (g+h) is always in [0]
			closed.Add (current);
			open.RemoveAt(0);

			if (closed.Find (x => x.pos == dst_node.pos) != null) // Found the end position
				break;
			
			List<Node> neighbors = GetNeighbors (current, dst_node, moves);
			foreach(Node ne in neighbors){
				if (closed.Find (x => x.pos == ne.pos) != null) // Discard neighbor if it's already in closed
					continue;
				Node result = open.Find (x => x.pos == ne.pos);
				if (result == null) // Add neighbor to open if not present
					open.Add (ne);
				else{ // Check if this path to ne is shorter than the one it had
					if (ne.Score () < result.Score ()) { 
						result.g = ne.g;		 // Reassign parent and g if it provides a shorter path
						result.parent = current;  
					}
				}
			}
			open.Sort ();
		}
		boxCollider.enabled = true;

		List<Node> path = new List<Node> ();
		while(current.parent != null){
			path.Add (current);
			current = current.parent;
		}
		path.Add (new Node (null, src, 0, 0));
		path.Reverse ();
		return path;
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
		float sqrRemainingDistance = (transform.position - end).sqrMagnitude;

		while (sqrRemainingDistance > float.Epsilon) {
			Vector3 newPosition = Vector3.MoveTowards (rb2D.position, end, inverseMoveTime * Time.deltaTime);
			rb2D.MovePosition (newPosition);
			sqrRemainingDistance = (transform.position - end).sqrMagnitude;
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
			charcoActual = col.gameObject.GetComponent<WaterScript> ();
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
