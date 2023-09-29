using System;
using System.Collections.Generic;
using UnityEngine;

internal class RenderTexturePool : IDisposable
{
    private Dictionary<string, RenderTexture> m_pool;
    private int m_dimension;

    internal RenderTexturePool(int dimension)
    {
        m_pool = new Dictionary<string, RenderTexture>();
        m_dimension = dimension;
    }

    public void Dispose()
    {
        foreach (var pair in m_pool)
        {
            GameObject.Destroy(pair.Value);
        }
    }

    internal RenderTexture Get(string name, RenderTextureFormat format)
    {
        return Get(name, format, Color.black);
    }

    internal RenderTexture Get(string name, RenderTextureFormat format, Color defaultColor)
    {
        if (m_pool.TryGetValue(name, out RenderTexture rt))
        {
            return rt;
        }
        var text = new RenderTexture(m_dimension, m_dimension, 1, format);
        text.enableRandomWrite = true;

        if (!text.Create())
        {
            throw new Exception("Could not create texture");
        }

        var active = RenderTexture.active;
        RenderTexture.active = text;
        GL.Clear(true, true, defaultColor);
        RenderTexture.active = active;

        m_pool.Add(name, text);
        return text;
    }
}
