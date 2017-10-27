# BOIDS v2
BOID-based predation model (Unity)

Implemented so far:

	prey (fish, as small green sphere)
		k nearest neighbors flocking via reynolds rules
		predator avoidance (run directly away)
		kinematic, constant speed, unbounded turning capability
		food seeking
	
	predator (shark, as large red sphere)
		k nearest fish seeking, weighted by distance (directly toward)
		wall avoidance
		kinematic, constant speed, bounded turning capability

	walls 
		cube around origin, death if touched
		alternate soft boundary via attraction to origin when too far away

	food (small yellow sphere)
		wanders randomly, eaten by fish

To do:

	Starvation mechanics for fish / sharks
	Perpendicular shark-avoidance for fish
	Reproduction mechanics for fish / sharks
	Smaller, coordinated predators
	Growth mechanics?
	Terrain generation
	Collision mechanics between fish
		damage?
		short term speed reduction?
		chance of "injury" i.e. longer term mobility loss?
	
