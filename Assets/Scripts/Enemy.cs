using UnityEngine;
using System.Collections;

public class Enemy : MonoBehaviour {

	public player			player;
	[HideInInspector]	
	public Room				current_room;
	[HideInInspector]
	public bool				following = false;
	[HideInInspector]
	public bool				player_in_range = false;
	private Rigidbody2D		rb;
	private Door			go_to_door;
	private bool			indoor = false;
	[HideInInspector]
	public float 			currentRotation = 0.0f;
	private float nextAngle = 0.0f;
	private bool rotating = false;
	public float rotationSpeed;
	public float speed;
	public AudioClip deathClip;
	private AudioSource audioSource;
	public Checkpoint_list	checkpoint;
	private int				next = 0;
	[HideInInspector]
	public bool onDeath = false;
	private bool isStunt = false;
	
	void Start ()
	{
		player.onShoot += onPlayerShoot;
		rb = GetComponent<Rigidbody2D> ();
		this.audioSource = this.GetComponent<AudioSource> ();
		this.audioSource.clip = deathClip; 
	}

	void onPlayerShoot(weapon.typeWeapons type)
	{
		Debug.Log ("Player just shot");
		if (((room_near(this.player.current_room) != null && type == weapon.typeWeapons.RANGE) 
		    || (this.player.current_room == this.current_room)) && Vector2.Distance(this.player.transform.position, this.transform.position) < 10.0f)
			this.following = true;
	}

	IEnumerator rotate()
	{
		this.rotating = true;
		Vector3 direction = this.transform.rotation.eulerAngles;
		while (((direction.z + 0.01f) * this.nextAngle > 0.0f && (direction.z + this.rotationSpeed + 0.02f < this.nextAngle || direction.z - this.rotationSpeed - 0.02f > this.nextAngle))
		       || ((direction.z + 0.01f) * this.nextAngle < 0.0f && (direction.z - this.rotationSpeed - 0.02f < this.nextAngle || direction.z + this.rotationSpeed + 0.02f > this.nextAngle)))
		{

			if (this.nextAngle < direction.z && (direction.z + 0.01f) * this.nextAngle > 0.0f)
				this.transform.Rotate(0.0f, 0.0f, -this.rotationSpeed);
			else
				this.transform.Rotate(0.0f, 0.0f, this.rotationSpeed);
			direction = this.transform.rotation.eulerAngles;
			yield return null;
		}
		this.rotating = false;
	}

	IEnumerator stuntCharacter()
	{
		this.isStunt = true;
		yield return new WaitForSeconds (1.5f);
		this.isStunt = false;
	}

	public void stunt()
	{
		StartCoroutine("stuntCharacter");
	}

	void Update ()
	{
		if (!this.onDeath && !this.player.onDeath && !this.isStunt) 
		{
			if (following && this.player && Vector2.Distance(this.player.transform.position, this.transform.position) < 15.0f) 
			{
				// Rotation du suit le player
				if (room_near(player.current_room))
				{
					Vector2 dir = player.transform.position - transform.position;
					this.nextAngle = Mathf.Atan2 (dir.y, dir.x) * Mathf.Rad2Deg + 90.0f;
					if (!rotating)
						StartCoroutine ("rotate");
				}
					// Changer de piece si le joueur est dans une piece differente
				if (current_room.name != player.current_room.name && indoor == false) 
				{
					Door currentDoor = room_near (player.current_room);
					go_to_another_room (currentDoor);
				} else { // Dans la meme piece que le joueur ou a la porte
					// si le joueur est pas dans le trigger
					if (player_in_range) {
						rb.velocity = Vector2.zero;
						Vector3 direction = this.transform.rotation.eulerAngles;
						currentRotation = direction.z;
						RaycastHit2D[] hitAll = Physics2D.RaycastAll (this.transform.position, new Vector2 (Mathf.Cos ((direction.z - 90.0f) * Mathf.Deg2Rad), Mathf.Sin ((direction.z - 90.0f) * Mathf.Deg2Rad)));
						foreach (RaycastHit2D hit in hitAll) {
							if (hit.collider.tag == "Player" && hit.distance < 5.0f)
								this.GetComponentInChildren<weapon> ().shoot ();
							else if (hit.collider.tag == "Wall" || hit.collider.tag == "Door")
								break;
						}
					} else {
						transform.position = Vector2.MoveTowards (transform.position, player.transform.position, speed);
					}
				}
			} else if (checkpoint != null) {
				patrol ();
			}
			if (!following)
				rb.velocity = Vector2.zero;
		}
		else if (this.isStunt)
			this.transform.Rotate(new Vector3(0.0f, 0.0f, 10.0f), Space.Self);
	}

	private void patrol()
	{
		if (transform.position == checkpoint.checkpoints[next].transform.position)
			next++;
		if (next >= checkpoint.checkpoints.Count)
			next = 0;
		Vector2 dir = checkpoint.checkpoints[next].transform.position - transform.position;
		transform.rotation = Quaternion.AngleAxis (Mathf.Atan2 (dir.y, dir.x) * Mathf.Rad2Deg + 90.0f, Vector3.forward);
		transform.position = Vector2.MoveTowards (transform.position, checkpoint.checkpoints[next].transform.position, 0.05F);
	}
	
	private Door room_near(Room go_to)
	{	
		go_to_door = null;
		foreach (Door door in current_room.doors)
		{
			foreach(Door to_door in go_to.doors)
			{
				
				if (to_door.name == door.name)
				{
					go_to_door = door;
					break ;
				}
			}
		}
		return (go_to_door);
	}

	private void go_to_another_room(Door go_to_door)
	{
		if (go_to_door != null)
			transform.position = Vector2.MoveTowards(transform.position, go_to_door.transform.position + new Vector3(0F, 0.8F, 0F), this.speed);

	}

	IEnumerator playDeath()
	{
		this.audioSource.Play ();
		while (this.audioSource.isPlaying)
			yield return null;
		GameObject.Destroy (this.gameObject);
	}
	
	public void death()
	{
		if (!this.onDeath) {
			this.onDeath = true;
			this.player.onShoot -= onPlayerShoot;
			foreach (SpriteRenderer sprite in this.GetComponentsInChildren<SpriteRenderer>())
				sprite.color = new Color (0.0f, 0.0f, 0.0f, 0.0f);
			this.gameObject.GetComponentInChildren<weapon>().GetComponent<SpriteRenderer>().color = new Color(0.0f, 0.0f, 0.0f, 0.0f);
			this.tag = "Untagged";
			Camera.main.GetComponent<cameraHandler>().enemies.Remove(this);
			StartCoroutine ("playDeath");
		}
	}

	void OnTriggerStay2D(Collider2D col)
	{
		if (col.tag == "Room")		
			current_room = col.GetComponent<Room>();
	}

	void OnCollisionEnter2D(Collision2D col)
	{
		if (col.gameObject.tag == "Door")
			indoor = true;
	}

	void OnCollisionExit2D(Collision2D col)
	{
		if (col.gameObject.tag == "Door")
			indoor = false;
	}
}
