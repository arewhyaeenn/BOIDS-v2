# BOIDS v2
BOID-based predation model (Unity)

Implemented so far:

	prey (fish, as small green spheres)
		k nearest neighbors flocking via reynolds rules
		predator avoidance (run directly away)
		kinematic, constant speed, unbounded turning capability
	
	predator (shark, as large red ball)
		k nearest fish seeking, weighted by distance (directly toward)
		wall avoidance
		kinematic, constant speed, bounded turning capability

	walls 
		cube around origin, death if touched

To do:

	Resource generation / seeking for fish
	Starvation mechanics for fish / sharks
	Perpendicular shark-avoidance for fish
	Reproduction mechanics for fish / sharks
	Smaller, coordinated predators
	Growth mechanics?
	Terrain generation
	Softer boundary mechanics
		attraction to center if too far away?
		short term speed reduction if walls are hit?
	Collision mechanics between fish
		damage?
		short term speed reduction?
		chance of "injury" i.e. longer term mobility loss?
	
