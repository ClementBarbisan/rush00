using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class bullets : MonoBehaviour {

	public player 	Player;
	public Text		nb;
	public Image	img;

	private weapon 	weapon;
	private Color	color;

	void Start ()
	{
		color = img.color;
	}
	
	// Update is called once per frame
	void Update ()
	{
		if (!Player.onDeath) {
			weapon = Player.GetComponentInChildren<weapon> ();
			if (weapon) {
				color.a = 1;
				img.color = color;
				if (weapon.currentType == weapon.typeWeapons.CLOSE)
					nb.text = "INF";
				else
					nb.text = weapon.bullets.ToString ();
				img.sprite = weapon.currentWeapon;
			} else {
				color.a = 0;
				img.color = color;
				nb.text = "";
			}
		}
	}
}
