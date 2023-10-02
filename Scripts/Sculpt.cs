using UnityEngine;

internal class Sculpt
{
    private Settings m_settings;
    private ComputeShader m_shader;
    private int m_kernel;
    private int m_threadGroupX;
    private Mesh m_mesh;
    private int m_MVP;
    internal Sculpt(Settings settings)
    {
        m_settings = settings;
        m_shader = Resources.Load<ComputeShader>("Sculpt");
        m_kernel = m_shader.FindKernel("Update");
        m_MVP = Shader.PropertyToID("mvp");


        m_mesh = m_settings.sculpt.mesh.sharedMesh;
        m_mesh.vertexBufferTarget |= GraphicsBuffer.Target.Raw;

        using var buffer = m_mesh.GetVertexBuffer(0);

        m_shader.SetBuffer(m_kernel, "vertices", buffer);

        m_shader.GetKernelThreadGroupSizes(m_kernel, out uint x, out uint y, out uint z);
        m_threadGroupX = Mathf.CeilToInt(buffer.count / x);

        m_shader.SetInt("stride", m_mesh.GetVertexBufferStride(0));
        m_shader.SetInt("size", 4);

        m_shader.SetTexture(m_kernel, "brushTexture", m_settings.brush.texture);

        // Debug.Log(m_mesh.GetVertexAttributeOffset(UnityEngine.Rendering.VertexAttribute.TexCoord0));
    }

    internal void Update(Vector3 screenPos, float deformation)
    {
            var model = m_settings.sculpt.mesh.transform.localToWorldMatrix;
            var view = m_settings.sculpt.camera.worldToCameraMatrix;
            var projection = m_settings.sculpt.camera.projectionMatrix;
                //GL.GetGPUProjectionMatrix(m_settings.sculpt.camera.projectionMatrix, false);
            var mvp = projection * view * model;

            m_shader.SetMatrix(m_MVP, mvp);

            var input = m_settings.mask.camera.ScreenToViewportPoint(screenPos);
            var position = new Vector2(input.x, input.y);
            m_shader.SetVector("mousePos", position);
            m_shader.SetFloat("aspect", m_settings.sculpt.camera.aspect);
            m_shader.SetFloat("brushSize",1.0f/m_settings.brush.size);
            m_shader.SetFloat("maxDeformation", deformation);
            RenderDocCapture.RunWithCapture(() => { m_shader.Dispatch(m_kernel, m_threadGroupX, 1, 1); }, 1);
    }
}
