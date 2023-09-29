using System;
using UnityEngine;

[Serializable]
internal class Capture
{
    [SerializeField]
    internal RenderTexture texture;
    [SerializeField]
    internal Material material;
    [SerializeField]
    internal MeshRenderer renderer;

    private Camera m_camera;
    private LayerMask m_cull;

    internal void Init(Camera cam, LayerMask cull)
    {
        m_camera = cam;
        m_cull = cull;

        texture =
            new RenderTexture(
                m_camera.pixelWidth,
                m_camera.pixelHeight,
                1,
                RenderTextureFormat.ARGBFloat);

        texture.enableRandomWrite = true;

        if (!texture.Create())
        {
            throw new Exception("Could not create texture");
        }
    }

    internal void Update()
    {
        var prevMaterial = renderer.sharedMaterial;
        var prevCull = m_camera.cullingMask;
        var prevClearFlag = m_camera.clearFlags;
        var prevBackgroundColor = m_camera.backgroundColor;
        var prevTarget = m_camera.targetTexture;

        renderer.sharedMaterial = material;

        m_camera.cullingMask = m_cull;
        m_camera.clearFlags = CameraClearFlags.SolidColor;
        m_camera.backgroundColor = Color.black;
        m_camera.targetTexture = texture;

        m_camera.Render();

        renderer.sharedMaterial = prevMaterial;
        m_camera.cullingMask = prevCull;
        m_camera.clearFlags = prevClearFlag;
        m_camera.backgroundColor = prevBackgroundColor;
        m_camera.targetTexture = prevTarget;
    }
}