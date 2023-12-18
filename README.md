# CS462_Assignments
Assignments created for Virtual Worlds Course at Colorado State University

The course project requires students build a game in Unity. As a broad course with open goals, these assignments offer a small sample of programming tasks that occur in game development for **computer graphics**, **collision detection**, and **prototyping AI for non-playable characters**. A common theme in these small assignments is the use of vector mathematics - a primary tool needed to develop 3D games and gameplay mechanics therein. However, additional concepts are described and available to study/use in the provided code to handle some of the quirks of game programming (e.g., solving problems over time rather than in one algorithm/function call).

## Assignment Overview
### A0 Lights On
This assignment demonstrates how to build a basic shader within Unity. Unity utilizes an intermediary interface for shader programming called Shader Lab. Traditional shader programs are written in separate files with some common skeleton structures required when beginning to build the file (e.g., version numbers, _main_ entry point definition, etc..), but Shader Lab provides some of these details out-of-the box (_making it easier, and sometimes harder, to work with_). Unity builds the Shader Lab file into proper shaders and so has some additional features provided to end users.

The assignment asks for students to transform a vertex in the traditional _model_, _view_, and _projection_ matrices to yield a coordinate in _normalized-device coordinates_ (i.e., clip space). Understanding how to transform objects between coordinate systems is meaningful to be able to solve graphics and gameplay challenges in typical game design. Finally, students finish a fragment/pixel shader to color pixels rasterized in the render pipeline - utlimately relying on _Lambertian reflection_ to yield a color with dynamic light sources. This lighting model demonstrates the geometric representation of a dot product - comparing how alike two vectors are - which is a powerful tool when attempting to analyze multivariable problems. Exploiting this geometric relationship yields relationships for how matrix transformations work, how transcendental functions (trig) can be solved with finite operations, and extends to how neural networks function. This topic is also crucial for multivariable calculus for solving physics simulation problems, but can also be used to make basic gameplay functionality (view direction, backstap, etc..).

### P1
This assignment demonstrates how to consider the geometric boundaries between interactive objects. For events that are not driven by user input, a typical _event trigger_ is to use collision detection to spur up new events. The assignment goes through two reduced collision detection algorithms (using simple geoemtries). These algorithms are for specialized shapes, which is a common practice to minimize execution time when solving a strict type of collision (e.g., broadphase collision detection).

### P2
This assignment extends A0 to discuss how shaders can be used to add additional details in a rendering without using higher-poly meshes. This relies on having an additional texture to store information across the planar surfaces of triangles. Effectively, because the final image representation is dictated by lighting equations for each pixel, these pixels can use a texture look-up to determine what normal they would have if the planar surface was actually a complex multifaceted mesh. This is a common rendering technique to increase fidelity for _interactive computer graphics_ and is meant to demosntrate both how the technique works, and to make students aware of normal textures as a means to enhance their final 3D graphics for the course project.

### P3
This assignment demonstrates how a finite state machine approach to design can be used to build an artificial intelligent agent for controlled gameplay interactions. As the AI field moves into more Neural Network-based architecture, this assignment is meant to illustrate to students that a general computer model (i.e., finite state machine) is sufficient, easy to design, and tailor for creating AI interactions with a human player.

## Contribution Acknowledgements
- Adam Coler (Pilot tester)
- Ryan Job (Pilot tester)
- Nathan Kampbell (Pilot tester)
- Everett Lewark (Reporting Bugs)
