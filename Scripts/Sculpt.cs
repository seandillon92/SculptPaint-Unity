using UnityEngine;
using UnityEngine.Rendering;

internal class Sculpt
{
    private Settings m_settings;
    private ComputeShader m_shader;
    private int m_kernel;
    private int m_threadGroupX;
    private Mesh m_mesh;
    private int m_MVP;
    private int m_direction;
    internal Sculpt(Settings settings)
    {
        m_settings = settings;
        m_shader = Resources.Load<ComputeShader>("Sculpt");
        m_kernel = m_shader.FindKernel("Update");
        m_MVP = Shader.PropertyToID("mvp");
        m_direction = Shader.PropertyToID("direction");


        m_mesh = m_settings.sculpt.mesh.mesh;
        m_mesh.vertexBufferTarget |= GraphicsBuffer.Target.Raw;

        using var buffer = m_mesh.GetVertexBuffer(0);

        m_shader.SetBuffer(m_kernel, "vertices", buffer);

        m_shader.GetKernelThreadGroupSizes(m_kernel, out uint x, out uint y, out uint z);
        m_threadGroupX = Mathf.CeilToInt(buffer.count / x);

        m_shader.SetInt("stride", m_mesh.GetVertexBufferStride(0));
        m_shader.SetInt("size", 4);
        
        var position = m_mesh.GetVertexAttributeOffset(VertexAttribute.Position);
        var normal = m_mesh.GetVertexAttributeOffset(VertexAttribute.Normal);
        var tangent = m_mesh.GetVertexAttributeOffset(VertexAttribute.Tangent);

        m_shader.SetInt("offset_pos", position);
        m_shader.SetInt("offset_norm", normal);
        m_shader.SetInt("offset_tangent", tangent);

        m_shader.SetTexture(m_kernel, "brushTexture", m_settings.brush.texture);
    }

    internal void Update(
        Matrix4x4 mvp,
        Vector3 screenPos, 
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

        direction *= Time.deltaTime * m_settings.sculpt.strength;

        m_shader.SetMatrix(m_MVP, mvp);
        m_shader.SetVector(m_direction, direction);
        var input = m_settings.mask.camera.ScreenToViewportPoint(screenPos);
        var position = new Vector2(input.x, input.y);
        m_shader.SetVector("mousePos", position);
        m_shader.SetFloat("aspect", m_settings.sculpt.camera.aspect);
        m_shader.SetFloat("brushSize", 1.0f / m_settings.brush.size);
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
