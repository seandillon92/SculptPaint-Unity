using System;
using UnityEngine;

internal class Sculpt : IDisposable
{
    private Settings m_settings;
    private ComputeShader m_shader;
    private int m_kernel;
    private int m_threadGroupX;
    private Mesh m_mesh;
    private GraphicsBuffer m_buffer;
    internal Sculpt(Settings settings)
    {
        m_settings = settings;
        m_shader = Resources.Load<ComputeShader>("Sculpt");
        m_kernel = m_shader.FindKernel("Update");

        m_mesh = m_settings.sculpt.mesh.sharedMesh;

        m_mesh.vertexBufferTarget |= GraphicsBuffer.Target.Raw;
        m_buffer = m_mesh.GetVertexBuffer(0);
        m_shader.SetBuffer(m_kernel, "vertices", m_buffer);

        m_shader.GetKernelThreadGroupSizes(m_kernel, out uint x, out uint y, out uint z);
        m_threadGroupX = Mathf.CeilToInt( m_buffer.count/ x);

        m_shader.SetInt("stride", m_mesh.GetVertexBufferStride(0));
        m_shader.SetInt("size", 4);

       // Debug.Log(m_mesh.GetVertexAttributeOffset(UnityEngine.Rendering.VertexAttribute.TexCoord0));
    }

    public void Dispose()
    {
        m_buffer.Dispose();
    }

    internal void Update()
    {
        if (Input.GetMouseButton(0))
        {
            m_shader.Dispatch(m_kernel, m_threadGroupX, 1, 1);
        }
    }
}
