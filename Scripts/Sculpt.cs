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
        private int m_model;
        private int m_direction;

        public Sculpt(SculptSettings settings, BrushSettings brushSettings)
        {
            m_settings = settings;
            m_brushSettings = brushSettings;
            m_shader = Resources.Load<ComputeShader>("Sculpt");
            m_kernel = m_shader.FindKernel("Update");
            m_model = Shader.PropertyToID("mvp");
            m_direction = Shader.PropertyToID("direction");


            Mesh = m_settings.mesh.mesh;
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
            Vector3 scale,
            float aspect,
            float brushSize,
            float deformation)
        {
            var direction = m_settings.direction;
            var space = m_settings.space;
            if (space == SculptSettings.Space.World)
            {
                direction =
                    m_settings.mesh.transform.worldToLocalMatrix.MultiplyVector(direction).normalized;
            }

            m_shader.SetInt("space", (int)space);
            m_shader.SetInt("brushSpace", (int)m_brushSettings.projection);

            direction *= Time.deltaTime * m_settings.strength;
            m_shader.SetVector(m_direction, direction);

            Vector3 tangent = Vector3.Cross(normal, Vector3.up);
            Vector3 bitangent = Vector3.Cross(tangent, normal);

            m_shader.SetVector("position", position);
            m_shader.SetVector("tangent", tangent.normalized);
            m_shader.SetVector("bitangent", bitangent.normalized);
            m_shader.SetVector("normal", normal.normalized);

            m_shader.SetFloat("aspect", aspect);
            m_shader.SetVector("scale", scale);

            m_shader.SetFloat("radius", 1.0f / brushSize);
            m_shader.SetFloat("maxDeformation", deformation);
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
