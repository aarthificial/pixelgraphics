# Velocity Emitters
Objects that interact with the velocity texture are called velocity emitters.<br>
There are two main ways of setting them up.

## Standalone emitter
A standalone emitter is any renderer assigned to a layer dedicated for velocity emitters.<br>
This layer is excluded when rendering the scene making all standalone emitters invisible. 

1. Create a dedicated layer for velocity emitters (For example - "Velocity")

1. Setup the layer mask to contain the created layer<br>
![Layer Mask property](Images/layer_mask.png)

1. If you are using a render feature, exclude the layer in the filtering settings<br>
![Filtering > Transparent Layer Mask property](Images/culling_mask.png)<br>
(When using a velocity camera, the layer will be excluded autmatically)

1. Create an emitter<br>
![2D Object > Sprites > Velocity Emitter](Images/velocity_emitter_object.png)<br>
See the _Configuration_ section below for more info on how to configure an emitter.

## Integrated emitter
_(Render feature only)_<br>
Sometimes we want to use an existing renderer as an emitter while still having it appear on the screen.<br>
In cases like this, we can utilize rendering layers to use the same renderer for both rendering and emitting velocity.

1. Choose a rendering layer to use for velocity emitters (For example - "Layer2")<br>
![Rendering Layer Mask property](Images/rendering_layer_mask.png)

1. Find a renderer that you want to use as an emitter and add to it the __Velocity Emitter__ component<br>
![Add Component > PixelGraphics > Velocity Emitter](Images/velocity_emitter_component.png)<br>
See the _Configuration_ section below for more info on how to configure an emitter.

1. Make sure that the rendering layer you have picked is checked in the Rendering Layer Mask of the renderer.<br>
![Rendering Layer Mask property](Images/renderer_rendering_layer_mask.png)

## Configuration

### `Mode`
The emitter mode decides what should be the source of velocity for this emitter.<br>
There are four modes available:

Mode | Description
--- | ---
Translation | Velocity is calculated based on the change of position between frames
Rigidbody | Velocity is read from a Rigidbody component
Rigidbody2D | Velocity is read from a Rigidbody2D component
Custom | Velocity is read from a public `customVelocity` field

### `Max Speed`
The maximum speed for this emitter.<br>
The actuall speed of an emitter is remapped so that, upon reaching the maximum speed, the resulting speed will be equal to 1.
Anything above that will also result in 1 as well.  

### `Remapping`
A curve that allows you to fine-tune how the speed is remapped.<br>
X-axis is the real speed (0 = 0, 1 = maxSpeed).<br>
Y-axis is the speed passed to the velocity texture.

Pseudocode describing how the final speed is calculated:
```
speed = remapping(clamp01(length(velocity) / maxSpeed))
```

## Custom emitters
Velocity emitters are not the only way to interact with the buffer.
In fact, any sprite assigned to our velocity layer can do that.

You can use [the default emitter shader](../Runtime/Shaders/Velocity/Emitter.shader) as a template 
and create your own shaders that will interact with the buffer differently.

---

Once you have set up a few emitters, you can use [the available shaders](./shaders.md) to see them in action.