![Demo](Docs/LavaDemo.gif)
# SculptPaint-Unity
Sculpting and Painting package for Unity Runtime. Usefull for prototyping visual effects with world space painting and sculpting.

## Features
### **Painting**
![Paint](Docs/Stamp.png)
Paint textures on your models, using world space coordinates. Stamp decals that take the surface shape into account. 

### **Sculpting**
![Sculpt](Docs/Sculpt.png)
Change the geometry of your model, using world space coordinates. Everything is done on the GPU without any copies. Apply a brush on the model to change position of vertices towards a direction in local, world or tagent space.

### **Material Blending**
Blend any type of material without writting new shaders for blending.

## Supported Platforms
### Win, Mac, Linux :heavy_check_mark: 
Developed and tested on Win64 and DX11/12. All other desktop platforms and APIs should work but have not been tested.

### Android, iOS ✔️
Builds are possible, though code has been developed and optimized only for desktop build targets.

### WebGL ❌
WebGL does not support Compute Shaders.

## Supported Rendering Pipelines
* Built-In (Standard) ✔️
* URP ❌
* HDRP ❌

## Supported Editor Versions
* Unity 2022 LTS ✔️
* Unity 2021 LTS ✔️

## Installation
Install via the package manager. See https://docs.unity3d.com/Manual/upm-ui-giturl.html

## Usage
### 1. Create a custom behavior script
###

## Credits
Originally, the system was working only in screen space. I implemented the approach described in [1] to convert it into world space. I extended it with decals (brushes) and my own solution for the rasterization artefact.
### References
[1] Mesh Texture Painting in Unity using Shaders - Shahriar Shahrabi. https://shahriyarshahrabi.medium.com/mesh-texture-painting-in-unity-using-shaders-8eb7fc31221c
