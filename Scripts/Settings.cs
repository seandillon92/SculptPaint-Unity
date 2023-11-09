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
        R8192 = 8192,
        R16384 = 16384,
    }

    static class ResolutionExtension
    {
        public static int GetInt(this Resolution duration)
        {
            return (int)duration;
        }
    }

    [Serializable]
    public class BrushSettings
    {
        /// <summary>
        /// Size of the brush.
        /// </summary>
        [SerializeField]
        public float size = 0.25f;

        /// <summary>
        /// The brush texture.
        /// </summary>
        [SerializeField]
        public Texture texture;

        /// <summary>
        /// The type of projection used when the brush is projected on the model.
        /// Local Tangent uses a tagnent space defined by the surface normal and a user-defined forward vector.
        /// Global Tangent uses a tangent space defined by user-defined normal and forward vectors.
        /// </summary>
        [SerializeField]
        public ProjectionType projection;

        public enum ProjectionType
        {
            LocalTangent = 0,
            GlobalTangent = 1,
        }

        /// <summary>
        /// Rotation of the brush in euler angles.
        /// </summary>
        [SerializeField]
        [Range(0, 360)]
        public float rotation = 0f;

        public float Aspect => texture.width/((float)texture.height);
    }

    [Serializable]
    public class PaintSettings
    {
        /// <summary>
        /// How fast does the mask change. Dissipation x will change a normalized mask channel by x per second.
        /// i.e. it will reduce a channel from 1 to 0 in 1/x seconds. Dissipation of 0 means that the mask does not change.
        /// </summary>
        [SerializeField]
        public float dissipation = 0.1f;

        /// <summary>
        /// Delay time until the mask starts changing. Delay of x will trigger the dissipation after x seconds.
        /// </summary>
        [SerializeField]
        public float delay = 5.0f;

        /// <summary>
        /// Camera to be used for paint operations. It is recomended to use a disabled secondary camera, to avoid issues with changing your game cameras.
        /// </summary>
        [SerializeField]
        public Camera camera;

        /// <summary>
        /// Paint material, set a serialized property to stop the build system from stripping away the shader. 
        /// Assign it to Packages/com.sean-dillon.sculpt-paint-runtime/Assets/MaterialPaint.mat
        /// </summary>
        [SerializeField]
        public Material material;

        /// <summary>
        /// Resolution of the texture used for painting. Use GetInt to get the dimension as integer.
        /// </summary>
        [SerializeField]
        public Resolution resolution = Resolution.R1024;
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

        /// <summary>
        /// The strength of the sculpting force
        /// </summary>
        [SerializeField]
        public float strength;

        /// <summary>
        /// The space that the sculpting direction is defined.
        /// </summary>
        [SerializeField]
        public Space space;

        /// <summary>
        /// The direction of the sculpting force, relative to the selected space.
        /// </summary>
        [SerializeField]
        public Vector3 direction;
    }
}
