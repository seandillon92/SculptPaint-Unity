using System;
using UnityEngine;
using UnityEngine.Rendering;

[RequireComponent(typeof(Camera))]
internal class DeferredRender : MonoBehaviour
{
    [SerializeField]
    private Capture m_mask;

    [SerializeField]
    private Capture m_brush;

    [SerializeField]
    private Capture m_foreground;

    [SerializeField]
    private Capture m_background;

    [SerializeField]
    private Capture m_brushRender;

    [SerializeField]
    private Material m_blend;

    [SerializeField]
    private Camera m_camera;

    [SerializeField]
    private LayerMask m_cull;

    public void Awake()
    {
        m_camera = GetComponent<Camera>();
    }

    public void Start()
    {
        m_mask.Init(m_camera, m_cull);
        m_foreground.Init(m_camera, m_cull);
        m_brush.Init(m_camera, m_cull);
        m_brushRender.Init(m_camera, m_cull);
        m_background.Init(m_camera, m_cull);
    }

    private bool blend = false;

    public void LateUpdate()
    {
        blend = false;

        m_mask.Update();
        m_foreground.Update();
        m_brush.Update();
        m_brushRender.Update();
        m_background.Update();
        
        blend = true;
    }

    public void OnPostRender()
    {
        if (blend)
        {
            // Capture material 1 render
            SetForeground(m_foreground.texture);
            SetMask(m_mask.texture, 0);
            Blend(BlendMode.SrcAlpha, BlendMode.OneMinusSrcAlpha);

            SetForeground(m_background.texture);
            SetMask(m_mask.texture, 1);
            Blend(BlendMode.SrcAlpha, BlendMode.OneMinusSrcAlpha);

            SetMask(m_brush.texture);
            SetForeground(m_brushRender.texture);
            Blend(BlendMode.SrcAlpha, BlendMode.OneMinusSrcAlpha);
        }
    }

    private void SetMask(RenderTexture mask, int channel = 0)
    {
        m_blend.SetFloat("maskChannel", channel);
        m_blend.SetTexture("_Mask", mask);
    }

    private void SetForeground(RenderTexture foreground)
    {
        m_blend.SetTexture("_MainTex", foreground);
    }

    private void Blend(BlendMode source, BlendMode dest)
    {
        InitializeBlendMaterial(source, dest);
        GL.PushMatrix();
        GL.LoadOrtho();

        GL.Begin(GL.QUADS);
        GL.TexCoord(new Vector3(0.0f, 0.0f, 0.0f));
        GL.Vertex3(0, 0, 0);

        GL.TexCoord(new Vector3(1.0f, 0.0f, 0.0f));
        GL.Vertex3(1, 0, 0);

        GL.TexCoord(new Vector3(1.0f, 1.0f, 0.0f));
        GL.Vertex3(1, 1, 0);

        GL.TexCoord(new Vector3(0.0f, 1.0f, 0.0f));
        GL.Vertex3(0, 1, 0);

        GL.End();

        GL.PopMatrix();
    }

    private void InitializeBlendMaterial(BlendMode source, BlendMode dest)
    {
        m_blend.SetPass(0);
        m_blend.hideFlags = HideFlags.HideAndDontSave;
        // Set blend mode to invert destination colors.
        m_blend.SetInt("_SrcBlend", (int)source);
        m_blend.SetInt("_DstBlend", (int)dest);
        // Turn off backface culling, depth writes, depth test.
        m_blend.SetInt("_Cull", (int)CullMode.Off);
        m_blend.SetInt("_ZWrite", 0);
        m_blend.SetInt("_ZTest", (int)CompareFunction.Always);
    }


}
