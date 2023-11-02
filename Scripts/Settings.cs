using PlasticGui.WorkspaceWindow.Merge;
using System;
using System.Runtime.InteropServices.WindowsRuntime;
using UnityEngine;
using UnityEngine.Rendering;

[Serializable]
internal class Settings
{
    [SerializeField]
    internal BrushSettings brush;

    [SerializeField]
    internal ControlSettings control;

    [SerializeField]
    internal PaintSettings paint;

    [SerializeField]
    internal SculptSettings sculpt;

    internal enum Resolution
    {
        None = 0,
        R128 = 128,
        R256 = 256,
        R512 = 512,
        R1024 = 1024,
        R2048 = 2048,
        R4096 = 4096,
    }
}

[Serializable]
internal class ControlSettings
{
    [SerializeField]
    internal float rotateVelocity = 0.1f;

    [SerializeField]
    internal float sizeChangeSpeed = 0.1f;

    [SerializeField]
    internal float distanceMin = 1;

    [SerializeField]
    internal float distanceMax = 10;

    [SerializeField]
    [Range(0, 1)]
    internal float distance = 0.5f;

    [SerializeField]
    internal float distanceChangeSpeed = 1.0f;

    [SerializeField]
    internal Transform transform;
}

[Serializable]
internal class BrushSettings
{
    [SerializeField]
    internal float size = 0.25f;

    [SerializeField]
    internal float minSize = 0.1f;

    [SerializeField]
    internal float maxSize = 1f;

    [SerializeField]
    internal float pressure = 0.1f;

    [SerializeField]
    internal Texture texture;

    [SerializeField]
    internal float rotation = 0f;
}

[Serializable]
internal class PaintSettings
{
    [SerializeField]
    internal float dissipation = 0.1f;

    [SerializeField]
    internal float delay = 5.0f;

    [SerializeField]
    internal Camera camera;

    [SerializeField]
    internal LayerMask layer;

    [SerializeField]
    internal Material material;

    [SerializeField]
    private Settings.Resolution resolution;

    internal int GetResolution() => (int)resolution;
}

[Serializable]
internal class SculptSettings
{
    internal enum Space
    {
        World = 0, 
        Local = 1,
        Normal = 2,
    }

    [SerializeField]
    internal MeshFilter mesh;

    [SerializeField]
    internal Camera camera;

    [SerializeField]
    internal float strength;

    [SerializeField]
    internal Space space;

    [SerializeField]
    internal Vector3 direction;
}
