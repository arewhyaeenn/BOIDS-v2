using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.IO;
using System.Text;

public class Master_Behavior : MonoBehaviour {

	private GameObject clone;

	public GameObject fish;
	private Fish_Behavior fish_behavior;
	public float fish_spawn_rad;
	private List<GameObject> fishes;

	public GameObject shark;
	private Shark_Behavior shark_behavior;
	public float shark_spawn_rad;
	private List<GameObject> sharks;

	public GameObject food;
	private Food_Behavior food_behavior;
	public float food_spawn_rad;
	private List<GameObject> foods;

	public int initial_fish;
	public int initial_sharks;
	private int food_iter=1;
	public float food_spawn_rate;

	private GameObject fish_parent;
	private GameObject shark_parent;
	private GameObject food_parent;

	public float mean_shark_speed;
	public float sd_shark_speed;
	public float delta_shark_speed;

	public float mean_shark_ang;
	public float sd_shark_ang;
	public float delta_shark_ang;

	public float mean_shark_size;
	public float sd_shark_size;
	public float delta_shark_size;

	public float mean_shark_fish_weight;
	public float sd_shark_fish_weight;
	public float delta_shark_fish_weight;

	public float mean_shark_separation_weight;
	public float sd_shark_separation_weight;
	public float delta_shark_separation_weight;

	public float mean_fish_speed;
	public float sd_fish_speed;
	public float delta_fish_speed;

	public float mean_fish_ang;
	public float sd_fish_ang;
	public float delta_fish_ang;

	public float mean_fish_cohesion_weight;
	public float sd_fish_cohesion_weight;
	public float delta_fish_cohesion_weight;

	public float mean_fish_separation_weight;
	public float sd_fish_separation_weight;
	public float delta_fish_separation_weight;

	public float mean_fish_alignment_weight;
	public float sd_fish_alignment_weight;
	public float delta_fish_alignment_weight;

	public float mean_fish_shark_weight;
	public float sd_fish_shark_weight;
	public float delta_fish_shark_weight;

	public float mean_fish_dodge_weight;
	public float sd_fish_dodge_weight;
	public float delta_fish_dodge_weight;

	public float mean_fish_food_weight;
	public float sd_fish_food_weight;
	public float delta_fish_food_weight;

	public int shark_reproduction_method; // 0 for identical, 1 for gaussian
	public int fish_reproduction_method;
	public float shark_mean_adaptation_rate;
	public float fish_mean_adaptation_rate;

	string filepath;
	string dt;
	private decimal time = 0;

	private float mfs = 0;
	private float mfa = 0;
	private float mfcw = 0;
	private float mfaw = 0;
	private float mfsew = 0;
	private float mfshw = 0;
	private float mfdw = 0;
	private float mffw = 0;

	private float mss = 0;
	private float msa = 0;
	private float mssw = 0;
	private float msfw = 0;
	private float mssize = 0;

	void Start () {

		filepath = Application.dataPath + "/Data/idlist.txt";
		dt = System.DateTime.Now.ToString("MMddyyyyHHmmss");
		using (StreamWriter sw = File.AppendText (filepath)) {
			sw.Write (string.Format ("{0}\n", dt));
			sw.Close ();
		}

		filepath = Application.dataPath + "/Data/" + dt + "-info.csv";
		using (FileStream fs = File.Create (filepath)) {
			Byte[] info = new UTF8Encoding (true).GetBytes (string.Format (
				              "id,if,is,fsr," +
				              "frm,srm,far,sar," +
				              "mfs,sdfs,dfs," +
				              "mfa,sdfa,dfa," +
				              "mfcw,sdfcw,dfcw," +
				              "mfaw,sdfaw,dfaw," +
				              "mfsew,sdfsew,dfsew," +
				              "mfshw,sdfshw,dfshw," +
				              "mfdw,sdfdw,dfdw," +
				              "mffw,sdffw,dffw," +
				              "mss,sdss,dss," +
				              "msa,sdsa,dsa," +
				              "mssw,sdssw,dssw," +
				              "msfw,sdsfw,dsfw," +
				              "mssize,sdssize,dssize" +
				              "\n{0},{1},{2},{3}," +
				              "{4},{5},{6},{7}," +
				              "{8},{9},{10}," +
				              "{11},{12},{13}," +
				              "{14},{15},{16}," +
				              "{17},{18},{19}," +
				              "{20},{21},{22}," +
				              "{23},{24},{25}," +
				              "{26},{27},{28}," +
				              "{29},{30},{31}," +
				              "{32},{33},{34}," +
				              "{35},{36},{37}," +
				              "{38},{39},{40}," +
				              "{41},{42},{43}," +
				              "{44},{45},{46}",
				              dt, initial_fish, initial_sharks, food_spawn_rate,
				              fish_reproduction_method, shark_reproduction_method, fish_mean_adaptation_rate, shark_mean_adaptation_rate,
				              mean_fish_speed, sd_fish_speed, delta_fish_speed,
				              mean_fish_ang, sd_fish_ang, delta_fish_ang,
				              mean_fish_cohesion_weight, sd_fish_cohesion_weight, delta_fish_cohesion_weight,
				              mean_fish_alignment_weight, sd_fish_alignment_weight, delta_fish_alignment_weight,
				              mean_fish_separation_weight, sd_fish_separation_weight, delta_fish_separation_weight,
				              mean_fish_shark_weight, sd_fish_shark_weight, delta_fish_shark_weight,
				              mean_fish_dodge_weight, sd_fish_dodge_weight, delta_fish_dodge_weight,
				              mean_fish_food_weight, sd_fish_food_weight, delta_fish_food_weight,
				              mean_shark_speed, sd_shark_speed, delta_shark_speed,
				              mean_shark_ang, sd_shark_ang, delta_shark_ang,
				              mean_shark_separation_weight, sd_shark_separation_weight, delta_shark_separation_weight,
				              mean_shark_fish_weight, sd_shark_fish_weight, delta_shark_fish_weight,
				              mean_shark_size, sd_shark_size, delta_shark_size));
			fs.Write (info, 0, info.Length);
			fs.Close ();
		}

		filepath = Application.dataPath + "/Data/"+ dt + "-data.csv";
		Debug.Log (filepath);
		using (FileStream fs = File.Create (filepath)) {
			Byte[] info = new UTF8Encoding (true).GetBytes (string.Format (
				              "time,nf,ns,fs,fa,fcw,faw,fsew,fshw,fdw,ffw,ss,sa,ssw,sfw,ssize\n"));
			fs.Write (info, 0, info.Length);
			fs.Close ();
		}

		Vector3 gtfo_of_the_way = new Vector3 (500, 0, 0);
		fish_parent = new GameObject ("Fish");
		fish_parent.transform.parent = transform;
		fish_parent.transform.position = gtfo_of_the_way;
		shark_parent = new GameObject ("Shark");
		shark_parent.transform.parent = transform;
		shark_parent.transform.position = gtfo_of_the_way;
		food_parent = new GameObject ("Food");
		food_parent.transform.parent = transform;
		food_parent.transform.position = gtfo_of_the_way;

		fishes = new List<GameObject>();
		sharks = new List<GameObject> ();
		foods = new List<GameObject> ();

		while (food_spawn_rate > 1) {
			food_spawn_rate /= 10;
			food_iter *= 10;
		}
		int i = 0;
		while (i < initial_fish) {
			spawnFish ();
			i++;
		}
		i = 0;
		while (i < initial_sharks) {
			spawnShark ();
			i++;
		}
	}

	void Update () {
		int i = 0;
		while (i < food_iter) {
			float sample = UnityEngine.Random.Range (0f, 1f);
			if (sample <= food_spawn_rate) {
				spawnFood ();
			}
			i++;
		}
		display (); // do not comment out, only call to get averages
		using (StreamWriter sw = File.AppendText (filepath)) {
			sw.Write (string.Format ("{0},{1},{2},{3},{4},{5},{6},{7}," +
				"{8},{9},{10},{11},{12},{13},{14},{15}\n",
				time, fishes.Count, sharks.Count,
				mfs, mfa, mfcw, mfaw, mfsew, mfshw, mfdw, mffw,
				mss, msa, mssw, msfw,mssize));
		}
		time += (decimal) 0.01;
	}

	public void spawnFish (Fish_Behavior parent = null) {
		
		clone = Instantiate (fish);
		clone.transform.parent = fish_parent.transform;
		fish_behavior = clone.GetComponent<Fish_Behavior> ();
		fish_behavior.setMaster (this.gameObject);
		fishes.Add (clone);

		if (parent == null) {
			clone.transform.position = UnityEngine.Random.insideUnitSphere * fish_spawn_rad;
			fish_behavior.dir = UnityEngine.Random.onUnitSphere;
			fish_behavior.speed = mean_fish_speed + delta_fish_speed * randStdNormal ();
			fish_behavior.max_turn_degrees = mean_fish_ang + delta_fish_ang * randStdNormal ();
			fish_behavior.cohesion_weight = mean_fish_cohesion_weight + delta_fish_cohesion_weight * randStdNormal ();
			fish_behavior.alignment_weight = mean_fish_alignment_weight + delta_fish_alignment_weight * randStdNormal ();
			fish_behavior.separation_weight = mean_fish_separation_weight + delta_fish_separation_weight * randStdNormal ();
			fish_behavior.shark_weight = mean_fish_shark_weight + delta_fish_shark_weight * randStdNormal ();
			fish_behavior.dodge_weight = mean_fish_dodge_weight + delta_fish_dodge_weight * randStdNormal ();
			fish_behavior.food_weight = mean_fish_food_weight + delta_fish_food_weight * randStdNormal ();
			fish_behavior.set_max_turn_rad ();
		} else {
			clone.transform.position = parent.gameObject.transform.position;
			fish_behavior.dir = parent.dir;
			int fat = parent.get_fat ();
			fish_behavior.set_health (fat);
			if (fish_reproduction_method == 0) {
				fish_behavior.speed = parent.speed;
				fish_behavior.max_turn_degrees = parent.max_turn_degrees;
				fish_behavior.cohesion_weight = parent.cohesion_weight;
				fish_behavior.alignment_weight = parent.alignment_weight;
				fish_behavior.separation_weight = parent.separation_weight;
				fish_behavior.shark_weight = parent.shark_weight;
				fish_behavior.dodge_weight = parent.dodge_weight;
				fish_behavior.food_weight = parent.food_weight;
				fish_behavior.set_max_turn_rad ();
			} else if (fish_reproduction_method == 1) {
				
				float s = mean_fish_speed + sd_fish_speed * randStdNormal ();
				if (s > parent.speed) {
					fish_behavior.speed = parent.speed + delta_fish_speed;
				} else if (s < parent.speed) {
					fish_behavior.speed = parent.speed - delta_fish_speed;
				} else {
					fish_behavior.speed = parent.speed;
				}

				s = mean_fish_ang + sd_fish_ang * randStdNormal ();
				if (s > parent.max_turn_degrees) {
					fish_behavior.max_turn_degrees = parent.max_turn_degrees + delta_fish_ang;
				} else if (s < parent.max_turn_degrees) {
					fish_behavior.max_turn_degrees = parent.max_turn_degrees - delta_fish_ang;
				} else {
					fish_behavior.max_turn_degrees = parent.max_turn_degrees;
				}

				s = mean_fish_cohesion_weight + sd_fish_cohesion_weight * randStdNormal ();
				if (s > parent.cohesion_weight) {
					fish_behavior.cohesion_weight = parent.cohesion_weight + delta_fish_cohesion_weight;
				} else if (s < parent.cohesion_weight) {
					fish_behavior.cohesion_weight = parent.cohesion_weight - delta_fish_cohesion_weight;
				} else {
					fish_behavior.cohesion_weight = parent.cohesion_weight;
				}

				s = mean_fish_alignment_weight + sd_fish_alignment_weight * randStdNormal ();
				if (s > parent.alignment_weight) {
					fish_behavior.alignment_weight = parent.alignment_weight + delta_fish_alignment_weight;
				} else if (s < parent.alignment_weight) {
					fish_behavior.alignment_weight = parent.alignment_weight - delta_fish_alignment_weight;
				} else {
					fish_behavior.alignment_weight = parent.alignment_weight;
				}

				s = mean_fish_separation_weight + sd_fish_separation_weight * randStdNormal ();
				if (s > parent.separation_weight) {
					fish_behavior.separation_weight = parent.separation_weight + delta_fish_separation_weight;
				} else if (s < parent.separation_weight) {
					fish_behavior.separation_weight = parent.separation_weight - delta_fish_separation_weight;
				} else {
					fish_behavior.separation_weight = parent.separation_weight;
				}

				s = mean_fish_shark_weight + sd_fish_shark_weight * randStdNormal ();
				if (s > parent.shark_weight) {
					fish_behavior.shark_weight = parent.shark_weight + delta_fish_shark_weight;
				} else if (s < parent.shark_weight) {
					fish_behavior.shark_weight = parent.shark_weight - delta_fish_shark_weight;
				} else {
					fish_behavior.shark_weight = parent.shark_weight;
				}

				s = mean_fish_dodge_weight + sd_fish_dodge_weight * randStdNormal ();
				if (s > parent.dodge_weight) {
					fish_behavior.dodge_weight = parent.dodge_weight + delta_fish_dodge_weight;
				} else if (s < parent.dodge_weight) {
					fish_behavior.dodge_weight = parent.dodge_weight - delta_fish_dodge_weight;
				} else {
					fish_behavior.dodge_weight = parent.dodge_weight;
				}

				s = mean_fish_food_weight + sd_fish_food_weight * randStdNormal ();
				if (s > parent.food_weight) {
					fish_behavior.food_weight = parent.food_weight + delta_fish_food_weight;
				} else if (s < parent.food_weight) {
					fish_behavior.food_weight = parent.food_weight - delta_fish_food_weight;
				} else {
					fish_behavior.food_weight = parent.food_weight;
				}
				fish_behavior.set_max_turn_rad ();
			}
		}
	}

	public void spawnShark (Shark_Behavior parent = null) {
		
		clone = Instantiate (shark);
		clone.transform.parent = shark_parent.transform;
		shark_behavior = clone.GetComponent<Shark_Behavior> ();
		shark_behavior.setMaster (this.gameObject);
		sharks.Add (clone);

		if (parent == null) {
			clone.transform.position = UnityEngine.Random.onUnitSphere * shark_spawn_rad;
			shark_behavior.dir = UnityEngine.Random.onUnitSphere;
			shark_behavior.speed = mean_shark_speed + delta_shark_speed * randStdNormal ();
			shark_behavior.max_turn_degrees = mean_shark_ang + delta_shark_ang * randStdNormal ();
			shark_behavior.separation_weight = mean_shark_separation_weight + delta_shark_separation_weight * randStdNormal ();
			shark_behavior.fish_weight = mean_shark_fish_weight + delta_shark_fish_weight * randStdNormal ();
			float size = mean_shark_size + sd_shark_size * randStdNormal ();
			shark_behavior.set_size (size);
		} else {
			clone.transform.position = parent.gameObject.transform.position;
			shark_behavior.dir = parent.dir;
			int fat = parent.get_fat ();
			shark_behavior.set_health (fat);
			if (shark_reproduction_method == 0) {
				shark_behavior.speed = parent.speed;
				shark_behavior.max_turn_degrees = parent.max_turn_degrees;
				shark_behavior.separation_weight = parent.separation_weight;
				shark_behavior.fish_weight = parent.fish_weight;
			} else if (shark_reproduction_method == 1) {
				
				float s = mean_shark_speed + sd_shark_speed * randStdNormal ();
				if (s > parent.speed) {
					shark_behavior.speed = parent.speed + delta_shark_speed;
				} else if (s < parent.speed) {
					shark_behavior.speed = parent.speed - delta_shark_speed;
				} else {
					shark_behavior.speed = parent.speed;
				}

				s = mean_shark_ang + sd_shark_ang * randStdNormal ();
				if (s > parent.max_turn_degrees) {
					shark_behavior.max_turn_degrees = parent.max_turn_degrees + delta_shark_ang;
				} else if (s < parent.max_turn_degrees) {
					shark_behavior.max_turn_degrees = parent.max_turn_degrees - delta_shark_ang;
				} else {
					shark_behavior.max_turn_degrees = parent.max_turn_degrees;
				}

				s = mean_shark_separation_weight + sd_shark_separation_weight * randStdNormal ();
				if (s > parent.separation_weight) {
					shark_behavior.separation_weight = parent.separation_weight + delta_shark_separation_weight;
				} else if (s < parent.separation_weight) {
					shark_behavior.separation_weight = parent.separation_weight - delta_shark_separation_weight;
				} else {
					shark_behavior.separation_weight = parent.separation_weight;
				}

				s = mean_shark_fish_weight + sd_shark_fish_weight * randStdNormal ();
				if (s > parent.fish_weight) {
					shark_behavior.fish_weight = parent.fish_weight + delta_shark_fish_weight;
				} else if (s < parent.fish_weight) {
					shark_behavior.fish_weight = parent.fish_weight - delta_shark_fish_weight;
				} else {
					shark_behavior.fish_weight = parent.fish_weight;
				}

				s = mean_shark_size + sd_shark_size * randStdNormal ();
				if (s > parent.size) {
					shark_behavior.set_size (parent.size + delta_shark_size);
				} else if (s < parent.size) {
					shark_behavior.set_size (parent.size - delta_shark_size);
				} else {
					shark_behavior.set_size (parent.size);
				}
			}
		}
	}

	void spawnFood () {
		clone = Instantiate (food, UnityEngine.Random.insideUnitSphere * food_spawn_rad, transform.rotation);
		clone.transform.parent = food_parent.transform;
		foods.Add (clone);
	}

	public void killFish(GameObject dead_fish) {
		fishes.Remove (dead_fish);
		Destroy (dead_fish);
		if (fishes.Count == 0) {
			filepath = Application.dataPath + "/Data/" + dt + "-info.csv";
			using (StreamWriter sw = File.AppendText (filepath)) {
				sw.Write (string.Format (
					"\n{0},{1},{2},{3}," +
					"{4},{5},{6},{7}," +
					"{8},{9},{10}," +
					"{11},{12},{13}," +
					"{14},{15},{16}," +
					"{17},{18},{19}," +
					"{20},{21},{22}," +
					"{23},{24},{25}," +
					"{26},{27},{28}," +
					"{29},{30},{31}," +
					"{32},{33},{34}," +
					"{35},{36},{37}," +
					"{38},{39},{40}," +
					"{41},{42},{43}," +
					"{44},{45},{46}",
					dt, initial_fish, initial_sharks, food_spawn_rate,
					fish_reproduction_method, shark_reproduction_method, fish_mean_adaptation_rate, shark_mean_adaptation_rate,
					mean_fish_speed, sd_fish_speed, delta_fish_speed,
					mean_fish_ang, sd_fish_ang, delta_fish_ang,
					mean_fish_cohesion_weight, sd_fish_cohesion_weight, delta_fish_cohesion_weight,
					mean_fish_alignment_weight, sd_fish_alignment_weight, delta_fish_alignment_weight,
					mean_fish_separation_weight, sd_fish_separation_weight, delta_fish_separation_weight,
					mean_fish_shark_weight, sd_fish_shark_weight, delta_fish_shark_weight,
					mean_fish_dodge_weight, sd_fish_dodge_weight, delta_fish_dodge_weight,
					mean_fish_food_weight, sd_fish_food_weight, delta_fish_food_weight,
					mean_shark_speed, sd_shark_speed, delta_shark_speed,
					mean_shark_ang, sd_shark_ang, delta_shark_ang,
					mean_shark_separation_weight, sd_shark_separation_weight, delta_shark_separation_weight,
					mean_shark_fish_weight, sd_shark_fish_weight, delta_shark_fish_weight,
					mean_shark_size, sd_shark_size, delta_shark_size));
				sw.Close ();
			}
			Debug.Break();
		}
	}

	public void killShark(GameObject dead_shark) {
		sharks.Remove (dead_shark);
		Destroy (dead_shark);
		if (sharks.Count == 0 && initial_sharks != 0) {
			filepath = Application.dataPath + "/Data/" + dt + "-info.csv";
			using (StreamWriter sw = File.AppendText (filepath)) {
				sw.Write (string.Format (
					"\n{0},{1},{2},{3}," +
					"{4},{5},{6},{7}," +
					"{8},{9},{10}," +
					"{11},{12},{13}," +
					"{14},{15},{16}," +
					"{17},{18},{19}," +
					"{20},{21},{22}," +
					"{23},{24},{25}," +
					"{26},{27},{28}," +
					"{29},{30},{31}," +
					"{32},{33},{34}," +
					"{35},{36},{37}," +
					"{38},{39},{40}," +
					"{41},{42},{43}," +
					"{44},{45},{46}",
					dt, initial_fish, initial_sharks, food_spawn_rate,
					fish_reproduction_method, shark_reproduction_method, fish_mean_adaptation_rate, shark_mean_adaptation_rate,
					mean_fish_speed, sd_fish_speed, delta_fish_speed,
					mean_fish_ang, sd_fish_ang, delta_fish_ang,
					mean_fish_cohesion_weight, sd_fish_cohesion_weight, delta_fish_cohesion_weight,
					mean_fish_alignment_weight, sd_fish_alignment_weight, delta_fish_alignment_weight,
					mean_fish_separation_weight, sd_fish_separation_weight, delta_fish_separation_weight,
					mean_fish_shark_weight, sd_fish_shark_weight, delta_fish_shark_weight,
					mean_fish_dodge_weight, sd_fish_dodge_weight, delta_fish_dodge_weight,
					mean_fish_food_weight, sd_fish_food_weight, delta_fish_food_weight,
					mean_shark_speed, sd_shark_speed, delta_shark_speed,
					mean_shark_ang, sd_shark_ang, delta_shark_ang,
					mean_shark_separation_weight, sd_shark_separation_weight, delta_shark_separation_weight,
					mean_shark_fish_weight, sd_shark_fish_weight, delta_shark_fish_weight,
					mean_shark_size, sd_shark_size, delta_shark_size));
				sw.Close ();
			}
			Debug.Break();
		}
	}

	public void killFood(GameObject dead_food) {
		foods.Remove (dead_food);
		Destroy (dead_food);
	}

	public void display () {
		get_means ();
		Debug.Log (string.Format ("Time: {0} ; Fish Speed: {1} ; Fish Mobility: {2}" +
		" ; Shark Speed: {3} ; Shark Mobility: {4}", time, mfs, mfa, mss, msa));
	}

	private float randStdNormal () {
		float x1 = 1.0f - UnityEngine.Random.Range (0.0f,0.99f);
		float x2 = 1.0f - UnityEngine.Random.Range (0.0f,0.99f);
		return Mathf.Sqrt(-2.0f * Mathf.Log(x1)) * Mathf.Sin(2.0f * Mathf.PI * x2);
	}

	private void get_means () {
		
		Fish_Behavior fb;
		Shark_Behavior sb;

		mfs = 0;
		mfa = 0;
		mfcw = 0;
		mfaw = 0;
		mfsew = 0;
		mfshw = 0;
		mfdw = 0;
		mffw = 0;

		int i = 0;
		while (i < fishes.Count) {
			fb = fishes [i].GetComponent<Fish_Behavior> ();
			mfs += fb.speed;
			mfa += fb.max_turn_degrees;
			mfcw += fb.cohesion_weight;
			mfaw += fb.alignment_weight;
			mfsew += fb.separation_weight;
			mfshw += fb.shark_weight;
			mfdw += fb.dodge_weight;
			mffw += fb.food_weight;
			i++;
		}
		if (i > 0) {
			mfs /= i;
			mfa /= i;
			mfcw /= i;
			mfaw /= i;
			mfsew /= i;
			mfshw /= i;
			mfdw /= i;
			mffw /= i;
		}

		mss = 0;
		msa = 0;
		mssw = 0;
		msfw = 0;
		mssize = 0;

		i = 0;
		while (i < sharks.Count) {
			sb = sharks [i].GetComponent<Shark_Behavior> ();
			mss += sb.speed;
			msa += sb.max_turn_degrees;
			mssw += sb.separation_weight;
			msfw += sb.fish_weight;
			mssize += sb.size;
			i++;
		}
		if (i > 0) {
			mss /= i;
			msa /= i;
			mssw /= i;
			msfw /= i;
			mssize /= i;
		}
			
		mean_fish_speed += fish_mean_adaptation_rate * (mfs - mean_fish_speed);
		mean_fish_ang += fish_mean_adaptation_rate * (mfa - mean_fish_ang);
		mean_fish_cohesion_weight += fish_mean_adaptation_rate * (mfcw - mean_fish_cohesion_weight);
		mean_fish_alignment_weight += fish_mean_adaptation_rate * (mfaw - mean_fish_alignment_weight);
		mean_fish_separation_weight += fish_mean_adaptation_rate * (mfsew - mean_fish_separation_weight);
		mean_fish_shark_weight += fish_mean_adaptation_rate * (mfshw - mean_fish_shark_weight);
		mean_fish_dodge_weight += fish_mean_adaptation_rate * (mfdw - mean_fish_dodge_weight);
		mean_fish_food_weight += fish_mean_adaptation_rate * (mffw - mean_fish_food_weight);

		mean_shark_speed += shark_mean_adaptation_rate * (mss - mean_shark_speed);
		mean_shark_ang += shark_mean_adaptation_rate * (msa - mean_shark_ang);
		mean_shark_separation_weight += shark_mean_adaptation_rate * (mssw - mean_shark_separation_weight);
		mean_shark_fish_weight += shark_mean_adaptation_rate * (msfw - mean_shark_fish_weight);
		mean_shark_size += shark_mean_adaptation_rate * (mssize - mean_shark_size);
	}
}
