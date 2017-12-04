using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shark_Spawner : MonoBehaviour {

	public GameObject shark;
	private GameObject clone;
	private Shark_Behavior behavior;

	public int initial_sharks;
	public float spawn_sphere_rad;

	private int total_fish_eaten;
	private int n_sharks_alive;

	void Start () {
		n_sharks_alive = 0;
		total_fish_eaten = 0;
		int i = 0;
		while (i < initial_sharks) {
			spawn (UnityEngine.Random.onUnitSphere * spawn_sphere_rad, UnityEngine.Random.onUnitSphere, 0);
			i++;
		}
	}
		
	void Update () {

	}

	public void spawn(Vector3 position, Vector3 direction, int fat) {
		clone = Instantiate (shark);
		behavior = clone.GetComponent<Shark_Behavior> ();
		behavior.setMaster (this.gameObject);
		clone.transform.parent = transform;
		behavior.dir = direction;
		clone.transform.position = position;
		if (fat > 0) {
			clone.GetComponent<Shark_Behavior> ().set_health (fat);
		}
		addShark ();
	}

	public void addFish () {
		total_fish_eaten++;
	}

	public void loseShark () {
		n_sharks_alive -= 1;
		display ();
	}

	public void display () {
		Debug.Log (string.Format ("Sharks Alive: {0} | Total Fish Eaten: {1}", n_sharks_alive, total_fish_eaten));
	}

	void addShark () {
		n_sharks_alive += 1;
		display ();
	}
}
