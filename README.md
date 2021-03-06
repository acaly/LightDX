# LightDX
[![NuGet](https://img.shields.io/nuget/v/LightDx.svg)](https://www.nuget.org/packages/LightDx/)

LightDX is a graphics library in C#. It is designed to be used for those who want
to use DirectX for accelerated rendering (visualization or a simple game). It supports
most important funtions in Direct3D, but heavily relys on .NET Framework on other works.

## **Note**
This is a work in progress, so public APIs are expected to have breaking changes.

# Features
* **Lightweight.**
No dependencies except the Framework. Less than 100KB after compiled. You can just
copy the source to your project and include them (though it requires unsafe). Even
if you don't want to directly include them, you can still have everything in a
single small DLL (SharpDX can generously give you 7). No native DLL is needed, so it
works on 'AnyCPU'.
* **Clean.**
Focus on Direct3D. Use the .NET Framework as much as possible: Bitmap format
conversion, font rendering, mouse and keyboard input, vector and matrix math.
Fortunately the .NET Framework API is well designed and can be directly used here,
which avoids tons of work.
* **Easy to use.**
Unlike SharpDX or SlimDX, LightDX is not a DirectX binding. Therefore it hides
the complicated details of creating each components and only provides simplified
API, which makes it much easier to use. This may have some limits, but hopefully 
LightDX will provide everything you really need.
* **Fast.**
LightDX utilizes calli instruction to call native COM methods, as
SharpDX does. This should be the fastest method. Other parts are also written with
efficiency in mind (at least to my best).

# Limitations
Limitations in design:
* Single thread API. No multithread rendering.
* Only supports Windows desktop application. Other platforms are not tested.

Limitations in current implementation (may be fixed in the future):
* Only support Texture2D as ShaderResource.

... and many other not supported features in DX11...

# How to use
Nuget package ```LightDx``` is available now. (Only .NET Framework 4.7 is supported.)

Please check the following projects that uses LightDx:
* [Examples](Examples) folder.
* [DirectX 11 Tutorial](https://github.com/acaly/LightDx.DirectX11Tutorials), implementing 
some of the examples from http://rastertek.com/tutdx11.html.
* [ImGuiOnLightDx](https://github.com/acaly/ImGuiOnLightDx), a simple implementation of 
the ImGui backend.
* [MMDRenderer](https://github.com/acaly/MMDRenderer), a simple MMD model renderer using 
deferred rendering pipeline.
* [Sandbox](https://github.com/acaly/Sandbox), a high-performance Minecraft-like world 
rendering engine.
* [VoxelModelEditor](https://github.com/acaly/VoxelModelEditor), a model and animation 
editor using WinForms.

### About Cube example

* It's outside the examples folder by mistake.
* The `Camera` class should have a `GetViewMatrix` method:

```c#
    public Matrix4x4 GetViewMatrix()
    {
        return MatrixHelper.CreateLookAt(Position, Position + CalcOffset(), Vector3.UnitZ).Transpose();
    }
```
