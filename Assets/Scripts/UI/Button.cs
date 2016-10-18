using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class Button : MonoBehaviour {

	public Button		pink;
	private bool 		over = false;
	private Color		color;
	private int nbLevel = 2;

	void Update ()
	{
	 	if (over)
		{
			Quaternion rot = transform.rotation;
			
			rot.z += Random.Range (-0.005F, 0.005F);
			if (rot.z > 0.03F)
				rot.z = 0.03F;	
			else if (rot.z < -0.03F)
				rot.z = -0.03F;
			transform.rotation = rot;
		}
	}
	public void loadNextLevel()
	{
		if (Application.loadedLevel < nbLevel) 
		{
			Application.LoadLevel (Application.loadedLevel + 1);
			Camera.main.GetComponent<cameraHandler>().currentPlayer.rb.isKinematic = false;
		}
	}

	public void reloadLevel()
	{
		Application.LoadLevel(Application.loadedLevel);
	}

	public void MouseIsOn()
	{
		if (pink) {
			pink.over = true;
		}
		over = true;
	}

	public void MouseExit()
	{
		if (pink) {
			pink.over = false;
		}
		over = false;
	}

	public void LoadLevel()
	{
		Application.LoadLevel (1);
	}

	public void Exit()
	{
		Application.Quit ();
	}
	
}
