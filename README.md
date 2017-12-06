# BOIDS v2
BOID-based predation model (Unity)

Implemented so far:

	prey (fish, as small green sphere)
		k nearest neighbors flocking via reynolds rules
		predator avoidance (run directly away)
		predator dodging (if predator is coming toward you move perpendicular to its velocity)
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
		
	feeding and starvation mechanics
		fish and sharks both have health points
		health is gained by eating
		fish gain for eating is constant
		shark gain for eating is that of eaten fish
	
	reproduction mechanics
		hp gained past maximum is stored as "fat"
		upon reaching fat threshold, asexually reproduce
		parameters vary upon reproduction based on simulation inputs
		pros: we can see natural selection happen
		cons: much harder to create stability; inevitably one population out-adapts than the other -> extinction
	
	collision mechanics
		fish and sharks lose health for colliding with eachother
		
		

To do:

	Smaller, coordinated predators
		herd into ball, surround, take turns diving in (thanks discover channel)
		requires agreed upon rendezvous point ("home base")
	Under-fed / well-fed agent mechanics
	Growth and aging mechanics
	Terrain generation
		planar walls in a box are boring and the soft cap distance is better
		sea floor , surface would allow for more niche behaviors
		allows for terrain-shelter behavior for prey, egg-laying and sexual reproduction...
	Collision mechanics
		short term speed reduction?
		chance of "injury" i.e. longer term mobility loss?
