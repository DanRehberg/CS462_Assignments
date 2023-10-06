
# Programming Assignment 1

## Description
This assignment introduces concepts of collision detection and physics-derived motion of rigid bodies. The results of the assignment should allow you to understand a basic and yet fundamental approach to collision detection as it is used in virtual worlds. This topic is relevant to explore further computational geometry as it pertains to broad-phase (e.g., **oriented bounding boxes**, **sphere collisions**) and narrow-phase (**Separating-Axis Theorem**, **Gilbert-Johnson-Keerthi**, **Minkowski-Portal Refinement**, **Voronoi Clip**) collision detection algorithms. The assignment will also provide some information regarding discrete and continuous flavors of collisions detection.

**Note:** Collision detection is a large field with a plethora of research behind it. Highly specialized collision scenarios are used across several parts of a game engine as well as for writing more advanced shaders. It is commonly part of the *"physics engine"* inside a game engine as mechanical simulation requires knowing if volumes are occupying the same space, and what axis or what contact point describes this overlap. For an entire catalogue of collision detection solution for simulation, ray tracing, and more, please check out *Christer Ericson*'s book **Real-time Collision Detection**, and for physics integration with code, feel free to check out *Martin Baker*'s website [https://www.euclideanspace.com/physics/dynamics/collision/index.htm].

## Background

When math problems are solved by hand, it is typically desired to have exact **analytical** solutions. These types of solutions account for all variances allowed for *rational* (and *real*) numbers (i.e., exact divisions, holding onto an infinite series, etc..). *Some problems cannot be solved for analytically, or are made easier through approximate solutions in a piecewise fashion*. These types of methods fall into a subject referred to as **numerical analysis**. For geometry and calculus style problems, numerical methods are employed to observe changes from a starting state to an ending state. Some examples of this can be observed in projectile motion with multiple *Forces*; where something like a rocket might have an initial velocity, be influenced by gravity, and at a whim adjust course with propellent. Rather than attempting to find an ideal curve to analyze the trajectory of this rocket, the path can be segmented into discrete steps where influences on the rocket (i.e., Forces) at a given step produces an approximate result to reveal where the rocket will be for the next time step. That is, if a rocket is located at <0,0> and the net Forces acting on it form an impulse to change its velocity to <1, 5>, and our discrete step interval (typically time) is 1.0, then the location of the rocket at the next step will be <0,0> + 1.0 x <1, 5>. The simplified perspective for this type of numerical method (namely motion) can be described naturally as:

**next state = initial state + some change**. 

The above is a brief synopsis of Euler integration, feel free to read [https://en.wikipedia.org/wiki/Euler_method] for more information. Computers are an excellent tool for these discretized approaches to solving numerical problems as the states of a computer are discrete. Virtual worlds hoping to simulate physical phenomena will employ these numerical methods to apply motion to objects, including influence of external Forces, as well as resolve constraints describing interactions between objects/fields. **However, the discrete nature of these problems and numerical precision pose tradeoffs between accuracy and performance for real-time physics simulations**. For instance, if a time step used for Euler integration is too large, then an object will deviate more from its original analytical trajectory, but allowing this to happen means information on-screen is presented in a time state near identical to the end user's perspective.

The compromise with Euler integration is minimal as all objects in a game world can be updated independently of one another, but this method of motion causes more trouble for *accuracy to performance* when dependencies are involved - notably, collisions between bodies. Two primary approaches to collision detection (and simulation) exist:

- Discrete collisions
	- Lower accuracy, possibly better performance
- Continuous collisions
	- Higher accuracy, possibly slower performance

The possibility of performance costs is dependent on the choice of algorithm - **algorithms which can be inherently costly if dealing with arbitrary polyhedra** (*3D polytopes, typically convex*). So, it is common to break collision detection into two phases, using a low-cost test to determine if the higher-cost collision algorithm warrants execution:

- Broad phase collision detection
	- Are simplistic geometries representing shapes colliding?
- Narrow phase collision detection
	- Are the actual polyhedra colliding?

Because initial choices of physics simulation (e.g., numerical integration) affect how further parts of a *physics engine* can be built, physics simulation becomes a complex topic with divergent solutions based on these primary designation of **discrete** and **continuous** simulation. Feel free to read the excellent book **Real-time Collision Detection** *by Christer Ericson* to gain insights into the tools and approaches to these problems.

## Discrete Event Simulation

Without too much detail, *discrete-event simulation* is a formalism describing how certain classes of problems can be run as a simulation in a computer. This formal description works for a vast number of simulations that are not necessarily physics problems, **including the operation of video game engines** (*see García, and Ramón Mollá. “Videogames dec1Inmaculadaoupled discrete event simulation,” Computer & Graphics no. 29 (2005): 195-202.*) The basic outline of events in a game engine are running a logic loop which has the following:

- **States** which change based on a **clock**
	- *Clock* is local to the simulation
- **Events** which are queued (and might propagate) to simulate

Within this model of simulation is embedded a physics engine - *when physics simulation is desired in a game*. For physics simulation, *states include kinematic equations which describe current motion, shapes describing the hull of objects (used to determine inertial resistance given a mass), and position and orientation* to name a few. *Events might describe two bodies colliding and propagating a constraint to a solver to cause build an impulse which will separate the bodies*.

Determining whether a collision event occurred is critical to avoid unnecessary compute intensive operations. For instance, it would be excessive to build a system of constraints for every pair of objects in a game, per frame, if the majority solutions to these constraints result in zero impulses (*suggesting no constraint violations are occurring*). But, even adding complex collision algorithms to test for needed constraints can get out of hand if testing all pairwise combinations, **O(N^2)**, of objects per frame.

Instead, a low-cost heuristic (typically **O(1)**) is used as a preliminary test to determine if objects are likely colliding.

## Broad-Phase Collision Tests

The heuristic of choice is to describe all objects as simple geometries to run an inaccurate collision test before employing a higher-cost algorithm using the actual shapes describing these objects. If and only if the simple geometries appear to be colliding do we then run a **narrow phase collision test** on the two objects.

While several different geometries have simplifications to determine if collisions (i.e., intersections) are occurring, commonly used choices are sphere-sphere collisions and box-box collisions. In general, a common simplification is used for both broad-phase and narrow-phase tests: using convex shapes. For more information as to why, consider reading about *support mapping functions* which describe a function which return a position along the outer hull of a shape.

### Sphere Collisions

Spheres, and circles in 2D, have extremely simple qualities to define their internal volume as well as their hull (external boundary). Spheres have a *position describing their center of mass* **from which their hull** *is a constant distance (**radius**) from the position in all directions.* This uniform distance from the sphere's center is the quality that makes this type of test incredibly simple (*performant*).

*Effectively, a point is inside a sphere if the distance between the point and the sphere's center is less than the length of the sphere's radius.* **The assignment exercise is not performing sphere-sphere collisions and instead working from this even simpler perspective as an introduction to collision subjects.**

Vector mathematics can help simplify this testing for arbitrary dimensions as this uniform radius quality holds for all hyperspheres (hulls of n-dimensional spheres which separate the inside of the sphere from the outside).

- **Given a sphere**
	- Position described with a Vector
		- Call it *C*
	- Radius described as a Scalar (e.g., Real number, Float)
		- Call it *r*
- **Given a position to test for collision with the sphere**
	- Position described with a Vector
		- Call it *P*

We first need to determine how far *P* is from *C*. **We will need a scalar describing this distance to correctly compare it with the distance *r***.

Because we merely want a distance between *P* and *C*, we can start by subtracting *P - C* **or alternatively** *C - P* to get a **new Vector** *CP* or *PC*, respectively. While Vector subtraction is not commutative (because the result will describe direction using positive and negative signs), the magnitude of both of these vectors will be equivalent, and so we can ignore the order of subtraction.

Next, we will need to calculate the magnitude of *CP* or *PC*, based on whichever subtraction was performed. This is achieved by using the *Pythagorean Theorem* as if a right triangle was constructed with *CP* and *CP* (or *PC* and *PC*). **Recall that taking the Dot product of a vector with itself is effectively the Pythagorean Theorem without a square root operation.** So, say we computed the vector *CP*, we can find its magnitude with the dot product and a square root function:

**magnitude = sqrt(Dot(*CP*, *CP*))**

The result magnitude is a scalar distance, and now can be compared to the radial distance *r*:

- if *r* > *magnitude*
	- then position is inside sphere
- else
	- then position is outside of sphere

**NOTE:** *For the assignment, this test is made slightly more performant*. While the above is an accurate representation of being in a sphere, it also includes a **square root** invocation which can be avoided. **If the squared radius is used in testing, then the magnitude of the test position to the sphere can also be squared.** Because  distances are proportional to their squared counterpart, we can determine if one distance is larger than another even if these distances are both squared magnitudes.

### Box Collisions

In broad-phase collision testing, encapsulating a complex shape with a simpler geometry results in an overestimation of space the shape occupies. This inaccuracy is what allows for increased performance when performing a broad-phase test, but the choice of shape can provide worse inaccuracy while achieving greater performance. In the case of **box-box collisions, *it takes more time to compute a collision inquiry than sphere-sphere collision but it represents a tighter-fitting boundary and so is more accurate than sphere-sphere collisions.***

The increased cost of testing box collisions can be worthwhile when shapes are narrow along one dimension, but wide along another. Whereas a sphere needs a constant radius while encapsulating a shape, and so must always choose the largest dimension the true shape is occupying - *resulting in potentially more false-positives in broad-phase collisions*.

Because of the increased cost for box collisions, we might want to utilize the simplest scenario of intersecting boxes to continue to ensure that a broad-phase collision test is fast. *The simplest box-box collision scenario is having two axis-aligned bounding boxes (**AABB**) to test.*

An AABB means that the dimensions of the box are parallel to like axes of a coordinate space. This description is somewhat abstract because, similar to sphere collisions, the characteristic of AABB collisions hold for arbitrary dimensions. Let's consider a one-dimensional scenario to start:

**Given two line segments on a single axis, because line segments are continuous and described by end points, if one of the end points falls between the end points of the other line, then these two lines must be occupying some of the same space on this axis.** **Otherwise, these lines are not intersecting.** Visually, this is shown below.

[[image]]

For arbitrary dimensions with AABBs, this axis test needs to be performed for all dimensions present (i.e., twice in 2D, three times in 3D, etc..). If any of these tests fail to find an intersection along an axis, then there must be a separating axis between the two boxes - for more information, feel free to read into **separating-axis theorem**. This means that, for 3D, the x-values of the two AABBs are tested for intersection, y-values are testing for intersection, and the z-values are tested for intersection *independently of the other axes!*

In pseudo code, this amounts to:

- **If there is an intersection on the x axis**
	- **If there is an intersection on the y axis**
		- **If there is an intersection on the z axis**
			- ***Then the AABBs are colliding***
		- Else
			- The z axis is a separating axis
	- Else
		- The y axis is a separating axis
- Else
	- The x axis is a separating axis

To perform each of these separating axis test, given AABB, the minimal amount of information needed is a Vector holding the minimum values along each axis and a Vector holding the maximum values along each axis. For instance:

**Given a 2D AABB problem, with min0 and max0 describing the minimum and maximum positions of one box and min1 and max1 describing the minimum and maximum positions of a second box:**

- If min0.x is between min1.x and max1.x OR max0.x is between min1.x and max1.x
	- If min0.y is between min1.y and max1.y OR max0.y is between min1.y and max1.y
		- Then the AABBs are colliding
	- Else
		- No collision
- Else
	- No Collision

Notice that the two sets of conditions to test are nearly identical, **the only change is what axis is being examined.**

Briefly, it is worth mentioning why the same separating-axis tests cannot be performed if hoping to have an even tighter fitting bounding box: *an oriented-bounding box (OBB)*.

Similar to a sphere possibly greatly overestimating the size of an object, an AABB will also overestimate the size of an object as it rotates. The extrema (minimum and maximum positions) of an AABB need to be recomputed as a shape is rotating to ensure that it will still wholly encapsulate that shape for broad-phase tests. But certain shapes that are elongated will result in larger overestimations for their corresponding AABB at some rotations, and better occupancy estimations at other rotations. A fix to this would be to instead perform tests against rotated boxes, OBBs, because their volumes are always preserved.

The problem with OBB testing is an increase in complexity which results in a higher cost of execution compared to AABB collision tests. Without abstracting to higher dimensions, the problem of complexity can be observed when considering box collisions in 3D. When using AABB tests, all line segments are parallel to a corresponding axis which allows intersection testing with just the extrema points of a box. However, if boxes are rotated (two OBBs) such that their edges do not align with each other relative to a common coordinate system's axes, then a different set of features must be tested - vertex-face, edge-face, and edge-edge collisions. Again, refer to *Christer Ericson*'s book or *separating-axis theorem* if you want greater understanding of this problem.

## Tasks

The assignment will have you completing two functions for collision testing in Unity. The Unity project to open in this repository is **P1_DiscreteCollisions**. The functions to complete are in the script **CollisionTests.cs**. The types of functions to implement are typically used for broad-phase collision tests but will be used to yield a discrete physics simulation scenario. Download and unzip the repository, open P1_DiscreteCollisions and open the *CollisionScene* scene if it is closed. Unity version 2021.3.1 was used for this project, but this project should be safe to open with any version of Unity 2021.3.X.

Discrete physics simulation implies that collisions of objects are only considered if objects are found to be colliding after their motion integration (Euler integration in this case). This means that if a objects are moving too fast, they could be seen to fly through each other with colliding (**the bullet through paper problem**). Continuous physics simulation would instead try to find a collision event between the starting state of a frame and the end state from motion integration. For demonstration purposes, discrete simulation is sound for the assignment.

Feel free to review the Unity Vector3 operations available, https://docs.unity3d.com/ScriptReference/Vector3.html, but here are the following operations you will need for this assignment:

- Vector subtraction, given Vector3 *a* and Vector3 *b*
	- Vector3 result = *a* - *b*
- Dot product, given Vector3 *a*, and Vector3 *b*
	- float result = Vector3.Dot(*a*, *b*)
	- **NOTE:** sqrt is not needed in the assignment as the sphere tests use squared values
- Component access, given a Vector3 *a*
	- *a*.x
		- Access the "x" float component
	- *a*.y
		- Access the "y" float component
	- *a*.z
		- Access the "z" float component
Other operators used will be those typically used for floating point value comparisons.

For the first task, you will be completing **collisionVertexSphere**. This functions takes in the position of a sphere (its center of mass), a squared radius (i.e., radius x radius), and a vertex position to test against the boundaries of the sphere. **Three subtasks need to be finished to complete this function:**

- Compute a vector between the *sphere center* and the *vertex*
	- Subtraction is not commutative, but does not matter for this task because..
- Compute the square magnitude of the vector found above
	- Note, this is the Pythagorean Theorem without a square root operation
- Compare the *square magnitude* with the *square radius* in the provided if statement
	- *Square magnitude* represented the squared distance of the vertex to the sphere
	- This if statement returns true if the vertex is inside the sphere
		- *Therefore, be sure to use the correct comparison in the if condition*

For the second task, you will be completing **collisionAABB**. This function takes in the extrema (minimum and maximum positions) of two different AABBs:
- min0 and max0 refer to the minimum and maximum position of object 0
- min1 and max1 refer to the minimum and maximum position of object 1

Three if statements are provided, with two of them being nested. Each if statement should compare the min and max values of object 0 and 1 for a single axis to determine if an intersection is occurring on that specific axis. For instance, the first if statement could be for *x* values, the second if statement for *y* values, and the third if statement for *z* values, or any combination therein so long as all three axes are tested (*feel free to build a helper function above this one if so desired*).

The nesting of if statements ensure that if one separating axis is found, the function can stop testing the other axis to save on some compute. Therefore, the only time this function should return true is if no separating axis was found. Because each condition will be testing one axis' information, the operations involved are merely floating point comparisons, for instance:

**min0.x >= min1.x**

as the *x* values accessed here are just floats.

**The challenge with completing the AABB function is verifying that your conditional tests per axis cover all corner cases.** However, there are a few ways to test your correctness:

- Prove by hand formally (good starting place)
- Test play the Unity game and look at console output
	- Some test cases are provided
- While test playing the game
	- Go into the Scene window, select a cube while *motionOn* is false
		- Move the cube into another cube and see if the console output report looks correct

The *motionOn* variable is a Boolean attached to the CollisionTests.cs script that is instantiated on the ConfigurationSpace object in the scene hierarchy. You can change this Boolean to true and false by selecting the ConfigurationSpace object in the scene hierarchy (typically left side of editor), and then clicking the checkbox beside *motionOn* inside the CollisionTest.cs instance found in the Inspector window (typically right side of editor).

While *motionOn* is false, the console window will print when two AABBs are found to be colliding.

**NOTE:** The scene initially given to you has all of the boxes separated from one another. If you test play the game will motionOn is false, and the console is reporting collisions, then it is likely that there is a fault in your implementation of **Task 2**.

While *motionOn* is true, the boxes will have motion integration applied using a timestep and their respective velocities. You should be able to see the boxes bouncing off each other when AABB collisions occur, and you should be able to see the boxes moving back into the sphere if they started to leave the sphere.

## Submission