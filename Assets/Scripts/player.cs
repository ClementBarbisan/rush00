using UnityEngine;
using System.Collections;

public class player : MonoBehaviour {
	public float speed = 1.0f; 
	private Rigidbody2D rb;
	[HideInInspector]
	public weapon currentWeapon = null;
	[HideInInspector]
	public float currentRotation = 0.0f;
	[HideInInspector]
	public Room current_room;
	public delegate void onShootEvent(weapon.typeWeapons type);
	public event onShootEvent onShoot;
	public AudioClip deathClip;
	private AudioSource audioSource;
	[HideInInspector]
	public bool onDeath = false;

	// Use this for initialization
	void Start () {
		rb = this.GetComponent<Rigidbody2D> ();
		this.audioSource = this.GetComponent<AudioSource> ();
		this.audioSource.clip = deathClip;
	}
	
	// Update is called once per frame
	void Update () {
		if (!this.onDeath && !Camera.main.GetComponent<cameraHandler>().gameWin.IsActive()) {
			Vector2 dist = Camera.main.ScreenToWorldPoint (Input.mousePosition) - this.transform.position;
			Vector2 velocity = new Vector2 (0.0f, 0.0f);
			currentRotation = Mathf.Atan2 (dist.y, dist.x) * Mathf.Rad2Deg + 90.0f;
			if (Input.GetKey (KeyCode.W))
				velocity.y += speed;
			if (Input.GetKey (KeyCode.S))
				velocity.y -= speed;
			if (Input.GetKey (KeyCode.A))
				velocity.x -= speed;
			if (Input.GetKey (KeyCode.D))
				velocity.x += speed;
			this.transform.rotation = Quaternion.AngleAxis (currentRotation, Vector3.forward);
			rb.velocity = velocity;
			if (currentWeapon != null && Input.GetMouseButtonDown (1)) {
				currentWeapon.throwAway ();
				currentWeapon = null;
			}
			if (currentWeapon != null && Input.GetMouseButton (0)) {
				this.onShoot (this.currentWeapon.currentType);
				currentWeapon.shoot ();
			}
		}
	}

	IEnumerator playDeath()
	{
		this.onDeath = true;
		this.audioSource.Play ();
		while (this.audioSource.isPlaying)
			yield return null;
		GameObject.Destroy (this.gameObject);
	}

	public void death()
	{
		if (!this.onDeath)
			StartCoroutine ("playDeath");
	}

	void OnTriggerEnter2D(Collider2D col)
	{
		if (col.tag == "Enemy" && col.GetComponent<Enemy> ().current_room.name == current_room.name)
		{
			col.GetComponent<Enemy> ().following = true;
			col.GetComponent<Enemy> ().player_in_range = true;
		}
	}

	void OnTriggerExit2D(Collider2D col)
	{
		if (col.tag == "Enemy" && col.GetComponent<Enemy> ().current_room.name == current_room.name)
			col.GetComponent<Enemy> ().player_in_range = false;
	}
	
	void OnTriggerStay2D(Collider2D col)
	{
		if (col.tag == "Room")		
		{
			current_room = col.GetComponent<Room>();
		}
	}

}
