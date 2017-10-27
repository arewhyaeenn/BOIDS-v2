using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Food_Behavior : MonoBehaviour {

	public float speed;
	public float spawn_rad;
	public float chaos;
	public float soft_max_distance;
	public float soft_boundary_weight;
	public Vector3 dir;


	void Start () {
		transform.position = Random.insideUnitCircle * spawn_rad;
	}


	void Update () {
		
		dir += Random.onUnitSphere * chaos;

		float from_origin = transform.position.magnitude;
		if (from_origin > soft_max_distance) {
			Vector3	other = transform.position.normalized;
			dir -= soft_boundary_weight * (from_origin - soft_max_distance) * other;
		}

		dir.Normalize ();
		transform.position += dir * speed;
	}
}
