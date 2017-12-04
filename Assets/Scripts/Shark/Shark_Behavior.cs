using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shark_Behavior : MonoBehaviour {

	// view, speed, max angular change per frame, randomness, reproduction...
	public float view_rad;
	public float speed;
	public float chaos;
	public float max_turn_degrees;
	private float max_turn_rad;
	private int fat;
	private int food_eaten;
	public int reproduction_threshold;
	private int offspring;
	public int max_hp;
	public int hp_lost_per_frame;
	private int hp;
	public int collision_penalty;
	public float size;

	// weights and max distances for stimuli
	public float wall_weight;
	public float separation_weight;
	public float fish_weight;
	public float wall_distance;
	public float separation_distance;
	private GameObject master;
	private Master_Behavior spawner;

	// spawn location max distance from origin, direction
	public Vector3 dir;
	public float soft_max_distance;
	public float soft_boundary_weight;

	// number of fish to consider
	public int k_fish;


	void Start () {

		this.gameObject.transform.localScale = new Vector3 (size, size, size);

		food_eaten = 0;
		fat = 0;
		hp = (int)max_hp/2;
		offspring = 0;


		max_turn_rad = (float) (Math.PI * max_turn_degrees / 180);
	}

	public void set_max_turn_rad () {
		max_turn_rad = (float) (Math.PI * max_turn_degrees / 180);
	}

	public void set_size (float s) {
		size = s;
		this.gameObject.transform.localScale = new Vector3 (s, s, s);
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
			separation_weight * separation_vector +
			fish_weight * fish_vector +
			wall_weight * wall_vector +
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

		case "Fish":
			eat (other.gameObject);
			break;

		case "Shark":
			collide ();
			break;

		default:
			break;
		}
	}

	public void collide () {
		hp -= collision_penalty;
		Debug.Log ("Shark Collision");
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

	public void burn_fat () {
		hp += fat;
		if (hp <= 0) {
			die ();
		}
		fat = 0;
	}

	public void die () {
		spawner.killShark (this.gameObject);
	}

	public void eat (GameObject fish) {
		hp += fish.GetComponent<Fish_Behavior>().total_resources ();
		spawner.killFish (fish);
		food_eaten++;
		if (hp > max_hp) {
			fat += hp - max_hp;
			hp = max_hp;
		}
		if (fat >= reproduction_threshold) {
			spawner.spawnShark (this);
			offspring++;
			fat = 0;
		}
		//Debug.Log (string.Format ("{0}, {1}, {2}", this.gameObject.GetInstanceID (), hp, fat));
	}

	public void set_health(int health) {
		hp = health;
	}

	public int get_hp() {
		return hp;
	}

	public int get_fat () {
		return fat;
	}


}
