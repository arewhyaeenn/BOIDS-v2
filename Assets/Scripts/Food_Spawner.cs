using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Food_Spawner : MonoBehaviour {

	public float spawn; // expected # spawned in frame, if <1 then chance to spawn 1 in frame
	private int iter;
	public GameObject food;
	public GameObject clone;

	void Start () {
		iter = 1;
		while (spawn >= 1) {
			spawn /= 10;
			iter *= 10;
		}
	}
	

	void Update () {
		int i = 0;
		while (i < iter) {
			float sample = Random.Range (0f, spawn);
			if (sample <= spawn) {
				clone = Instantiate (food, Random.insideUnitSphere, transform.rotation);
				clone.transform.parent = transform;
			}
			i++;
		}
	}
}
