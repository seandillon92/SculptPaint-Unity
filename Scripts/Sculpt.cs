using UnityEngine;
using UnityEngine.Rendering;

namespace PaintSculpt
{
    public class Sculpt
    {
        public Mesh Mesh { get; private set; }

        private SculptSettings m_settings;
        private BrushSettings m_brushSettings;
        private ComputeShader m_shader;
        private int m_kernel;
        private int m_threadGroupX;
        private int m_direction;

        private Transform m_transform;

        /// <summary>
        /// Create a Sculpt component to change the vertex positions of a mesh.
        /// </summary>
        /// <param name="settings"> Sculpt settings</param>
        /// <param name="brushSettings"> Brush settings </param>
        /// <param name="mesh">MeshFilter containing the mesh to be modifier. 
        /// Only an instance of the mesh will be modified instead the original mesh.</param>
        public Sculpt(SculptSettings settings, BrushSettings brushSettings, MeshFilter mesh)
        {
            m_settings = settings;
            m_brushSettings = brushSettings;
            m_shader = Resources.Load<ComputeShader>("Sculpt");
            m_kernel = m_shader.FindKernel("Update");
            m_direction = Shader.PropertyToID("direction");
            m_transform = mesh.transform;

            Mesh = mesh.mesh;
            Mesh.vertexBufferTarget |= GraphicsBuffer.Target.Raw;

            using var buffer = Mesh.GetVertexBuffer(0);

            m_shader.SetBuffer(m_kernel, "vertices", buffer);

            m_shader.GetKernelThreadGroupSizes(m_kernel, out uint x, out uint y, out uint z);
            m_threadGroupX = Mathf.CeilToInt(buffer.count / x);

            m_shader.SetInt("stride", Mesh.GetVertexBufferStride(0));
            m_shader.SetInt("size", 4);

            var position = Mesh.GetVertexAttributeOffset(VertexAttribute.Position);
            var normal = Mesh.GetVertexAttributeOffset(VertexAttribute.Normal);
            var tangent = Mesh.GetVertexAttributeOffset(VertexAttribute.Tangent);

            m_shader.SetInt("offset_pos", position);
            m_shader.SetInt("offset_norm", normal);
            m_shader.SetInt("offset_tangent", tangent);

            m_shader.SetTexture(m_kernel, "brushTexture", m_brushSettings.texture);
        }

        public void Update(
            Vector3 position,
            Vector3 normal,
            Vector3 forward)
        {
            var direction = m_settings.direction;
            var space = m_settings.space;
            if (space == SculptSettings.Space.World)
            {
                direction =
                    m_transform.worldToLocalMatrix.MultiplyVector(direction).normalized;
            }

            m_shader.SetInt("space", (int)space);
            m_shader.SetInt("brushSpace", (int)m_brushSettings.projection);

            direction *= Time.deltaTime * m_settings.strength;
            m_shader.SetVector(m_direction, direction);

            Vector3 tangent = Vector3.ProjectOnPlane(forward, normal);
            tangent = Quaternion.Euler(normal * m_brushSettings.rotation) * tangent;
            Vector3 bitangent = Vector3.Cross(tangent, normal);

            m_shader.SetVector("position", position);
            m_shader.SetVector("tangent", tangent.normalized);
            m_shader.SetVector("bitangent", bitangent.normalized);
            m_shader.SetVector("normal", normal.normalized);
            m_shader.SetVector("forward", forward.normalized);
            m_shader.SetFloat("rotation", m_brushSettings.rotation);

            m_shader.SetFloat("aspect", m_brushSettings.Aspect);
            m_shader.SetVector("scale", m_transform.lossyScale);

            m_shader.SetFloat("radius", 1.0f / m_brushSettings.size);
            m_shader.SetVector(m_direction, direction);
            var maxThreadSize = 65535;
            var iterations = m_threadGroupX / maxThreadSize;

            m_shader.SetInt("iteration_offset", maxThreadSize);
            for (int i = 0; i < iterations; i++)
            {
                m_shader.SetInt("iteration", i);

                m_shader.Dispatch(m_kernel, maxThreadSize, 1, 1);
            }
            if (m_threadGroupX % maxThreadSize > 0)
            {
                m_shader.SetInt("iteration", iterations);
                m_shader.Dispatch(m_kernel, m_threadGroupX % maxThreadSize, 1, 1);
            }

        }
    }
}
