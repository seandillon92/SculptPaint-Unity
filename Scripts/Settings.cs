using System;
using UnityEngine;

namespace PaintSculpt
{
    public enum Resolution
    {
        None = 0,
        R128 = 128,
        R256 = 256,
        R512 = 512,
        R1024 = 1024,
        R2048 = 2048,
        R4096 = 4096,
    }

    [Serializable]
    public class BrushSettings
    {
        [SerializeField]
        public float size = 0.25f;

        [SerializeField]
        public float minSize = 0.1f;

        [SerializeField]
        public float maxSize = 1f;

        [SerializeField]
        public float pressure = 0.1f;

        [SerializeField]
        public Texture texture;

        [SerializeField]
        public ProjectionType projection;

        public enum ProjectionType
        {
            LocalTangent = 0,
            GlobalTangent = 1,
        }

        [SerializeField]
        [Range(0, 360)]
        public float rotation = 0f;
    }

    [Serializable]
    public class PaintSettings
    {
        [SerializeField]
        public float dissipation = 0.1f;

        [SerializeField]
        public float delay = 5.0f;

        [SerializeField]
        public Camera camera;

        [SerializeField]
        public LayerMask layer;

        [SerializeField]
        public Material material;

        [SerializeField]
        private Resolution resolution;

        public int GetResolution() => (int)resolution;
    }

    [Serializable]
    public class SculptSettings
    {
        public enum Space
        {
            World = 0,
            Local = 1,
            Tangent = 2,
        }

        [SerializeField]
        public MeshFilter mesh;

        [SerializeField]
        public Camera camera;

        [SerializeField]
        public float strength;

        [SerializeField]
        public Space space;

        [SerializeField]
        public Vector3 direction;
    }
}
