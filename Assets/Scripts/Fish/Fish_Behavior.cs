using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fish_Behavior : MonoBehaviour {

	// view, speed, randomness, reproduction...
	public float view_rad;
	public float max_turn_degrees;
	private float max_turn_rad;
	public float speed;
	public float chaos;
	public int reproduction_threshold;
	private int fat;
	private int food_eaten;
	private int offspring;
	public int max_hp;
	public int hp_lost_per_frame;
	public int hp_gained_for_eating;
	private int hp;

	// weights and max distances for stimuli
	public float wall_weight;
	public float cohesion_weight;
	public float separation_weight;
	public float alignment_weight;
	public float shark_weight;
	public float food_weight;
	public float dodge_weight;
	public float separation_distance;
	public float wall_distance;
	private GameObject master;
	private Master_Behavior spawner;
	public int collision_penalty;

	// spawn location max distance from origin, direction, soft max distance from origin
	public Vector3 dir;
	public float soft_max_distance;
	public float soft_boundary_weight;

	// number of nearest neighbors
	public int k_fish;
	public int k_shark;
	public int k_food;


	void Start () {

		hp = max_hp;
		offspring = 0;
		food_eaten = 0;
		fat = 0;

		max_turn_rad = (float) (Math.PI * max_turn_degrees / 180);
	}

	public void set_max_turn_rad () {
		max_turn_rad = (float) (Math.PI * max_turn_degrees / 180);
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
		Vector3 dodge_vector = Vector3.zero;
		Collider[] sharks = neighborsByDistance (10, view_rad);
		i = 0;
		I = Math.Min (k_shark, sharks.Length);
		while (i < I) {
			// run
			Vector3 other = transform.position - sharks [i].transform.position;
			float distance = other.magnitude;
			other.Normalize ();
			float coeff = view_rad - distance;
			shark_vector += coeff * other;
			// dodge
			Vector3 shark_dir = sharks [i].GetComponent<Shark_Behavior> ().dir;
			if (Vector3.Dot (other, shark_dir) > .5) {
				Vector3 e = Vector3.Cross (shark_dir, other);
				other = coeff * Vector3.Cross (e, shark_dir).normalized;
				dodge_vector += other;

			}
				
			i++;
		}

		// nearby food (layer 11)
		Vector3 food_vector = Vector3.zero;
		Collider[] food = neighborsByDistance (11, view_rad);
		i = 0;
		I = Math.Min (k_food, food.Length);
		while (i < I) {
			Vector3 other = food [i].transform.position - transform.position;
			float distance = other.magnitude;
			other.Normalize ();
			food_vector += (view_rad - distance) * other;
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
		food_vector.Normalize ();
		dodge_vector.Normalize ();

		Vector3 desired = (dir + cohesion_weight * cohesion_vector +
		                  separation_weight * separation_vector +
		                  alignment_weight * alignment_vector +
		                  wall_weight * wall_vector +
		                  shark_weight * shark_vector +
		                  dodge_weight * dodge_vector +
		                  food_weight * food_vector +
		                  UnityEngine.Random.onUnitSphere * chaos);
		
		float from_origin = transform.position.magnitude;
		if (from_origin > soft_max_distance) {
			Vector3	other = transform.position.normalized;
			desired -= soft_boundary_weight * (from_origin - soft_max_distance) * other;
		}

		desired.Normalize();

		dir = rotateToward (dir, desired, max_turn_rad);
		transform.position += speed * dir;

		hp -= hp_lost_per_frame;
		if (hp <= 0) {
			burn_fat ();
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


	void OnTriggerEnter (Collider other)
	{
		switch (other.gameObject.tag) {

		case "Food":
			eat (other.gameObject);
			break;

		case "Fish":
			collide ();
			break;

		default:
			break;
		}
	}

	public void collide () {
		hp -= collision_penalty;
		Debug.Log ("Fish Collision");
	}
		
	public void setMaster (GameObject Master) {
		master = Master;
		spawner = master.GetComponent<Master_Behavior> ();
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

	public void die () {
		spawner.killFish (this.gameObject);
	}

	public void eat (GameObject food) {
		spawner.killFood (food);
		food_eaten++;
		hp += hp_gained_for_eating;
		if (hp > max_hp) {
			fat += hp - max_hp;
			hp = max_hp;
		}
		if (fat >= reproduction_threshold) {
			spawner.spawnFish (this);
			offspring++;
			fat = 0;
		}
	}

	public void set_health(int health) {
		hp = health;
	}

	public void burn_fat () {
		hp += fat;
		if (hp <= 0) {
			die ();
		}
		fat = 0;
	}

	public int total_resources () {
		return hp + fat;
	}

	public int get_fat() {
		return fat;
	}
}
