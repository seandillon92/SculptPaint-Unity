using System;
using UnityEngine;

[Serializable]
internal class Settings
{
    [SerializeField]
    internal BrushSettings brush;

    [SerializeField]
    internal ControlSettings control;

    [SerializeField]
    internal MaskSettings mask;

    [SerializeField]
    internal SculptSettings sculpt;
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
    internal Material material;

    [SerializeField]
    internal Texture texture;
}

[Serializable]
internal class MaskSettings
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
    internal Capture capture;
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
