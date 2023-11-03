using UnityEngine;
using UnityEngine.Rendering;

internal class Sculpt
{
    internal Mesh mesh { get; private set; }

    private Settings m_settings;
    private ComputeShader m_shader;
    private int m_kernel;
    private int m_threadGroupX;
    private int m_model;
    private int m_direction;

    internal Sculpt(Settings settings)
    {
        m_settings = settings;
        m_shader = Resources.Load<ComputeShader>("Sculpt");
        m_kernel = m_shader.FindKernel("Update");
        m_model = Shader.PropertyToID("mvp");
        m_direction = Shader.PropertyToID("direction");


        mesh = m_settings.sculpt.mesh.mesh;
        mesh.vertexBufferTarget |= GraphicsBuffer.Target.Raw;

        using var buffer = mesh.GetVertexBuffer(0);

        m_shader.SetBuffer(m_kernel, "vertices", buffer);

        m_shader.GetKernelThreadGroupSizes(m_kernel, out uint x, out uint y, out uint z);
        m_threadGroupX = Mathf.CeilToInt(buffer.count / x);

        m_shader.SetInt("stride", mesh.GetVertexBufferStride(0));
        m_shader.SetInt("size", 4);
        
        var position = mesh.GetVertexAttributeOffset(VertexAttribute.Position);
        var normal = mesh.GetVertexAttributeOffset(VertexAttribute.Normal);
        var tangent = mesh.GetVertexAttributeOffset(VertexAttribute.Tangent);

        m_shader.SetInt("offset_pos", position);
        m_shader.SetInt("offset_norm", normal);
        m_shader.SetInt("offset_tangent", tangent);

        m_shader.SetTexture(m_kernel, "brushTexture", m_settings.brush.texture);
    }

    internal void Update(
        Vector3 position,
        Vector3 normal,
        Vector3 scale,
        float aspect,
        float brushSize,
        float deformation)
    {
        var direction = m_settings.sculpt.direction;
        var space = m_settings.sculpt.space;
        if (space == SculptSettings.Space.World)
        {
            direction =  
                m_settings.sculpt.mesh.transform.worldToLocalMatrix.MultiplyVector(direction).normalized;
        }

        m_shader.SetInt("space", (int)space);
        m_shader.SetInt("brushSpace", (int)m_settings.brush.projection);

        direction *= Time.deltaTime * m_settings.sculpt.strength;
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
        if (m_threadGroupX % maxThreadSize > 0 )
        {
            m_shader.SetInt("iteration", iterations);
            m_shader.Dispatch(m_kernel, m_threadGroupX % maxThreadSize, 1, 1);
        }

    }
}
