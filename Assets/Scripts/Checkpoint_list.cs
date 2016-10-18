using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Checkpoint_list : MonoBehaviour {

	public List<GameObject>		checkpoints;

	void OnDrawGizmos()
	{
		Gizmos.DrawIcon (transform.position, "hud/arrow.png", false);
	}
}
