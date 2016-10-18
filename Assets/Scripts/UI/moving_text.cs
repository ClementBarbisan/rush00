using UnityEngine;
using System.Collections;

public class moving_text : MonoBehaviour {
	
	void Update ()
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
