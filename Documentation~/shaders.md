# Available Shaders

### `foliage_shader` and `foliage_pixelart_shader`
Simple wind simulation and velocity reaction.<br>
Sways in any direction.<br>
Best for bushes/trees and other foliage suspended in the air. 

### `grass_shader` and `grass_pixelart_shader`
Simple wind simulation and velocity reaction.<br>
Sways back and forth along its right axis.<br>
Best for grass/flowers and other small foliage that grows out of the ground.

## Variants
Each shader is available in two variants - normal and pixelart.

| Default sprite | grass_shader | grass_pixelart_shader |
| --- | --- | --- |
|![Default texture](Images/shader_variant_default.png)|![Normal variant](Images/shader_variant_normal.png)|![Pixelart variant](Images/shader_variant_pixelart.png)

### Pixelart shaders
Pros:
- Pixel-perfect distortion at higher resolutions

Cons:
- Computationally heavy
- Can be used only with normal sprite renderers.

### Normal shaders
Pros:
- Lightweight
- Can be used with any renderer
- Can be used for *non* pixel art assets 

Cons:
- Pixel-perfect effect requires upscaling

## Preparing mesh
Depending on what shader variant you are using, you will need to prepare your sprites differently.

Normal shaders displace the vertices of a mesh. 
To improve the effect, a mesh needs to be additionally subdivided. 
This can be done using the Custom Outline mode of the Sprite Editor (see the table below).

Pixelart shaders displace only the uv coordinates, the mesh itself stays the same. 
To make it work, the mesh needs to be bigger than the actual sprite to account for any possible displacement.
This can be done using the Sprite Editor (see the table below) or the import settings.  
 
| | Normal shader | Pixelart shader |
| --- | --- | --- |
| Outline | ![Outline for normal shaders](Images/outline_normal.png) | ![Outline for pixelart shaders](Images/outline_pixelart.png) |
| Mesh | ![Mesh for normal shaders](Images/outline_mesh_normal.png) | ![Mesh for pixelart shaders](Images/outline_mesh_pixelart.png) |

## Configuration

### `Velocity Strength`
How much the material is distorted by the velocity texture.

### `Wind Velocity`
2D Vector defining direction and speed of the wind.

### `Wind Strength`
How much the material is distorted by the wind.

### `Wind Scale`
Scale of the noise used to generate the wind.

## Displacement Mask
The distortion can be further controlled by using a secondary texture called `_DisplacementMask`

| Channel | Function |
| --- | --- |
| R | Controls how much a given fragment/vertex get displaced by the velocity texture |
| G | Controls how much a given fragment/vertex get displaced by the wind |

Secondary textures are assigned from within the sprite editor:

![Assigning a secondary texture](Images/secondary_texture.png)

You can use a displacement mask to, for example, keep the branches of a bush in place:

| Texture | Displacement mask |
| --- | --- |
|![Texture](Images/displacement_texture.png)|![Displacement mask](Images/displacement_mask.png)|

---

These shaders are just an example of what is possible. You can use [the exposed properties](./emitters.md) to create your own interactive shaders.
