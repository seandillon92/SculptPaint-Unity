using System;
using System.Collections.Generic;
using UnityEngine;
internal class Paint
{
    private ComputeShader m_shader;
    private int m_dissipate;
    private int m_stamp;

    private Settings m_settings;
    private PingPongBuffer m_buffer;

    private Capture m_capture;

    internal Paint(Settings settings, List<MeshRenderer> renderer)
    {
        m_shader = Resources.Load<ComputeShader>("Paint");
        m_dissipate = m_shader.FindKernel("Dissipate");
        m_stamp = m_shader.FindKernel("Stamp");

        m_settings = settings;

        var resolution = m_settings.paint.GetResolution();
        var read = new RenderTexture(resolution, resolution, 1, RenderTextureFormat.RHalf);
        read.enableRandomWrite = true;

        var write = new RenderTexture(resolution, resolution, 1, RenderTextureFormat.RHalf);
        write.enableRandomWrite = true;

        if (!read.Create())
        {
            throw new Exception("Could not create texture");
        }
        if (!write.Create())
        {
            throw new Exception("Could not create texture");
        }

        m_buffer = new PingPongBuffer(read, write);

        m_capture = 
            new Capture(
                m_settings.paint.camera, 
                m_settings.paint.layer, 
                renderer, 
                m_settings.paint.material);
    }

    internal RenderTexture Write(Vector3 position)
    {
        var material = m_settings.paint.material;
        material.SetVector("position", position);

        m_settings.paint.material.SetFloat("radius", 1.0f / m_settings.brush.size);
        m_capture.Update(m_buffer.read);

        m_shader.SetTexture(m_stamp, "Read", m_buffer.read);
        m_shader.SetTexture(m_stamp, "Write", m_buffer.write);
        m_shader.SetFloat("delay", m_settings.paint.delay);

        m_shader.GetKernelThreadGroupSizes(m_stamp, out uint x, out uint y, out _);
        m_shader.Dispatch(m_stamp, (int)(m_buffer.read.width / x), (int)(m_buffer.read.height / y), 1);

        return m_buffer.write;
    }

    internal void Update()
    {
        return;
        m_shader.SetTexture(m_dissipate, "Write", m_buffer.write);
        m_shader.SetTexture(m_dissipate, "Read", m_buffer.read);

        m_shader.SetFloat("dissipation", Time.deltaTime * m_settings.paint.dissipation);

        m_shader.GetKernelThreadGroupSizes(m_dissipate, out uint x, out uint y, out _);
        m_shader.Dispatch(m_dissipate, (int)(m_buffer.read.width / x), (int)(m_buffer.read.height / y), 1);

        m_buffer.Swap();
    }
}