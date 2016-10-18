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
	private bool cameraShaking = false;
	// Use this for initialization
	void Start () {
		Cursor.SetCursor (cursor, Vector2.zero, CursorMode.Auto);
		foreach (GameObject current in GameObject.FindGameObjectsWithTag("Enemy")) {
			Debug.Log ("Add Enemy");
			enemies.Add (current.GetComponent<Enemy>());
		}
	}

	IEnumerator shakeCamera()
	{
		cameraShaking = true;
		Camera.main.transform.Translate (0.1f, -0.1f, 0.0f, Space.Self);
		yield return new WaitForEndOfFrame ();
		Camera.main.transform.Translate (0.1f, 0.1f, 0.0f, Space.Self);
		yield return new WaitForEndOfFrame ();
		Camera.main.transform.Translate (-0.1f, -0.1f, 0.0f, Space.Self);
		yield return new WaitForEndOfFrame ();
		Camera.main.transform.Translate (-0.1f, 0.1f, 0.0f, Space.Self);
		cameraShaking = false;
	}

	public void cameraShake()
	{
		StartCoroutine (shakeCamera ());
	}

	// Update is called once per frame
	void Update () {
		if (currentPlayer != null && !cameraShaking)
			this.transform.position = new Vector3(currentPlayer.transform.position.x, currentPlayer.transform.position.y, this.transform.position.z);
		if (!this.gameWin.IsActive () && !this.gameOver.IsActive ()) {
			if (enemies.Count <= 0) {
				gameWin.gameObject.SetActive (true);
				currentPlayer.rb.isKinematic = true;
			}
			if (!currentPlayer)
			{
				gameOver.gameObject.SetActive (true);
			}
		}
	}
}
