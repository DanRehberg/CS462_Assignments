
# A0: Lights On

## Description
This assignment introduces the programmable render pipeline associated with *Modern 3D Graphics Programming*. The assignment will provide you a basic vertex and fragment shader for use within the Unity game engine, as well as give you fundamental understanding for basic lighting operations in virtual worlds. This information is relevant to diving into more advanced lighting models (e.g., **blinn-phong, physically-based rendering (PBR), ray tracing, etc..**) as well as further shading techniques for interactive computer graphics (e.g., **GPU animations, forward vs differed rendering, texture mapping, parallax textures, ray marching w/signed-distance fields (SDF)**). This assignment should also leave you with further systems-level understanding of graphics-processing units (GPU).

**Note:** Further exploration of shaders in programmable rendering pipelines include topics like primitive grouping (*geometry shaders*) and dynamic-level of detail (tessellation shaders). These shaders are not typically required to get started with rendering through graphics APIs (e.g., DirectX, OpenGL, etc..), but allow for more complicated effects to be performed efficiently under real-time constraints (e.g., **generating fur, terrain rendering, infinite-level of detail, etc..**). Additionally, graphics APIs typically offer general computer shaders to perform arbitrary processing in the massively parallel processing environment provided within GPUs.

## Getting Started
Download this repositoy, unzip it, then add the A0_LightsOn project within Unity Hub. Unity version 2021.3.1 is used for this assignment.

If the **DiffuseLighting** scene is not open, then in the project window double click on DiffuseLighting inside: _Assets/Scenes_. **Ignore the NormalMapLighting scene, that will be for an upcoming assignment.**

There are four game object groups in the *scene heirarchy*.
- **Example Group**
	- Uses built in Unity lighting features
 - **Direction Group**
	- Plane and cube using the *Dir.shader* and *Unlit_Dir* material
 - **Point Group**
	- Plane and cube using the *Point.shader* and *Point_Dir* material
 - **Spot Group**
	- Plane and cube using the *Spot.shader* and *Unlit_Spot* material
   
Deactivate and activate the groups to observe correctness of shaders and lighting models as the assignment is being completed. The *Example Group* allows you to modify the built in Unity light which can be switched between a direction, point, and spot light. The other groups have a **Sphere** object which can be modified to alter their respective lighting model once the shaders are completed. These **Sphere** objects can be translated and rotated demonstrating variance in a lighting model (i.e., *direction lights change via rotations, points by translations, and spot lights with both rotations and translations*). **These modifications only happen when testing playing the game.** To do so, click the *Test Play* button and switch back to the *Scene* window to modify the **Sphere** objects. *This includes setting a color for the light in the **x, y, and z** values for **red, green, and blue** colors in the public **Color** vector in the **Sphere's** attached script*.

## Background
Historically graphics rendering occurred through software renderers that ran on a CPU. Hardware acceleration became available with dedicated graphics hardware, i.e., GPUs. Graphics hardware provided application-specific integrated circuits (ASIC) to build out each section of a fixed-function render pipeline. The fixed-function pipeline existed in pre 2.0 OpenGL and through DirectX 7, standards and API designs of the Khronos Group and Microsoft, respectively. The fixed-function pipeline abstracted the need to interface with a GPU to get graphics on the screen quickly (via the ASICs for acceleration), but limited the artistic control over the final rendered graphics. A move towards a programmable model using shaders (*small programs to execute at stages of rendering on a GPU*) grew in OpenGL 2.0 and heavily in DirectX 9. Eventually this led to a more generalized architecture (unified) for GPUs rather than specialized circuits (ASICs) for each stage of a render pipeline. This allowed shaders to share a unified set of processors on a GPU, which consequently led to a massive growth in research and science utilizing general-purpose graphics processors (GPGPU) for parallel computations. *For a full history, check out **Programming Massively Parallel Processors: A Hands-on Approach** chapter 2, by David B. Kirk and Wen-mei W. Hwo.*

These programmable shaders have been the primary means of interfacing with GPUs for contemporary rasterized graphics, but are also pertinent to advanced methods of ray-tracing and other forms of conveying 3D imagery on screens.

## Basic Pipeline and Required Shaders
The simplest perspective of the render pipeline amounts to:
- Process vertices
- Group primitives
- Rasterize pixels in primitives
- Color rasterized pixels

Pipelines have more stages then this, but are not pertinent for the scope of this assignment. Feel free to read into the standards and APIs of OpenGL and DirectX for further insights.

### Instances for Object-based Rendering

For object-based rendering, meshes are loaded and stored as vertices in video memory (VRAM) of a GPU. The meshes typically represent triangulated meshes, but the actual vertex information and grouping procedure is somewhat arbitrary (*in part, given the power and flexibility of a programmable pipeline*). The meshes can effectively be instantiated an arbitrary number of times per frame through render calls (*of course performance might suffer, even with instancing optimizations*). This reusability through instancing is the feature of *"object-based rendering"*. **However, if each instance is supposed to exist at a different position, possibly with different directions or scales, then the vertices of a mesh must be transformed to account for these instance-based changes!**

A few transformations are required to get the vertices of a mesh where they need to be. The base mesh is defined within its own local (or object) coordinate space. So, the first transformation will be to modify the vertices so their positions align with a shared global coordinate space (*shared with all other instances being rendering*). This is typically referred to as the **model** matrix.

Next, a 3D scene is being viewed from a particular vantage point, which can include being view at obscure angles. Typically, there is an analogy used describing this as a **camera**, but the typical transformation comes from another matrix referred to as the **view** matrix.

Finally, the boundaries of the entire view need to get mapped to a space that will be fit to the end users screen. The screen space is typically referred to as *clip space* and the matrix to transform into this space is typically called the **projection** matrix (as the vertices are being projected into the screen). If the projection involves perspective then this matrix typically defines a frustum, a rectangular prism where a near plane is smaller than a far plane which is large (*forming a kind of pyramid shape with a flat top*). Objects in a frustum are scaled inversely to the size of the near and the far planes. While the near plane is small, objects scale larger when closer to the near plane, and though the far plane is large, objects scale smaller when closer to the far plane. **This scaling aligns with monocular depth perception, allowing object distances to be perceived by proximal size to a viewer rather than only relying on parallax to perceive distances.**

As we multiply matrices to transform vectors or vector spaces, **recall that matrix multiplication is NOT commutative**. Just remember that the left side of the multiplication operand is transforming the information on the right side of the operand.

For instance, to transform a mesh vertex to its global space, you will have the model matrix on the left and the position information on the right:

**Model Matrix * Position**

***This commutativity is relevant for part one of this assignment..***

### Vertex Data and Interpolation

You might think of vertex data as merely providing a position to describe where in a coordinate space the vertex exists. However, vertex data can contain many attributes relevant to rendering meshes. For instance, if a texture is applied to a triangle in a mesh, then the vertices could contain an attribute for their position in 3D space, but can also contain coordinates describes what *texels* to lookup in a texture (commonly referred to as **UV** coordinates).

If we only store the vertices that compose a mesh, how do final surfaces get generated? Moreover, if we supply additional vertex attributes like UV texture coordinates per vertex, then how does a pixel inside a triangle being rendered know what UV coordinates it should use?

The answer is related to how **rasterization**, *the creation of pixels along a primitive surface*, occurs. Rendering primitives typically occurs from 0D points, 1D lines, and 2D triangles. For cases above 0D primitives (or more specifically, **simplices** for these geometries), we effectively have endpoints describing the extrema of a line or triangle. When rasterization occurs, filling in the missing pixel information on a linear or triangle surface is performed by testing if a given pixel exists on this linear surfaces. Testing if a pixel should be on a surface typically coincides with an interpolation analysis because it reveals two things:

- Is this pixel on this surface?
- How do I compute the attributes for this pixel from the vertices that compose this surface?

There are other ways to generate surfaces during rasterization, for instance feel free to look at the *Bresenham line algorithm*, but interpolation provides weighting between vertices. Consider a line with endpoints *A* and *B*. *If B is equal to A plus the vector AB, then we can use interpolation to produce parametric weights between A and B, such that a weight of 0.0 means a position is at A and a weight of 1.0 means a position is at B - call W the scalar representing this weight \[0.0, 1.0\]. Any point between A and B can then be represented as:*

***(1.0 - W) * A + W * B***

These parametric weights thus represent a linear interpolation between *A* and *B*. Consequently when used to rasterize pixels in a surface, all attributes of the vertices are interpolated by this weight. While not the focus of this assignment, triangular weights are computed with **Barycentric Coordinates** as discovered by famed mathematician *August Ferdinand Möbius*. Feel free to explore this triangle projections further as they are pertinent to rasterization, physics simulation (via differential geometry), and even ray tracing.

As part of the rasterization process, this interpolation between vertex attributes is performed without your input in the programmable pipeline. So, when you store UV coordinates for vertices in your mesh, the interpolation process calculates what the UV coordinate should be for a given pixel. Effectively, when vertex attributes are passed out of the *vertex shader* and grouped into primitives, the rasterized pixels will contain the attributes passed out of the vertex shader but interpolated to correspond to the proximity of that pixel to the vertices which help compose it. **For this assignment, your concern will be with the normal vector attribute of the vertices, and ensuring they get passed along (and therefore interpolated) to the fragment (pixel) shader.** *Interpolating the normal vectors of vertices is what allows triangulated meshes to appear as smooth curves rather than as blocky polyhedra, given a process called **Gouraurd shading***. *This interpolation is also what allows continuity over surfaces between linearly independent primitive processing that occurs when rendering meshes.*

**Attributes that get interpolated in the render pipeline are passed out from an earlier shader stage as output(s) to an upcoming shader stage as input(s). *Conventions for passing output and input will vary between shader languages, but within Unity's interface we will be outputting and inputting structs which hold the information to interpolate.***
### Lambert's Cosine Law and Diffusing Light

Lighting models share a common question: *how illuminated is a surface?* This assignment will deal with **diffuse** light only, but here is a quick description of both **diffuse** and **specular** reflections:

- **Diffuse** reflection is light reflecting equally in all directions from a surface.
	- Referred to as **Lambertian Reflectance**
	- This gives surfaces a soft(er) look (matte)
- **Specular** reflection is light reflecting at the same incident angle that it hit a surface, but on the opposite side of a normal.
	- Think mirrors (shiny/gloss)
	- Traditional lighting models made this work via a hack
		- PBR models use Fresnel reflection to compute a more accurate hard reflection which preserves conservation of information

For diffuse reflectance (aka Lambertian Reflectance), it is the case that luminosity of a surface will obey Lambert's Cosine Law. Feel free to investigate this concept further, but effectively it asserts that the amount of luminosity provided by a light source is proportional to the cosine of the angle (**incident angle**) to a surface. Recall that the *dot product results in the cosine of an angle between two vectors, times the magnitude of each vector.* **Therefore, if two vectors are normalized (of unit length, implying magnitudes of 1.0), then we can compute the angle of incidence through a dot product!** This angle of incidence for light hitting a surface is a question of how perpendicular the light direction is to the surface in question - *so, the vector we compare to the light direction is the **normal vector** which is a vector perpendicular to the surface.*

**Note:** depending on the application, a normal vector to a surface is not always normalized/unit length. For instance, applications in physics want the normal to have a magnitude equal to the surface area of the surface it is on. **However, for computer graphics, just assume a normal vector is always unit length unless stated otherwise.**

In short, if we know a surface is supposed to be a given color, and we have the surface normal and have (or can compute) the light direction from the surface to the light source, then the final color for a surface is just (as pseudocode):

**Color * Max(0.0, Dot(Normalize(Direction to Light), Surface Normal))**

The right side of the multiplication operand is the coefficient representing the cosine of the incident angle. If this value is negative, then the light is on the opposite side of a surface, requiring us to force the coefficient to zero as no light will reach a surface facing away from a light source (e.g., simplistic lighting models, not ray tracing).
## Tasks

The objectives of this assignment are to complete three shaders in the supplied Unity project to observe three common lighting models using only Lambertian reflection (i.e., no specular reflections). These are:

- Directional Light
	- The light vector is consistent across a surface (e.g., the sun)
- Point Light
	- The light emits photons in all directions uniformly (e.g., ideal lightbulb)
- Spot Light
	- The light emits within a conical shape in a given direction (e.g., flashlight)

This will require fixing issues in partially complete vertex and fragment shaders. Three shaders inside the ./Materials/DiffuseLighting folder will need to be fixed:

- Dir.shader
- Point.shader
- Spot.shader

**Note:** the shaders in ./Materials/NormalMap are for a future assignment, so ignore those for now.

The issues to fix in the shaders are as followes:

- Dir Shader
	- Use the model, view, and projection matrices to transform the vertex to clip space
	- Compute the angle of incidence coefficient for Lambertian reflectance
- Point Shader
	- Use the model, view, and projection matrices to transform the vertex to clip space
	- Compute the vector from the pixel/fragments surface to the light source
	- Compute the angle of incidence coefficient for Lambertian reflectance
- Spot Shader
	- Use the model, view, and projection matrices to transform the vertex to clip space
	- Compute the vector from the pixel/fragments surface to the light source
	- Compute the angle of incidence of a pixel/fragment direction to the spot light and the direction the spot light is facing
	- Compute the angle of incidence coefficient for Lambertian reflectance

Completing these tasks will occur inside the vertex function, *v2f vert(appdata v)*, and the fragment function, *fixed4 frag(v2f i) : SV_Target*. These functions follow a c-style appearance where the functions have *return types, function name, and arguments*. The arguments in these functions are *structs* that are defined in the same *.shader* file above these functions. If directly interfacing with shaders for graphics APIs (e.g., OpenGL, DirectX), these functions would typically be in two separate files and are written in a program convention with a *main function* entry point. However, Unity abstracts this away and will ultimately generate your shader programs from these functions.

For the vertex function, the struct *appdata* contains the names of vertex attributes needed to render models. The attributes available by default can be found on, https://docs.unity3d.com/Manual/SL-VertexProgramInputs.html, but all of the necessary attributes needed for this assignment are already provided. These attributes are:
- the coordinates (position) of a vertex
- the normal vector (direction the vertex faces) of a vertex
- the texture (UV) coordinates mapped to a vertex
These attributes will be transformed and pushed towards the fragment shader program by rasterizing (and consequently interpolating) the outputs from the vertex shader program. The struct to populate and return from the vertex shader is called *v2f* in this particular case. *v2f* has the following variables:
- a 2-part vector for the vertex UV coordinate
- a 4-part vector for the screen-space (clip space) coordinate for the vertex position
- a 3-part vector for the vertex normal after transformed to *global space*
- (**In Point and Spot shaders**) a 3-part vector for the vertex position in *global space*
**The last 3 variables in the struct are filled in for you, you will just need to finish transforming the appdata position value using *model, view,* and *projection*.**

**Note:** In the Point and Spot shader you can see that the vertex position being constructed into a 4-part vector (float4) occurs by giving the 4th component a value of 1.0 before being multiplied by the *model matrix*. However, a similar calculation is occurring to transform the normal vector, but the 4th component is set to a 0.0. This is because the position is not a vector, meaning it exists at a location in 3D space, but the normal vector is a vector and could exist anywhere in 3D space. The last column of these 4x4 matrices is a translation vector, by having a 1.0 we include this transformation or nullify it with a 0.0 in the 4th component of the vector we are transforming.

The fragment shader will take in a *v2f* struct for each pixel created during rasterization. So, while the vertex program might only process 6 vertices (two triangles) for a plane, rasterization might result in thousands of pixels (filling the plane) to process through the fragment shader. For **forward rendering**, rendering to the screen buffer in the initial draw call, the job of the fragment shader is to compute the color of the pixel currently being processed. The execution shaders happen in a massively parallel way as processing vertices is an independent task, and for coloring individual pixels is also an independent task.

### Available Types, Functions, and Operators

The following information is useful to read and modify the incomplete shaders. Firstly, these are CG shader programs (*C for Graphics*) which is near identical to HLSL (*High-level Shader Language*). Unity provides additional functions and variables that get compiled into final shader code in a preprocessor stage. Unity can also translate these programs to other languages like GLSL (*OpenGL Shading Language*).

This is a C-style language with strict types (remember to end lines with a semicolon). You will be working with floating point vectors and matrices defined under the following convention:
- float is a 1D vector
- float2 is a 2D vector
- float3 is a 3D vector
- float4 is a 4D vector
- float2x2 OR float3x3 OR float4x4 are 2x2, 3x3, and 4x4 matrices, respectively
Feel free to look at all available types in CG/HLSL, but these are sufficient for the assignment.

Normal rules of matrix multiplication apply in these shader languages in that inner dimensions must match. For instance, **mul(float4, float4x4) does not work because 4x1 * 4x4 do not have matching dimensions on either side of the operator (i.e., 1 and 4), however mul(float4x4, float4) will work because 4x4 * 4x1 do have matching dimensions on (i.e., 4 and 4)**. And, of course, *matrix multiplication is not commutative, e.g., mul(A, B) is not (generally) equal to mul(B, A)*.

The constructor for a vector has one argument per component in the vector (i.e., number of args is the cardinality of the vector). For instance, *float4(x, y, z, w)* has four arguments because it is a 4-part vector.

Sadly, the asterisk(\*) is not overloaded in this Unity interface to represent multiplication for matrices. Instead use the **mul(left operand, right operand)** function where the first argument is the left matrix and the right argument is the right matrix (or vector). For instance, as mentioned above you will need to transform a vertex position by its model matrix. In the vertex program this will look like: *mul(model, v.vertex)* but you will still need to transform this vertex by the view and projection matrices. **The matrices you will need to transform *v.vertex* by are *model*, *view*, and *projection* (in the vertex shader)**.

To complete the Lambert luminosity term, you will need to perform a dot product. The function **dot(vector A, vector B)** will return the scalar from the dot product. Vectors must have matching sizes, and this operation is commutative, i.e., *dot(A, B) is equal to dot(B, A)*. The coefficient for Lambertian lighting will also need to be clamped. You can either use the **clamp(float x, min float, max float)** or **max(0.0, float)** functions to constraint the coefficient between \[0.0, 1.0\]. The clamp function requires that the value you wish to clamp be the first argument, followed by the min and max values for the second and third arguments, respectively. **The vectors need to compute the coefficient for Lambert lighting are *lightDir* and *norms* (in framgent shader)**.

Normal scalar multiplication for floats use the asterisk (\*) symbol. E.g., *given float A and float B, A \* B multiplies A and B*. **This is mentioned because this is a different language from Java and standard C, but your prior biases from programming likely happen to align with this operator.**

You will not need to normalize anything in this assignment, *such normalization has already been performed for you in the shaders*. However, for reference the function **normalize(vector)** will return a normalized version of the vector (by copy, not by reference). Sampling the albedo texture has also been performed for you, but the function for sampling a 2D texture is **tex2D(sampler2D texture, float2 coordinates)**.

Lastly, for the Point and Spot shaders you will need to compute the vector from the pixel to the light source. This is normal vector subtraction and you can use the minus (-) operation, e.g., *given vector A and vector B, B - A returns the vector from A to B (note, **NOT COMMUTATIVE**)*. **The vectors you will need to subtract are *\_LightPos.xyz* and *i.vertGlobal* (in fragment shader)**. If you subtract them in the wrong order, you will likely have a harder time seeing your objects.

Each section is labeled as **Task** in the Dir, Point, and Spot shaders. Each task requires you to finish one line of code, *although if you want to use more than one line (for instance in transforming the vertex position) then that is fine.*

*In the Spot.shader, a task exists to finish the dot product for cone test. This will involve the **lightDir** vector as well as the normalized \_LightDir.xyz vector, provided in the variable **spotDir** vector*.
## Submission

Download the Unity project from GitHub by clicking the *Code* button followed by the *Download Zip* button in the drop down menu. Unzip the project and open it in Unity by adding the project through Unity Hub.

Complete the tasks in the *Dir.shader*, *Point.shader*, and *Spot.shader*. Verify that no errors are popping up in the Unity console.

Zip you three shaders into an archive named *FirstName_LastName_A0.zip* filling in FirstName and LastName with your first and last name.

**You MUST use the *model, view, and projection* matrices provided in the vertex shader. If you use any other helper functions to transform your vertex position to clip space then you will lose 5 points.**

This assignment is worth **10 points**:
- 2 points for completing Dir.shader
	- Are the two tasks correct
- 3 points for completing Point.shader
	- Are the three tasks correct
- 4 points for completing Spot.shader
	- Are the four tasks correct
- 1 point for no errors for Dir.shader, Point.shader, and Spot.shader in Unity console
  - (Is okay if errs present for the TBN shaders)

