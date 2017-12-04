using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fish_Spawner : MonoBehaviour {

	public GameObject fish;
	private GameObject clone;
	private Fish_Behavior behavior;

	public int initial_fish;
	public float spawn_sphere_rad;
	public int n_fish_alive;
	private int total_food_eaten;

	void Start () {
		n_fish_alive = 0;
		total_food_eaten = 0;
		int i = 0;
		while (i < initial_fish) {
			spawn (UnityEngine.Random.insideUnitSphere * spawn_sphere_rad, UnityEngine.Random.onUnitSphere, 0);
			i++;
		}
	}

	void Update () {
		
	}

	public void spawn(Vector3 position, Vector3 direction, int fat) {
		clone = Instantiate (fish);
		behavior = clone.GetComponent<Fish_Behavior> ();
		behavior.setMaster (this.gameObject);
		clone.transform.parent = transform;
		behavior.dir = direction;
		clone.transform.position = position;
		if (fat > 0) {
			clone.GetComponent<Fish_Behavior> ().set_health (fat);
		}
		addFish ();
	}

	void display () {
		Debug.Log (string.Format ("Fish Alive: {0} | Total Food Eaten: {1}", n_fish_alive, total_food_eaten));
	}

	public void loseFish () {
		n_fish_alive -= 1;
		display ();
	}

	public void addFish () {
		n_fish_alive += 1;
		display ();
	}

	public void addFood () {
		total_food_eaten++;
	}
}
