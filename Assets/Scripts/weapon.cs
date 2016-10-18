using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class weapon : MonoBehaviour 
{
	public Sprite attachWeapon;
	public Sprite currentWeapon;
	public enum typeWeapons 
	{
		RANGE,
		CLOSE
	};
	public attack currentAttack;
	private List<attack> listAttack = new List<attack> ();
	public typeWeapons currentType;
	public float rateFire;
	private float lastFire;
	private float currentTime;
	public int maxbullets;
	[HideInInspector]
	public int bullets;
	private float lastRotation;
	private AudioClip takeWeapon;
	private AudioClip dry_fire;
	private IEnumerator coroutine = null;
	private AudioSource audioSource;
	public float stayVisible;

	// Use this for initialization
	void Start () {
		this.audioSource = this.GetComponent<AudioSource> ();
		this.dry_fire = Resources.Load("Sounds/dry_fire", typeof(AudioClip))as AudioClip;
		this.takeWeapon = Resources.Load ("Sounds/eject", typeof(AudioClip)) as AudioClip;
		lastFire = -rateFire;
		bullets = maxbullets;
		currentWeapon = this.GetComponent<SpriteRenderer> ().sprite;

	}

	public void throwAway()
	{
		this.GetComponent<SpriteRenderer> ().sprite = currentWeapon;
		lastRotation = this.transform.parent.GetComponent<player> ().currentRotation;
		this.transform.parent = null;
		this.audioSource.clip = takeWeapon;
		this.audioSource.Play ();
		coroutine = moveAway ();
		StartCoroutine(coroutine);
	}

	IEnumerator emitSparkles(attack current)
	{
		GameObject.Destroy (current.GetComponentInChildren<SpriteRenderer> ());
		if (current.GetComponent<ParticleSystem> ()) 
		{
			current.GetComponent<ParticleSystem> ().enableEmission = true;
			current.GetComponent<ParticleSystem> ().Play ();
			yield return new WaitForSeconds (0.2f);
		}
		else
			yield return null;
		GameObject.Destroy (current.gameObject);
	}

	public void destroyAttack(attack current)
	{
		if (current) {
			listAttack.Remove (current);
			StartCoroutine(emitSparkles(current));
		}
	}

	IEnumerator moveAway()
	{
		for (int i = 0; i < 10; i++) {
			this.transform.Rotate(new Vector3(0.0f, 0.0f, 30.0f), Space.Self);
			this.transform.Translate (new Vector3(-Mathf.Cos ((lastRotation + 90.0f) * Mathf.Deg2Rad) * 0.3f, -Mathf.Sin ((lastRotation + 90.0f) * Mathf.Deg2Rad) * 0.3f, 0.0f), Space.World);
			yield return null;
		}
		coroutine = null;
	}

	IEnumerator slash(attack current)
	{
		yield return new WaitForSeconds (this.stayVisible);
		this.destroyAttack (current);
	}

	public void shoot(bool player)
	{
		if (currentTime - lastFire >= rateFire && (maxbullets == 0 || bullets > 0))
		{
			lastFire = currentTime;
			if (player)
				Camera.main.GetComponent<cameraHandler>().cameraShake();
			listAttack.Add (GameObject.Instantiate(currentAttack));
			listAttack.Last ().setParent(this);
			listAttack.Last ().transform.position = this.transform.position;// + new Vector3(-Mathf.Cos ((lastRotation + 90.0f) * Mathf.Deg2Rad) * 0.3f, -Mathf.Sin ((lastRotation + 90.0f) * Mathf.Deg2Rad) * 0.3f, 0.0f);
			if (this.GetComponentInParent<player>())
				listAttack.Last().setDirection(this.transform.parent.GetComponentInParent<player> ().currentRotation + 90.0f);
			else if (this.GetComponentInParent<Enemy>())
				listAttack.Last().setDirection(this.transform.parent.GetComponentInParent<Enemy> ().currentRotation + 90.0f);
			if (maxbullets != 0)
				bullets--;
			if (currentType == typeWeapons.CLOSE)
			{
				StartCoroutine(slash(listAttack.Last()));
			}
		}
		else if (maxbullets > 0 && bullets <= 0)
		{
			this.audioSource.clip = this.dry_fire;
			this.audioSource.Play ();
		}
	}

	// Update is called once per frame
	void Update () {
		currentTime += Time.deltaTime;
	}



	void OnTriggerStay2D(Collider2D coll)
	{
		if (coll.tag == "Player" && (Input.GetMouseButtonDown (0) || Input.GetKeyDown(KeyCode.E))) {
			if (coll.GetComponent<player>().currentWeapon == null)
			{
				this.audioSource.clip = takeWeapon;
				this.audioSource.Play ();
				coll.GetComponent<player>().currentWeapon = this;
				this.transform.rotation = Quaternion.AngleAxis (coll.GetComponent<player>().currentRotation, Vector3.forward);; 
				this.transform.parent = coll.transform;
				this.transform.localPosition = new Vector2(-0.1f, -0.1f);
				this.GetComponent<SpriteRenderer>().sprite = attachWeapon;
			}

		}
		else if (coroutine != null && (coll.tag == "Wall" || coll.tag == "Door"))
			StopCoroutine(coroutine);
	}

	void OnTriggerEnter2D(Collider2D col)
	{
		if (coroutine != null && (col.tag == "Wall" || col.tag == "Door")) {
			StopCoroutine (coroutine);
			coroutine = null;
		}else if (coroutine != null && this.name == "Katana" && col.tag == "Enemy" && !col.isTrigger)
			col.gameObject.GetComponent<Enemy> ().death ();
		else if (coroutine != null && col.tag == "Enemy" && !col.isTrigger)
			col.gameObject.GetComponent<Enemy>().stunt();
	}
}
