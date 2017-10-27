using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fish_Behavior : MonoBehaviour {

	// view, speed, randomness
	public float view_rad;
	public float speed;
	public float chaos;

	// weights and max distances for stimuli
	public float wall_weight;
	public float cohesion_weight;
	public float separation_weight;
	public float alignment_weight;
	public float shark_weight;
	public float separation_distance;
	public float wall_distance;
	private Dictionary<string, float> weight;

	// spawn location max distance from origin, direction
	public float spawn_sphere_rad;
	public Vector3 dir;

	// number of nearest neighbors
	public int k_fish;
	public int k_shark;


	void Start () {

		weight = new Dictionary<string, float> ();
		weight.Add ("Wall", wall_weight);
		weight.Add ("Cohesion", cohesion_weight);
		weight.Add ("Alignment", alignment_weight);
		weight.Add ("Separation", separation_weight);
		weight.Add ("Shark", shark_weight);

		dir = UnityEngine.Random.onUnitSphere;
		transform.position = UnityEngine.Random.insideUnitSphere * spawn_sphere_rad;
	}


	void Update () {

		// nearby fish (layer 9)
		Vector3 cohesion_vector = Vector3.zero;
		Vector3 separation_vector = Vector3.zero;
		Vector3 alignment_vector = Vector3.zero;
		Collider[] neighbors = neighborsByDistance (9, view_rad);
		int i = 0;
		int I = Math.Min (k_fish + 1, neighbors.Length);
		while (i < I) {
			// cohesion
			Vector3 other = neighbors [i].transform.position - transform.position;
			float distance = other.magnitude;
			other.Normalize ();
			cohesion_vector += (view_rad - distance) * other;
			// separation 
			if (distance < separation_distance) {
				separation_vector += (distance - separation_distance) * other;
			}
			// alignment
			other = neighbors[i].GetComponent<Fish_Behavior>().dir;
			alignment_vector += (view_rad - distance) * other;
			i++;
		}

		// nearby sharks (layer 10)
		Vector3 shark_vector = Vector3.zero;
		Collider[] sharks = neighborsByDistance (10, view_rad);
		i = 0;
		I = Math.Min (k_shark, sharks.Length);
		while (i < I) {
			Vector3 other = transform.position - sharks [i].transform.position;
			float distance = other.magnitude;
			other.Normalize ();
			shark_vector += (view_rad - distance) * other;
			i++;
		}

		// nearby walls (layer 8)
		Vector3 wall_vector = Vector3.zero;
		Collider[] walls = Physics.OverlapSphere (transform.position, wall_distance, 1 << 8); // 8 is wall layer...
		i = 0;
		while (i < walls.Length) {
			Vector3 other = transform.position - walls [i].ClosestPoint (transform.position);
			float distance = other.magnitude;
			other.Normalize ();
			wall_vector += (wall_distance - distance) * other;
			i++;
		}

		cohesion_vector.Normalize ();
		separation_vector.Normalize ();
		alignment_vector.Normalize ();
		wall_vector.Normalize ();
		shark_vector.Normalize ();

		dir = (dir + weight ["Cohesion"] * cohesion_vector +
			weight ["Separation"] * separation_vector +
			weight ["Alignment"] * alignment_vector +
			weight ["Wall"] * wall_vector +
			weight["Shark"] * shark_vector +
			UnityEngine.Random.onUnitSphere * chaos);
		dir.Normalize();
		transform.position += speed * dir;

		Collider[] debugging = neighborsByDistance (10, (float) 1);
		if (debugging.Length > 1) {
			Debug.Log (debugging.Length);
		}
	}

	Collider[] neighborsByDistance (int layer, float rad) // up to k nearest neighbors in specified layer (8:wall, 9:fish, ...) within rad distance
	{
		int layer_mask = 1 << layer;
		Collider[] neighbors = Physics.OverlapSphere (transform.position, view_rad, layer_mask);
		int I = neighbors.Length;
		float[] distances = new float[neighbors.Length];
		int i = 0;
		while (i < I) {
			distances [i] = (transform.position - neighbors [i].transform.position).magnitude;
			i++;
		}
		Array.Sort (distances, neighbors);
		return neighbors;
	}
}
