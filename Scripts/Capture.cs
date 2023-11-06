using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
internal class Capture
{
    [SerializeField]
    internal RenderTexture texture;

    private List<Material> m_materials;
    private Camera m_camera;
    private LayerMask m_cull;

    private List<MeshRenderer> m_renderers = new List<MeshRenderer>();
    private List<List<Material>> m_cachedMaterials = new List<List<Material>>();

    internal Capture(
        Camera cam, 
        LayerMask cull, 
        List<MeshRenderer> renderers, 
        List<Material> materials)
    {
        m_camera = cam;
        m_cull = cull;
        m_materials = materials;

        texture =
            new RenderTexture(
                m_camera.pixelWidth,
                m_camera.pixelHeight,
                1,
                RenderTextureFormat.ARGBHalf);

        texture.enableRandomWrite = true;

        if (!texture.Create())
        {
            throw new Exception("Could not create texture");
        }

        m_renderers.AddRange(renderers);

        for (int i = 0; i < renderers.Count; i++)
        {
            var cachedMaterials = new List<Material>();
            renderers[i].GetSharedMaterials(cachedMaterials);

            m_cachedMaterials.Add(cachedMaterials);
        }
    }

    internal RenderTexture Update()
    {
        Update(this.texture);
        return this.texture;
    }

    internal void Update(RenderTexture target)
    {
        var prevCull = m_camera.cullingMask;
        var prevClearFlag = m_camera.clearFlags;
        var prevBackgroundColor = m_camera.backgroundColor;
        var prevTarget = m_camera.targetTexture;
        var prevResolution = Screen.currentResolution;
        var prevFullScreen = Screen.fullScreen;

        m_camera.cullingMask = m_cull;
        m_camera.clearFlags = CameraClearFlags.SolidColor;

        m_camera.backgroundColor = Color.black;
        m_camera.targetTexture = target;


        if (m_materials != null)
        {
            for (int i = 0; i < m_renderers.Count; i++)
            {
                m_renderers[i].SetSharedMaterials(m_materials);
            }
        }

        Screen.SetResolution(texture.width, texture.height, true);
        m_camera.Render();
        Screen.SetResolution(prevResolution.width, prevResolution.height, prevFullScreen);

        if (m_materials != null)
        {
            for (int i = 0; i < m_renderers.Count; i++)
            {
                m_renderers[i].SetSharedMaterials(m_cachedMaterials[i]);
            }
        }

        m_camera.cullingMask = prevCull;
        m_camera.clearFlags = prevClearFlag;
        m_camera.backgroundColor = prevBackgroundColor;
        m_camera.targetTexture = prevTarget;
    }
}