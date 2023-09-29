using UnityEngine;

internal class TextureBlender
{
    private ComputeShader m_shader;
    private int m_kernel;
    private int m_t;
    private int m_reada;
    private int m_readb;
    private int m_write;
    internal TextureBlender()
    {
        m_shader = Resources.Load<ComputeShader>("Blend");
        m_kernel = m_shader.FindKernel("Blend");
        m_t = Shader.PropertyToID("Read3");
        m_reada = Shader.PropertyToID("Read1");
        m_readb = Shader.PropertyToID("Read2");
        m_write = Shader.PropertyToID("Write");
    }

    internal void Blend(Texture a, Texture b, RenderTexture write, Texture t)
    {
        var width = Mathf.Max(a.width, b.width);
        var height = Mathf.Max(a.height, b.height);

        m_shader.GetKernelThreadGroupSizes(m_kernel, out uint x, out uint y, out _);
        m_shader.SetTexture(m_kernel, m_t, t);
        m_shader.SetTexture(m_kernel, m_reada, a);
        m_shader.SetTexture(m_kernel, m_readb, b);
        m_shader.SetTexture(m_kernel, m_write, write);
        m_shader.Dispatch(m_kernel, (int)(width / x), (int)(height / y), 1);
    }
}
