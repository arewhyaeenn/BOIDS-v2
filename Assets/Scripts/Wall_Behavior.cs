using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wall_Behavior : MonoBehaviour {

	void OnTriggerEnter (Collider other)
	{
		other.gameObject.SetActive (false);
		Debug.Log ("entered wall");
		switch (other.gameObject.tag) {

		case "Fish":
			other.gameObject.SetActive (false);
			Debug.Log ("fish entered wall");
			break;

		case "Shark":
			other.gameObject.SetActive (false);
			Debug.Log ("shark entered wall");
			break;

		default:
			break;
		}
	}
}
