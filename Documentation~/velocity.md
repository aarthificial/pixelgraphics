# Velocity Texture
The velocity texture is a screen space buffer that stores current velocity of specific objects (emitters) and performs
a simple, spring based simulation for every pixel on the screen.<br> 
It can be used to cheaply fake displacement effects caused by moving objects. 
For example, grass swaying when the player passes by. 

The velocity texture consists of four channels:

Channel | Data
--- | ---
R | Displacement along the X-axis
G | Displacement along the Y-axis
B | Velocity along the X-axis
A | Velocity along the Y-axis

## Setup
The velocity texture can be set up using two different methods.<br>
The preferred one is to add a `Velocity Render Feature` to your renderer.

Unfortuantely, as of writing this, only the Forward Renderer supports render features.<br>
If you are using a different renderer (the 2D Renderer, for example) 
add a `Velocity Camera` component to your main camera instead. 

(In the future (2021.2) the 2D Renderer will also support render features)

Adding a render feature | Adding a camera component 
--- | ---
![Add Render Feature > Velocity Render Feature](Images/render_feature.png) | ![Add Component > PixelGraphics > Velocity Camera](Images/velocity_camera.png) 

## Configuration
Both the render feature and the camera component use the same configuration.<br>
The only difference being that the component does not support the rendering layer mask.

Render feature | Camera component 
--- | ---
![Add Render Feature](Images/render_feature_ui.png) | ![Velocity Camera](Images/velocity_camera_ui.png)

### `Pixels Per Unit` 
The amount of pixels that make up one unit of the scene.<br>
Set this value to match the PPU value of sprites in the scene.

### `Rendering Layer Mask`
_(Render feature only)_<br>
Rendering layers used for [integrated emitters](./emitters.md#integrated-emitter)

### `Layer Mask`
Normal layers used for [standalone emitters](./emitters.md#standalone-emitter)

### `Texture Scale`
The resolution of the velocity texture relative to the screen resolution.<br>
1.0 means full screen size.

### `Preview`
_(Render feature only)_<br>
Displays the velocity texture on the screen. For debugging only.

![No blur](Images/velocity_texture_preview.png)

### `Stiffness`
The spring constant.<br>
The higher the value the stiffer the spring.

### `Damping`
The linear damping coefficient. Causes the spring to slow down and eventually reach equilibrium.<br>
Setting it to 0 will cause the spring to oscillate (almost) indefinitely.

### `Blur Strength`
The strength of the blur.<br>
Other factors, like the `Texture Scale`, can also affect blurring.

Blur = 0 | Blur = 1
--- | ---
![No blur](Images/velocity_texture_no_blur.png) | ![Blur](Images/velocity_texture_blur.png)

### `Max Delta Time`
The maximum delta time in seconds. Used to keep the simulation stable in case of FPS drops.<br> 
Should be higher than the average delta time of your targeted frame rate. The default value is 1/30 (30 FPS)

---

Once you have set up a render texture, you can start [placing emitters in your scene](./emitters.md)