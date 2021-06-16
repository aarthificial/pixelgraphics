# Shader properties
Once you have set up the velocity texture, you will gain access to a few shader properties. 
You can utilize them to create your own shaders.<br>
These properties can be accessed as follows, from within a shader graph:<br>
![Accesing a shader property](Images/shader_properties.png)<br>
(Make sure that `Exposed` is not checked)

### Velocity Texture
Reference: `_PG_VelocityTexture`<br>
Type:  `Texture2D`

The velocity texture itself.<br>
You can sample it using the coordinates returned from a `Screen Position` node.<br>
The retrieved value represents the velocity data for a given fragment/vertex. 

Channel | Data
--- | ---
R | Displacement along the X-axis
G | Displacement along the Y-axis
B | Velocity along the X-axis
A | Velocity along the Y-axis

### Pixel Screen Params
Reference: `_PG_PixelScreenParams`<br>
Type:  `Vector4`

Component | Data
--- | ---
X | Width of the camera measured in "pixel art" pixels
Y | Height of the camera measured in "pixel art" pixels
Z | Pixels per unit
W | 1 / Pixels per unit

### Velocity Simulation Params
Reference: `_PG_VelocitySimulationParams`<br>
Type:  `Vector4`

Component | Data
--- | ---
X | Stiffness
Y | Damping
Z | Blur strength
W | Max delta time
