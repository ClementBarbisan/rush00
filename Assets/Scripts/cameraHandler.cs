using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class cameraHandler : MonoBehaviour {
	public player currentPlayer;
	public Texture2D cursor;
	public Image gameOver;
	public Image gameWin;
	[HideInInspector]
	public List<Enemy> enemies = new List<Enemy>();
	// Use this for initialization
	void Start () {
		Cursor.SetCursor (cursor, Vector2.zero, CursorMode.Auto);
		foreach (GameObject current in GameObject.FindGameObjectsWithTag("Enemy")) {
			Debug.Log ("Add Enemy");
			enemies.Add (current.GetComponent<Enemy>());
		}
	}
	
	// Update is called once per frame
	void Update () {
		if (currentPlayer != null)
			this.transform.position = new Vector3(currentPlayer.transform.position.x, currentPlayer.transform.position.y, this.transform.position.z);
		if (!this.gameWin.IsActive () && !this.gameOver.IsActive ()) {
			if (enemies.Count <= 0) {
				gameWin.gameObject.SetActive (true);
			}
			if (!currentPlayer)
			{
				gameOver.gameObject.SetActive (true);
			}
		}
	}
}
