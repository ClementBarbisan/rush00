using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class Panel_color : MonoBehaviour {

	private float 	b = 0.7F;
	private float	inc = 0.02F;
	public Texture2D currentCursor;
	void Start()
	{
		Cursor.SetCursor (currentCursor, Vector2.zero, CursorMode.Auto);
	}

	void Update ()
	{
		GetComponent<Image> ().color = new Color(0.9F, 0.45F, b, 1F);
		b += inc;
		if (b > 1F || b < 0F)
			inc *= -1;
	}
}
