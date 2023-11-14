  
# Programming Assignment 3



## Description

This assignment introduces an approach to build non-playable character (NPC) artificial intelligence (AI) into a game. The results of the assignment should help you understand how a simple computational model can help you build interactions with NPCs. This topic covers finite-state machines (FSM) which are applicable to AI as well as general engineering problems involving control flow. The depth and breadth of AI is a vast topic, but FSMs offer a good approach to plan and implement behaviors of NPCs - while tailoring user experience for enjoyment - inside a video game.

The functions you finish in this assignment will also provide you the conceptual information needed to compute a velocity to travel from arbitrary end points, and how to keep track of programmed timed events in game engines. Moreover, you will be exposed to a simple approach to defining an FSM inside of software - something pertinent to several software engineering tasks both in and outside of the video game field.

**Note:** While a FSM is a simpler type of computer model, we write programs in an implementation of a Turing machine. This means we can utilize a structure of a FSM to design AI for NPCs in virtual worlds, but still have access to the additional features of a Turing model. This will allow us to simplify the structure of our FSM implementations when creating NPC behavior.
## Background

The topic of machine learning (ML) has been a recent discussion for researchers studying intelligence, but the subject of AI is broader than just being a topic of learning. The field of AI can utilize non-learning computational models and algorithms to solve problems necessary to simulate an independent agent. In the case of making specific and tailorable human-computer interactions with AI, it can be challenging to produce a desired product/experience with ML. One of the simplest ways to "fake" intelligent behavior is to rely on states and transitions between states using the FSM computer model. For a thorough understanding of computer models including FSMs, feel free to read Michael Sipser's textbook: *Introduction to the Theory of Computation*.

Entire virtual worlds rely on a logic loop to function - effectively, check input, update objects, and render graphics/audio. It can be very easy when programming to write excessively complex code that becomes hard to validate when it is closely coupled with other code's logic. For this reason it can be desirable to use computer models simpler than a Turing machine to view the correctness of large system behavior. From interactive experiences, for instance, branching storylines and sequential events can be characterized with little more than a set of states and the conditional transitions to branch to new states. This extends to AI where an independent agent can have their behavior described with a finite set of states while enabling dependent interactions as their state and transitions can be influence by a shared game world. That is, we can describes states with branches based entirely on an agents singular perspective, or have the agent's state change by a branch condition influenced by the external world.
## Finite State Machine

A FSM is a form of computer model that is easy to visualize and has limited abilities in the scopes of problems it can be used. The scope/set of programs one could build using a FSM model is still infinite, but the power available to the programs is limited. This is due to an FSM not having any modifiable memory, all available memory is predefined as the states in the program. Moreover, their is a fixed sequential order to execute a program represented with an FSM; there is no spontaneous branching.

The limited power of this model might seem concerning, but this simplicity enables the design of clean and simple logical programs. For instance, a vending machine could can be represented using an FSM. This is possible because the number of states for a vending machine can be determined beforehand given discrete units of money and predefined number of products (paired to buttons). Memory is not strictly necessary because the conditions for counting money can be predefined by the machine - by rejecting some values of legal tender, returning any input money exceeding the cost of a product, and only dispensing the product when in a state describing the correct threshold of currency input has been reached.

Within a videogame setting, the states of an FSM can be used to describe the flow of animation states (e.g., standing idle, walking, jumping, etc..) but can also correspond to more complicated tasks like: *find a path from A to B*, *act as an interactive merchant while at B*, *if the time of day is past 6:00 PM then find a path from B to A*, etc.. These types of diagrams can be visualized using conventions for drawing FSM:

//pending image

**Note:** The use of FSMs extend beyond AI inside virtual worlds. An FSM can be used to ensure *"on-the-rails"* moments in video games for real-time cutscenes, or for having user controlled limited within a narrow path for cinematic scripted events.

### Implementing an FSM

Implementation of an FSM in software can be diverse. However, the implementation can be assisted by utilizing the drawn FSM model and having a consistent mapping/correspondence between the states in a diagram to the names of the states in code. For instance, if an FSM has states labeled with names and the choice of implementation was through functions as states, then it might be useful to name the functions by the label/name from the FSM diagram. **Given typical game engine architecture, using functions that call functions as a state machine might not be the best solution (*as that type of model would run sequentially at the speed of the CPU*), and an alternative interface could be produced through a state variable and condition statements**.

Game engines typically have an object-based approach to modifying elements in a scene through independent *"update"* functions/directives. Because this is a progressive (in time) process, an NPC might need to be in the same state for several (*or quite a lot*) contiguous frames before changing state. This means that each update call needs to recall the state of the NPC from frame-to-frame. Given the object-oriented nature in game engines, this can be achieved by defining a state variable (as a class field/member) which retains the state of the character for the next frame. Inside an *update* function, we can then use a series of if,else if, etc.. statements where the condition in each statement is: ***are we in state x, or y, or z, etc..***. *Better yet, we can use a switch statement to utilize constant time for branching (jumps right to the correct condition of the state variable) AND using an integer state variable means we can have a mapping to a FSM diagram if the states are labeled with numbers*.

Going back to an idea of having an NPC travel to their place of work, an initial state variable might be *0*. In a frame, the update function for the character is called, and inside this function we implement a *switch statement* which used the state variable as a condition. *State 0 (case 0 in switch statement)* might test to see if the time of day is after 9:00 AM, if so then change to state variable to *1*, else do not change the state variable. **Changing or not changing the state variable indicates whether the NPC is in the same state for the next frame, or if they begin a new action in the next frame**. For this example, the initial state could be representing an NPC staying at home until their workday begins. In *state 1 (case 1)*, the NPC might need to solve a path finding problem to get from their home to their place of work. The condition in the state might be to see, *have I (the NPC) arrived at work?* If so, change the state variable, else keep the same state variable. Again, checking whether to change the state variable means that a persistent state can exist until some condition is reached.

**Note:** in the example of path finding, the condition to reach is actually a goal of the character. Deciding when to change a state variable can occur for desired goals, or under predefined (derived) utility concerns of actions.

### Tailoring an Experience with FSMs

When using an FSM to define AI behavior, it is important to recognize that this computational model allows you to easily tweak condition parameters to make a game more fun. Specifically, the logical sequence in a CPU is extremely fast to execute. While you could build an FSM to describe a virtual opponent in something like a fighting game, if the FSM describes the optimal thing to do state-by-state to win, then their will be no hope for a human player to beat the AI. For instance, if a human player presses a button to throw a punch, and their is an explicit 3 second period to before needing to block a punch, then an FSM can have no problem ensuring that an AI could always block the punch. Worse, if within that 3 second window the AI has an attack faster than 3 seconds that will damage the human player and stop their attack, then an FSM could always have the AI beating the player through these small time windows of reaction time. For human-human interactions, this windows might seem tight, but for a model in a computer this is ages of time to prepare.

For tailoring an enjoyable experience for a human-AI interaction, this is where FSMs shine. While ML could be used to train a model, it can be hard to control for fun experiences. Notably, training could be prematurely stopped to prevent the ML-based AI from achieving near 100% success rates in a game, but it might be hard to find a point of training that yields a good challenge to a human player or that does not still have some quirk that provides too much success for an AI in specific scenarios. In contrast, the FSM can be tailored by adding additional influences to state transitions. For instance, randomness can be added to provide stochastic influences in state transitions. Maybe an AI has a *"dice rolled"* to influence whether they can block an attack, or whether they can be persuaded in a dialogue encounter with the human player. Some states could even be added to give a human player full advantages to make them feel strong and victorious as they play a game.

In any case, it becomes important to tailor these experiences for the human player as they are the party invested in experiencing your virtual world. Some situations might involve strict linear control and guidance, in which case an FSM will have hard constraints defined transitions, but for sandbox play in a multiagent environment, it is prudent to ensure the type of experience given to the player is what you ultimately desire in your world.

## Tasks

The assignment will have you completing two functions in a C# script named **Librarian.cs** which is found in the *Assets/Scripts* folder of the relevant project. Download (or clone) and unzip the repository, open **P3_FiniteStateMachineAI** in *Unity Hub* and then open the **Library** scene if it is not open. Unity version 2021.3.1 was used for this project, but this project should be safe to open with any version of Unity 2021.3.X.

There are only two new features to use in this project to complete the two functions/tasks. In Unity, the Vector3 class contains a member that is accessible to obtain a normalized vector. Unity also gives you access to the amount of time that has elapsed since the previous frame:

- Normalized vector in Unity:
	- vectorA.normalized
- Time elapsed (fraction/float of a second)
	- Time.deltaTime

Task 1 requires you to compute a velocity to return in the following function:
- **private Vector3 velocity(float speed, Vector3 goalPosition)**
To do so you must compute a new Vector3 *traveling from the librarian position to the goal position*. **Recall that vector subtraction is not commutative**, so if you find the librarian travels in the wrong direction you can simply swap the terms (operands) in this subtraction. Secondly, *you want only the direction information in this vector, therefore after you perform the subtraction you will want to normalize the result*. Once you have the normalized direction, you can *scale it by the speed* argument passed into the function. *In total, you find the general (normalized) direction from the librarian to the goal, then scale that vector by a speed factor to generate a velocity.*

**To scale a vector by a float, you can use the typical asterisk operator for multiplication:**
	- Scale a Vector3 by a float:
		- floatVal \* vectorA


The second task requires a bit more management to work correctly as you will be modifying a member variable inside the Librarian class. The function to complete for this task is:
- **private bool waitForTime(float max)**
The function takes in a float *max* which describes the amount of time that must elapse (persistent across frames) before the AI can move onto the next state. **THEREFORE, DO NOT STALL THE PROGRAM IN THIS FUNCTION AS IT WILL PREVENT THE GAME FROM RENDERING NEW FRAMES!!** Instead, you will use the *Time.deltaTime* variable in Unity to *add the elapsed time from the previous frame to a member variable named selfTimer*. Once that is complete, *you will need to write an if-statement condition to test if selfTimer has exceeded the wait period defined by the float max argument passed into the function*. It the amount of time elapsed has exceeded max, *then you will need to reset selfTimer to zero and return true*. **Resetting selfTimer to zero is important to ensuring that future timed events can behave correctly in the FSM**. If you end up in a state where the librarian is waiting indefinitely, then check to make sure you have reset selfTimer.

**Note:** in the Mono implementation of C#, numbers with a decimal point (*e.g., 0.0*) are seen as *double* types, **BUT** objects like Vector3 in Unity use 32-bit floating point types. To ensure your numerical values are seen as floats, be sure to add the '*f*' literal to the end of a typed value (*e.g., 0.0f*).

To assist in your own project developments in the course, the *Update* function found in the *Librarian.cs* script demonstrates the implementation of the FSM describing the behavior of the librarian in this miniature virtual world. *While observing the FSM in the Update function is not directly related to completing the assignment tasks, observing the structure of this FSM implementation should allow you implement your own basic AI.* For instance, this project began by drawing a FSM model by hand to construct the logical sequence of the librarian:

![Librarian Finite State Machine Diagram](https://github.com/DanRehberg/CS462_Assignments/blob/main/images/librarianFSM.png)

Within the switch statement, the librarian begins in a state where they are waiting at a counter to service a request by a student:
- State 0:
	- Stand at the counter
	- Determine if any books are overdue
		- If so, change to state 7 to retrieve the book from a student
	- Else
		- If a student is at the counter
			- Take a book from them, if one exists and change to state 1
- State 1:
	- Go to the bookshelf
	- If arrived at the bookshelf then change to state 2
- State 2:
	- If the librarian is holding a book (book to return)
		- Wait for some amount of time
			- Modify a "returnedBook" variable to true
			- Return the book and change to state 3
	- If the librarian is not holding a book (checkout a book)
		- Wait for some amount of time
			- If books are available
				- Grab book and change to state 3
			- Else
				- Modify a "noBook" variable to true and change to state 3
- State 3:
	- Walk back to the counter
		- If at counter
			- If "noBook" is true
				- Set it to false and change to state 5
			- Else if "returnedBook" is true
				- Set it to false and change to state 6
			- Else
				- Change to state 4
- State 4:
	- If student is no longer at the count AND the librarian has a book
		- Then change to state 1 to return the book
	- Else
		- Keep a record of the student checking out the book
		- Give the book to the student
		- Change to state 6
- State 5:
	- If the student is not at the counter OR wait for some time
		- Change to state 6
- State 6:
	- if librarian takes a break for some amount of time
		- Change to state 0
- State 7:
	- If librarian has traveled to student with overdue book
		- Take the book
		- Remove the checkout record of the student
		- Change to state 1

**To test this FSM and your finished functions**, test play the game in the Unity editor, but switch back to the *Scene* view so you can move the student objects around. The librarian and students are represented as cylinders, and when the game starts the librarian will be a unique color compared to the student cylinders. Grab a student cylinder and move them in front of the librarian to initiate a book checkout request. If you leave the student in this position, then the librarian will eventually give the book to the student; otherwise, if you move the student, then the librarian will return the book after finding no student at the counter. Once a student has a book, you can move them away from the counter. At this point, you could move the student back to return a book, or move a new student to the counter to observe the FSM behaviors described above by the librarian. Eventually, if a book is not returned, the librarian will pursue a student to retrieve the overdue book.

The simplicity of the graphics and control through the test tools available in Unity demonstrate that gameplay mechanics can be prototyped well before all interactions are defined or final production graphics are available. For instance, a fully featured project might add in-game controls to move students, or might even describe students with their own AI FSM. However, once one component of a game is available, testing and iteration on design can begin immediately to verify correctness or to begin tailoring for user experience.

## Submission

Download the Unity project from GitHub by clicking the *Code* button followed by the *Download Zip* button in the drop down menu. Unzip the project and open it in Unity by adding the project through Unity Hub.

Complete the tasks in the *Librarian.cs*. Verify that no errors are popping up in the Unity console related to this script.

Zip your completed script into an archive named *FirstName_LastName_P3.zip* filling in FirstName and LastName with your first and last name.

This assignment is worth **100 points**:
Pending..
