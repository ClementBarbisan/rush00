using UnityEngine;
using System.Collections;

public class attack : MonoBehaviour {
	private float direction; 
	public float speed = 0.2f;
	public float initialOffset = 0.2f;
	[HideInInspector]
	public weapon weaponParent;
	public AudioClip spawnClip;
	private AudioSource audioSource;

	// Use this for initialization
	void Start () 
	{
		this.audioSource = this.GetComponent<AudioSource> ();
		this.audioSource.clip = spawnClip;
		this.audioSource.Play ();
		this.transform.position = this.transform.position + new Vector3 (-Mathf.Cos (direction * Mathf.Deg2Rad) * initialOffset, -Mathf.Sin (direction * Mathf.Deg2Rad) * initialOffset, 0.0f);
	}
	
	// Update is called once per frame
	void Update () 
	{
		if (Vector2.Distance (weaponParent.transform.position, this.transform.position) > 15f) {
			this.weaponParent.destroyAttack(this);
		}
		else
			this.transform.Translate (new Vector2(-Mathf.Cos (direction * Mathf.Deg2Rad) * speed, -Mathf.Sin (direction * Mathf.Deg2Rad) * speed));
	}

	public void setParent(weapon current)
	{
		this.weaponParent = current;
	}

	public void setDirection(float rotation)
	{
		this.direction = rotation;
		this.GetComponentInChildren<SpriteRenderer>().transform.rotation = Quaternion.AngleAxis ((rotation + 180.0f), Vector3.forward);
	}

	void OnCollisionEnter2D(Collision2D coll)
	{
		if (this.gameObject.layer == 12 && coll.gameObject.tag == "Enemy") {
			if (this.weaponParent)
				this.weaponParent.destroyAttack (this);
			else
				GameObject.Destroy (this.gameObject);
			coll.gameObject.GetComponent<Enemy> ().death ();
		} else if (this.gameObject.layer == 11 && coll.gameObject.tag == "Player") {
			if (this.weaponParent)
				this.weaponParent.destroyAttack (this);
			else
				GameObject.Destroy (this.gameObject);
			coll.gameObject.GetComponentInParent<player> ().death ();
		} else if (coll.gameObject.tag == "Wall" || coll.gameObject.tag == "Door") {
			if (this.weaponParent)
				this.weaponParent.destroyAttack (this);
			else
				GameObject.Destroy(this.gameObject);
		}
		else if (weaponParent && weaponParent.currentType == weapon.typeWeapons.CLOSE && coll.gameObject.tag == "Bullet")
			coll.gameObject.GetComponent<attack>().weaponParent.destroyAttack (coll.gameObject.GetComponent<attack> ());
	}
}
