using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shark_Behavior : MonoBehaviour {

	// view, speed, max angular change per frame, randomness
	public float view_rad;
	public float speed;
	public float chaos;
	public float max_degrees;
	private float max_rad;

	// weights and max distances for stimuli
	public float wall_weight;
	public float separation_weight;
	public float fish_weight;
	public float wall_distance;
	public float separation_distance;
	private Dictionary<string, float> weight;

	// spawn location max distance from origin, direction
	public float spawn_sphere_rad;
	public Vector3 dir;
	public float soft_max_distance;
	public float soft_boundary_weight;

	// number of fish to consider
	public int k_fish;


	void Start () {
		weight = new Dictionary<string, float> ();
		weight.Add ("Wall", wall_weight);
		weight.Add ("Separation", separation_weight);
		weight.Add ("Fish", fish_weight);
		weight.Add ("Soft", soft_boundary_weight);

		dir = UnityEngine.Random.onUnitSphere;
		transform.position = UnityEngine.Random.onUnitSphere * spawn_sphere_rad;
		max_rad = (float) (Math.PI * max_degrees / 180);
	}


	void Update () {

		// nearby fish (layer 9)
		Vector3 fish_vector = Vector3.zero;
		Vector3 alignment_vector = Vector3.zero;
		Collider[] food = neighborsByDistance (9, view_rad);
		int i = 0;
		int I = Math.Min (k_fish, food.Length);
		while (i < I) {
			Vector3 other = food [i].transform.position - transform.position;
			float distance = other.magnitude;
			other.Normalize();
			fish_vector += (view_rad - distance) * other;
			i++;
		}

		// nearby sharks (layer 10)
		Vector3 separation_vector = Vector3.zero;
		Collider[] neighbors = neighborsByDistance (10, separation_distance);
		i = 0;
		while (i < neighbors.Length) {
			Vector3 other = transform.position - neighbors [i].transform.position;
			float distance = other.magnitude;
			other.Normalize ();
			if (distance < separation_distance) {
				separation_vector += (separation_distance - distance) * other;
			}
			i++;
		}

		Vector3 wall_vector = Vector3.zero;

		// nearby walls (layer 8)
		Collider[] walls = Physics.OverlapSphere (transform.position, wall_distance, 1 << 8); // 8 is wall layer...
		i = 0;
		while (i < walls.Length) {
			Vector3 other = transform.position - walls [i].ClosestPoint (transform.position);
			float distance = other.magnitude;
			other.Normalize ();
			wall_vector += (wall_distance - distance) * other;
			i++;
		}
		
		separation_vector.Normalize ();
		fish_vector.Normalize ();
		wall_vector.Normalize ();

		Vector3 desired = (dir +
			weight ["Separation"] * separation_vector +
			weight ["Fish"] * fish_vector +
			weight ["Wall"] * wall_vector +
			UnityEngine.Random.onUnitSphere * chaos);

		float from_origin = transform.position.magnitude;
		if (from_origin > soft_max_distance) {
			Vector3	other = transform.position.normalized;
			desired -= weight ["Soft"] * (from_origin - soft_max_distance) * other;
		}

		desired.Normalize();

		dir = rotateToward (dir, desired, max_rad);
		transform.position += speed * dir;
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


	void OnTriggerEnter (Collider other)
	{
		switch (other.gameObject.tag) {

		case "Fish":
			other.gameObject.SetActive (false);
			Debug.Log ("Fish eaten");
			break;

		default:
			break;
		}
	}

	Vector3 rotateToward (Vector3 start, Vector3 target, float max_rad) // assumes both are unit vectors
	{
		float cos = Vector3.Dot (start, target);
		float Cos = (float) Math.Cos (max_rad);
		if (cos >= Cos) {
			return target;
		}
		float Sin = (float) Math.Sin (max_rad);
		Vector3 e = Vector3.Cross (start, target);
		e.Normalize ();
		return Cos * start + Sin * Vector3.Cross (e, start) + (1 - Cos) * Vector3.Dot (e, start) * e;
	}
}
