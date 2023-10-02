using System;
using UnityEngine;
internal class Mask
{
    private ComputeShader m_shader;
    private int m_writeKernel;
    private int m_updateKernel;
    private Settings m_settings;
    private PingPongBuffer m_buffer;

    internal Mask(Settings settings)
    {
        m_shader = Resources.Load<ComputeShader>("Mask");
        m_writeKernel = m_shader.FindKernel("WriteMask");
        m_updateKernel = m_shader.FindKernel("UpdateMask");
        m_settings = settings;

        m_settings.mask.capture.Init(m_settings.mask.camera, m_settings.mask.layer);

        var read = new RenderTexture(2048, 2048, 1, RenderTextureFormat.ARGBHalf);
        read.enableRandomWrite = true;

        var write = new RenderTexture(2048, 2048, 1, RenderTextureFormat.ARGBHalf);
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
    }

    internal void Update(bool write)
    {
        UpdateCaptures();
        UpdateMask(write);
    }

    private void UpdateCaptures()
    {
        m_settings.mask.capture.Update();
    }

    private void UpdateMask(bool write)
    {
        if (write)
        {
            m_shader.SetTexture(m_writeKernel, "Write", m_buffer.read);
            m_shader.SetTexture(m_writeKernel, "Read1", m_settings.mask.capture.texture);

            m_shader.SetFloat("pressure", m_settings.brush.pressure * Time.deltaTime);
            m_shader.GetKernelThreadGroupSizes(m_writeKernel, out uint x, out uint y, out _);
            m_shader.Dispatch(
                m_writeKernel,
                (int)(m_settings.mask.capture.texture.width / x),
                (int)(m_settings.mask.capture.texture.height / y), 1);
        }

        {
            m_shader.SetTexture(m_updateKernel, "Write", m_buffer.write);
            m_shader.SetTexture(m_updateKernel, "Read1", m_buffer.read);
            m_shader.GetKernelThreadGroupSizes(m_updateKernel, out uint x, out uint y, out _);
            m_shader.Dispatch(m_updateKernel, (int)(m_buffer.read.width / x), (int)(m_buffer.read.height / y), 1);
        }

        m_shader.SetFloat("dissipation", Time.deltaTime * m_settings.mask.dissipation);
        m_settings.mask.material.SetTexture("_MainTex", m_buffer.write);

        m_buffer.Swap();
    }
}