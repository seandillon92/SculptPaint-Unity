using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

internal class Paint
{
    private ComputeShader m_shader;
    private int m_dissipate;
    private int m_stamp;

    private Settings m_settings;
    private PingPongBuffer m_buffer;

    private Capture m_capture;
    private RenderTexture m_captureTexture;

    internal Paint(Settings settings, MeshRenderer renderer)
    {
        m_shader = Resources.Load<ComputeShader>("Paint");
        m_dissipate = m_shader.FindKernel("Dissipate");
        m_stamp = m_shader.FindKernel("Stamp");

        m_settings = settings;

        var resolution = m_settings.paint.GetResolution();
        var read = new RenderTexture(resolution, resolution, 1, RenderTextureFormat.RGFloat);
        read.enableRandomWrite = true;

        var write = new RenderTexture(resolution, resolution, 1, RenderTextureFormat.RGFloat);
        write.enableRandomWrite = true;

        m_captureTexture = new RenderTexture(resolution, resolution, 1, RenderTextureFormat.RGFloat);
        m_captureTexture.enableRandomWrite = true;

        if (!read.Create())
        {
            throw new Exception("Could not create texture");
        }
        if (!write.Create())
        {
            throw new Exception("Could not create texture");
        }
        if (!m_captureTexture.Create())
        {
            throw new Exception("Could not create texture");
        }

        m_buffer = new PingPongBuffer(read, write);

        m_capture = 
            new Capture(
                m_settings.paint.camera, 
                m_settings.paint.layer, 
                new List<MeshRenderer>() { renderer}, 
                new List<Material>() { m_settings.paint.material });

        m_settings.paint.material.SetTexture("_MainTex", m_settings.brush.texture);
    }

    internal RenderTexture Texture => m_buffer.write;

    internal void Write(Vector3 position, Vector3 normal, Vector3 forward, Vector3 scale)
    {
        var material = m_settings.paint.material;
        material.SetVector("position", position);
        material.SetVector("normal", normal.normalized);
        material.SetVector("forward", forward.normalized);
        material.SetFloat("rotation", m_settings.brush.rotation);

        if (normal == forward)
        {
            throw new Exception("Forward vector is same as normal");
        }
        var tangent = Vector3.ProjectOnPlane(forward, normal);
        tangent = Quaternion.Euler(normal * m_settings.brush.rotation) * tangent;

        var bitangent = Vector3.Cross(normal, tangent);
        material.SetVector("tangent", tangent.normalized);
        material.SetVector("bitangent", bitangent.normalized);

        material.SetInt("space", (int)m_settings.brush.projection);

        m_settings.paint.material.SetFloat("radius", 1.0f / m_settings.brush.size);
        m_settings.paint.material.SetVector("scale", scale);
        m_settings.paint.material.SetFloat("aspect", m_settings.brush.texture.width / ((float)m_settings.brush.texture.height));

        m_capture.Update(m_captureTexture);

        m_shader.SetTexture(m_stamp, "Read", m_captureTexture);
        m_shader.SetTexture(m_stamp, "Write", m_buffer.write);
        m_shader.SetFloat("delay", m_settings.paint.delay * m_settings.paint.dissipation);

        m_shader.GetKernelThreadGroupSizes(m_stamp, out uint x, out uint y, out _);
        m_shader.Dispatch(m_stamp, (int)(m_buffer.read.width / x), (int)(m_buffer.read.height / y), 1);
    }

    internal void Update()
    {
        m_buffer.Swap();
        m_shader.SetTexture(m_dissipate, "Write", m_buffer.write);
        m_shader.SetTexture(m_dissipate, "Read", m_buffer.read);

        m_shader.SetFloat("dissipation", Time.deltaTime * m_settings.paint.dissipation);

        m_shader.GetKernelThreadGroupSizes(m_dissipate, out uint x, out uint y, out _);
        m_shader.Dispatch(m_dissipate, (int)(m_buffer.read.width / x), (int)(m_buffer.read.height / y), 1);
    }
}