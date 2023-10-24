
# Programming Assignment 2

[Video Link](https://www.youtube.com/watch?v=F-eBozr0vCI)

## Description

This assignment introduces the concept of defining coordinate spaces for tangent spaces to solve otherwise complex problems. The results of the assignment should allow you to understand how defining a coordinate system can help you solve problems in computer graphics. This topic is relevant beyond rendering graphics, and is also pertinent to solving more complicated collision detection and physics problems. Defining coordinate systems allows you a definite (fixed/constrained) space to work from, ensuring you are always within the bounds of some domain.

**Note:** Outside of surface rendering techniques, tangents spaces are necessary in mechanics simulation in order to define frictional forces. Just as you will see below, a tangent space is a planar surface (in this case 2D plane in 3D space) and so has two axis, *tangent* and *bitangent*, which can be used to describe directions of impulses to simulate friction. These tangent spaces have further applications in defining additional gradients for constraints, feel free to look into discrete differential geometry from Keenan Crane to explore more surface-level data interpretation: https://scholar.google.com/citations?user=Qs9FzFUAAAAJ&hl=en

## Background

From A0, you have seen the concept of shader programs which run on a GPU. The rendering pipeline has a program written to process every *vertex* (**independently of each other**) and a program to process every *fragment* (**also independently of each other**) to get your 3D models to the screen. With this process, you computed a coefficient to describe how much *diffuse lighting* would occur at a given *pixel* using *Lambertian Reflectance* properties which effectively say that the *amount of diffuse light bouncing off a surface is directly proportional to the cosine of the angle between the vector from the surface to the light source **AND** the surface's normal vector*. **To emphasize this further, the magic of vector comparisons let's rely on a finite operation to find this otherwise value from a transcendental function, i.e., the *dot product***.

While not present in A0, the assignment documentation mentions a characteristic provided in the *modern rendering pipeline* which allows increased detail when performing per-pixel lighting. This is interpolation of information between vertices, a quality influenced by *Henri Gouraud* for "***smooth***" shading. Effectively, when you pass vertex attributes out of a vertex shader, rasterization of pixel information (*i.e., new data produced between vertices*), the render pipeline can provide interpolated values of vertex attributes for pixels generated within a primitive (e.g., lines, triangles). This allows for Gouraud shading, *an interpolation of normal vectors for pixels between vertices of a mesh*. Without this interpolation, discretized shapes that represent curves - **like a sphere** - would look blocky. You can see this in Blender if you add a sphere object and render it with the "*flat*" qualifier:

![Flat Sphere](https://github.com/DanRehberg/CS462_Assignments/blob/main/images/sphereFlat.png)

Specifically, for something like a curve, while we typically render planar surfaces in real-time computer graphics, having a mesh's vertices store disparate normal vectors allows for interpolation to create a seemingly curved surface when lighting equations are applied per pixel (**blue represents vertex normals, yellow is interpolated pixel normals**):

![Line Segment with Interpolated Normals](https://github.com/DanRehberg/CS462_Assignments/blob/main/images/interpNorms.png)

The result of this on the example sphere is now a "*smooth*" surface:

![Smooth Sphere](https://github.com/DanRehberg/CS462_Assignments/blob/main/images/sphereSmooth.png)

**Note**: Rendering APIs like OpenGL and DirectX allow you to pass out and take in vertex attributes which are not interpolated for situations where you want a surface to be flat.

With this technique, we can achieve a diverse set of detail over arbitrary topologies through just the supplementation of varying normals on discretized representations of surfaces (i.e., triangulated meshes). However, **we are bound to limiting the vertex density of meshes because we are trying to achieve performant interactive computer graphics!** This means that building detail at the *microfacet* level might be unreasonable for real-time virtual worlds, even if it is the type of solution found for *offline rendering* at studious like Disney. Feel free to check out *Universal Scene Description* for more info on scene graphs for offline rendering: https://openusd.org/release/api/index.html

As you have experienced in A0, and likely in interactive computer graphics, we can add more detail to a surface using textures. This works given the interpolation features of vertex attributes. Again, consider two vertices where pixels are rasterized between these endpoints. If one of the attributes passed along the render pipeline is a UV coordinate (*a coordinate that maps to a 2D texture to look up color values*), then each pixel generated can have is UV coordinate computed through interpolation (**blue represents vertex UVs, yellow is interpolated pixel UVs**):

![Line Segment with Interpolated UVs](https://github.com/DanRehberg/CS462_Assignments/blob/main/images/interpUV.png)

**Note**: This interpolation ensures that sampling from a texture is spatially coherent between vertices.

While an albedo texture provides the diffuse color of a surface, additional textures could map to a surface to provide higher fidelity for a surface and the type of material it represents. A modern extreme of this is *physically-based rendering* (**PBR**) which describes the hardness of a surface and how metallic it is (these two attributes describe how much light is diffused versus a hard, specular, reflection). Before PBR, one of the simplest techniques for increasing surface fidelity with textures was **bump mapping**. Bump mapping was introduced by *James Blinn*, the same person behind the *Blinn-Phong* specular reflection optimization over the *Phong* reflection model. *A bump map is a grayscale texture that skews the interpolated surface normal to provide bumps and wrinkles on a surface without needing to increase the vertex density of a mesh*.

![Height Map](https://github.com/DanRehberg/CS462_Assignments/blob/main/images/heightmapExample.png)

The more modern approach to modifying surface normals, **and relevant to this assignment**, is achieved by using a ***normal map***. A normal map is a texture that explicitly states the normal direction for a given texel - *a texel is a pixel within a texture object*. Bump maps best describe smooth gradients on a surface, but normal maps can describe arbitrary vector changes which are useful for conveying smooth or jagged microfacets. It can also be more performant than bump mapping as normal mapping only requires one texture sample to devise a normal vector whereas bump mapping requires a *finite difference* analysis with several samples to account for neighboring texel descriptions of height.

You may have seen a normal map before. It uses the red, green, and blue color channels of a texture to describe the normal direction per pixel:

![Normal Map](https://github.com/DanRehberg/CS462_Assignments/blob/main/images/normalmapExample.png)

The complication with using normal maps is the need to explicitly determine where each texel's normal vector is facing in the rendered scene. For 2D scenarios, this can be trivial as all polygonal faces might be aligned in one direction, but in 3D the triangulated surfaces of a mesh can be facing arbitrary directions. This means the texture needs to be transformed to the *tangent space* (i.e., the planar surface) of a mesh, and then transformed to global space (*i.e., the model matrix transform*) to be compared to a light vector which exists in global space.

**Note**: optimizations exist by performing fewer transformations in the vertex shader and relying on interpolation to build a screen space matrix through interpolation during rasterization, but for simplicity this assignment demonstrates working with lighting in global space.
## Tangent Space

The notion of tangent space is akin to the experiences you have likely had in mathematics courses like College Algebra and Calculus 1. In those settings, you likely observed some type of continuous curve (e.g., polynomial) that was nonlinear, where the tangent to any given position on the curve was a straight line running in the direction of the curve. This is to say that tangent lines are parallel to a curve, or more generally tangent spaces exist parallel to surfaces - *regardless of the dimensionality of the space*. In the case of triangulated meshes in 3D space, the tangent space is the surface of a triangle; defining the axes of the plane that the triangle exists within.

When we add more detail to meshes, most of the increases in fidelity come from the lighting models/equations used in the fragment shader. Knowing this, you might wonder why the tangent space is useful given that any vector inside the tangent space is orthogonal to the normal vector - *and, of course, it is the normal vector we need to determine relationships with light*. If you're not wondering this, the point of concern would be that by itself the tangent space cannot be used calculate any meaningful luminance of a surface because all lighting is based on likeness results of the normal dotted against a light vector; and the information lost in the dot product is what is tangent.

What can be done with the 2D plane of tangent space is a 3D coordinate system can be built which can transform information into the this planar surface. Once this coordinate system is defined in a matrix, the vectors of something like a normal map texture can be transformed to view a final texel-level normal vector is if the normal map texture was on the tangent surface. Think of this like wrapping paper, wallpaper, or apply a skin to a phone. If a normal map texture is created, but its initial creation represents texels in the XY plane facing the +Z axis, and we have a box with 6 faces going in different directions, then the texture needs its normals to be represented as if the were glued to each of the six faces. If the box is axis aligned, then this means the texture has to be reference in one case as if it was in the YZ plane facing +X axis, YZ plane facing -X, XY plane facing -Z, XZ plane facing +Y, and XZ plane facing -Y as well as its baseline representation in the XY plane facing +Z.

By having the matrix to transform to each triangle's planar surface, ***passed out from the vertex shader and input into the fragment shader***, each vector sampled from the normal texture can be rotated to calculate the Lambert Reflectance for a given pixel.

## Building a Coordinate System

It is easy to fall into assumptions based on legacy bias for coordinate systems. US school programs and culture standardize several concepts, like progression of information traveling from left to right (e.g., reading, order of like operations in math, number lines, etc..), which can include an inference of horizontal, vertical, and depth axes for a coordinate system. However, *the beauty of linear algebra and vector math is that we can have vector spaces, or more strictly coordinate spaces, defined in arbitrary places.* **The important part is understanding the operations of matrices (ultimately the dot product) and holding a consistent structure to build out spaces.**

### 3D Matrix Representation

The rules for building a coordinate system in a matrix are rather straightforward. Matrices are these seemingly 2D data structures, but really the number of elements in a row (or column) define information that can be entirely independent of every other element in the matrix. Specifically we need independent basis vectors to represent each coordinate axis for a system we are representing, and we need a square matrix whose column and row dimensions are equal to the number of axes. So, for a 2D space, a 2x2 matrix is needed with 2 independent basis vectors. For 3D, a 3x3 matrix with 3 independent basis vectors is needed, and so on.

The entire data structure we will then need for a 3D coordinate system is a 3x3 matrix. For each column of this matrix, we populate a column with one of the basis vectors representing a single axis of this coordinate system. We might typically think of <1, 0, 0> as the x axis, <0, 1, 0> as the y axis, and <0, 0, 1> as the z axis, but any permutation of these bases in the columns of a 3x3 matrix will successfully define a coordinate system. What is important is that there is a consistency if these coordinate spaces need to map to a common space OR if the spaces need to be similar to one another. Moreover, the axis described above are trivial to observe their independence, but values could exist in all 3 of these basis vectors' components so long as each vector is orthogonal to one another (**e.g., we could rotate each axis by the same rotation and still use those modified bases to represent a coordinate system**).

The important consistency for tangent space is that a normal texture is representing a plane facing the forward axis (the third column of a matrix), which will tell us how to define the matrix to go into tangent space.

An example matrix defining a 3D coordinate system:

$$trivial 3D Coordinate System = \begin{bmatrix} 1 & 0 & 0 \\\ 0 & 1 & 0 \\\ 0 & 0 & 1 \end{bmatrix}$$

**Note**: A consistency that can be overlooked in software (**IFF using a common library for matrix operations**) is the *major order* of vectors in a matrix. Commonly in math, matrices are described with vectors as columns, so it will be described here in that manner. However, implementation in software packages can be such that vectors are stored in a matrix in *row* OR *column major order*. As a programmer, this should not influence your thoughts when defining a coordinate system because the implementation of the library will (likely, but hopefully) change to rows dotted with columns and columns dotted with rows, respectively and appropriately, for matrix multiplications.

### Defining a Tangent Space Matrix

Our tangent space matrix exists in 3D space and defines a rotation to the tangent plane of a triangle surface. The important consistency to consider is that the original normal map texture we want to transform exists in the plane defined by the first two axes of a coordinate space (first two columns of a 3D matrix) with the RGB data representing a normal vector facing some direction that is common to the third axis (the third column in a 3D matrix). **This means that the matrix to transform to tangent space must use vectors in the tangent plane for the first two columns of the matrix we build**.

The information needed to build the tangent space matrix is two vectors: the *normal vector* attribute from a vertex, and a *tangent vector* attribute from a vertex. High-level engine and graphics interfaces will likely provide these attributes for you, but if you ever get into graphics engines and lower-level graphics programming it is worth noting the following:

*There are only ever two normal vectors that can exist for a planar surface - the front facing or back facing normal* (**most graphics just want the front-facing or outward facing normal**). However, in continuous mathematics there is an infinite number of tangent vectors in a planar surface, because all of them exist in this plane and are orthogonal to the normal. In these situations, effectively any tangent vector can be chosen, but the manner in how it is chosen might need to be consistent for every planar surface.

With your two vertex attributes a new vector operation to be aware of will be required: **the cross product**. The *cross product* is not something that exist in all dimensions, but is extremely convenient in 3D space. ***The cross product of two vectors is a new vector that is orthogonal to the original two vectors!*** I cannot say this is cooler or more useful than the dot product, but it is very practical. In particular, we need to axis in the tangent space to define the tangent plane in a 3D matrix. While there are infinitely many tangent vectors, we cannot use any two in combination as distinct axes, they must be independent bases!

Effectively, all we need do is compute the cross product between the tangent and normal vectors. Because the normal vector is orthogonal to the tangent plane, and the tangent vector exists in one direction of the tangent plane, the resultant vector must also be in the tangent plane but orthogonal to the first tangent vector. This is the the ***bitangent vector*.

**Note**: If two vectors are parallel or antiparallel, then the cross product returns the zero vector.

Again, to align with the original description of the normal map texture, we want the forward axis to align with the surface normal direction, and the first two axes should describe the tangent plane. So, we can building a 3D matrix where ***the first column is the tangent vector, the second column is the bitangent vector, and the third column is the normal vector***. **This is commonly called a *TBN matrix***.

$$TBN Matrix = \begin{bmatrix} T_x & B_x & N_x \\\ T_y & B_y & N_y \\\ T_z & B_z & N_z \end{bmatrix}$$

**The only caveat is *if a mesh's model matrix defines a rotation from its local coordinate space, then the tangent, bitangent, and normal vectors in the tangent space matrix must also be rotated!***

**Note**: because we do not want to grow or shrink information transformed by the TBN, we should always consider normalizing the vectors before storing them in the columns of a matrix. Analytically, two orthogonal normalized vectors which have the cross product applied, will yield another normalized vector. However, given floating point error, enough deviations can occur that then require explicit normalization. Consequently, it is conservatively safest to always normalize the bitangent vector.

### Using a Tangent Space Matrix

Using the tangent space matrix is straightforward, we perform matrix multiplication with the TBN matrix on the *left* of the multiplication operator and the vector to transform on the *right* of the operator:

$transformed\:vector=TBN*vector$

To not complicate too much, this assignment will focus on a particular use case of the TBN matrix - ***transforming a vector sampled from the normal map to global space***. This will require passing the TBN matrix as separate vectors to the fragment shader, building the TBN matrix inside the fragment shader, and then transforming the sampled normal map vector to global space.

**Note:** This is actually a bit expensive compared to something else that could be done. In this scenario, we are transforming every single pixel processed by a TBN matrix, but we could have alternatively used the inverse TBN in the vertex shader to transform the global light position to the same space as the original normal map texture. This still works across the pixels in a fragment shader because the transformed light vector can be interpolated as it is passed along as a varying variable from the vertex shader. *Again, for simplicity we will be performing the more expensive task in this exercise.*

## Tasks

The assignment will have you completing three shaders in Unity, **DirTBN.shader**, **PointTBN.shader**, and **SpotTBN.shader**. The Unity project to open in this repository is **A0_LightsOn**. Instead of opening **DiffuseLighting** you will now open the **NormalMapLighting** scene in the *Assets/Scenes* folder. Download (or clone) and unzip the repository, open A0_LightsOn and open the NormalMapLighting scene if it is not open. Unity version 2021.3.1 was used for this project, but this project should be safe to open with any version of Unity 2021.3.X.

Remember that we are working in CG, a version of HLSL, and will need to use the mul(A, B) function to multiply two matrices.

One new function will need to be used in this shader assignment compared to A0:

- Cross product:
	- cross(vectorA, vectorB)

The *cross product is not commutative*, consider if you have an **up** and **right** vector, then there are two orthogonal vectors, one facing you and one facing away from you. The order for this assignment will be described below, but you will know if it is wrong if your scene seems too dark right after test playing.

The other functions needed are ones you already were exposed to in A0.

The first tasks to get out of the way are to complete the **"OLD TASKS"** found in each fragment shader:

- Complete the Lambertian Reflection dot product in DirTBN, PointTBN, and SpotTBN
- Complete the lightDir subtraction in PointTBN and SpotTBN
- Complete the coneTest dot product in SpotTBN

Then, there are two new tasks to complete for each shader (you can just copy and paste your solutions as these new tasks are the same for all shaders):

- In the Vertex shader (*NEW TASK 1*)
	- Compute the *bitangent vector*
	- Rotate the *bitangent vector* using the provided *rotMat* matrix and store it in the declared *float3 b* vector
		- The tangent and normal vectors are accessed in the input argument v using dot notation
			- E.g., *v.tangent*
	- Rotate the *tangent vector* using the provided *rotMat* matrix and store it in the declared *float3 t vector
		- An example of the rotation can be seen in *float3 n = mul(rotMat, v.normal)*
- In the Fragment shader (*NEW TASK 2*)
	- Set norms equal to itself transformed by the provided TBN matrix


*NEW TASK 1* will require a cross product between *v.tangent* and *n.tangent*. Finding the direction based on operand order can be described using a notion of "*handedness*". In this particular case, we are dealing with a left-handed coordinate system and want our bitangent vector to point in the same direction as the global x-axis (in the situation were the global coordinate system is mapped to the tangent plane). If you point your middle finger of your left hand away from you, and point your index finger up, the cross product of your middle finger and index finger will yield the same direction as your thumb if it is sticking out. In this handedness case, your middle finger should be the v.normal and your index finger should be v.tangent. If you were two flip these operands in the cross product, then the result would point in the opposite direction of your thumb.

The tangent vector is provided by Unity for your mesh inside the *appdata* struct. The *float3* vectors **b, t,** and **n** are passed to three vectors being sent as output to be potentially interpolated in transit to the fragment shader after rasterization.

The *norms* vector is initially storing the normal vector sampled from a new texture object uniform associated with the shaders. You do not need to sample from this texture, just transform norms in the *NEW TASK 2* line to reassign it with its modified values.

## Submission
