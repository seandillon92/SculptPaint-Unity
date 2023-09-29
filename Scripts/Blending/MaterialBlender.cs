using System;
using UnityEngine;
using UnityEngine.Experimental.Rendering;

internal class MaterialBlender : IDisposable
{
    private delegate object GetProperty(Material mat, string id);
    private delegate void SetProperty(Material mat, string id, object ob);
    private delegate object Lerp(object a, object b, float t, string id);

    private Material m_material1;
    private Material m_material2;
    private Material m_blend;
    private TextureBlender m_textureBlender;
    private RenderTexturePool m_texturePool;
    private RenderTexturePool m_defaultTexturesPool;

    internal MaterialBlender(Material material1, Material material2, Material blend)
    {
        m_material1 = material1;
        m_material2 = material2;
        m_textureBlender = new TextureBlender();
        m_texturePool = new RenderTexturePool(2048);
        m_defaultTexturesPool = new RenderTexturePool(2048);
        m_blend = blend;
    }

    internal void Update(Texture mask, float t)
    {
        PassProperties(
            m_material1.GetPropertyNames(MaterialPropertyType.Float),
            (Material mat, string id) => mat.GetFloat(id),
            (Material mat, string id, object ob) => mat.SetFloat(id, (float)ob),
            (object a, object b, float t, string id) => Mathf.Lerp((float)a, (float)b, t), t);

        PassProperties(
            m_material1.GetPropertyNames(MaterialPropertyType.Int),
            (Material mat, string id) => mat.GetInt(id),
            (Material mat, string id, object ob) => mat.SetFloat(id, (int)ob),
            (object a, object b, float t, string id) => Mathf.Lerp((int)a, (int)b, t), t);

        PassProperties(
            m_material1.GetPropertyNames(MaterialPropertyType.Vector),
            (Material mat, string id) => mat.GetVector(id),
            (Material mat, string id, object ob) => mat.SetVector(id, (Vector4)ob),
            (object a, object b, float t, string id) => Vector4.Lerp((Vector4)a, (Vector4)b, t), t);

        PassProperties(
            m_material1.GetPropertyNames(MaterialPropertyType.Texture),
            (Material mat, string id) => mat.GetTexture(id),
            (Material mat, string id, object ob) => mat.SetTexture(id, ob as Texture),
            (object a, object b, float t, string id) => LerpTexture(a as Texture2D, b as Texture2D, mask, id), t);
    }

    private Texture LerpTexture(Texture a, Texture b, Texture t, string id)
    {

        if (a == null && b == null) return null;

        Texture exemplar = a ? a : b;

        var colors = GraphicsFormatUtility.GetColorComponentCount(exemplar.graphicsFormat);
        var format = colors switch
        {
            1 => RenderTextureFormat.RFloat,
            2 => RenderTextureFormat.RGFloat,
            3 => RenderTextureFormat.ARGBFloat,
            4 => RenderTextureFormat.ARGBFloat,
            _ => throw new Exception("Format not handled"),
        };
        var blend = m_texturePool.Get(id, format);

        if (a == null)
        {
            a = m_defaultTexturesPool.Get(id, format, Color.white);
        }
        else if (b == null)
        {
            b = m_defaultTexturesPool.Get(id, format, Color.white);
        }

        m_textureBlender.Blend(a, b, blend, t);
        return blend;
    }

    private void PassProperties(string[] names, GetProperty get, SetProperty set, Lerp lerp, float t)
    {
        for (int i = 0; i < names.Length; i++)
        {
            var id = names[i];

            var prop1 = get(m_material1, id);
            var prop2 = get(m_material2, id);
            var blend = lerp(prop1, prop2, t, id);
            set(m_blend, id, blend);
        }
    }

    public void Dispose()
    {
        m_texturePool.Dispose();
    }
}
