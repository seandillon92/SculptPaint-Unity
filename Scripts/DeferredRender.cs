using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

[Serializable]
internal class MaterialSet
{
    [SerializeField]
    internal List<Material> materials;

    [SerializeField]
    internal int maskChannel;
}

internal class RenderSet
{
    internal Capture capture;
    internal MaterialSet materials;

    public RenderSet(Capture capture, MaterialSet material)
    {
        this.capture = capture;
        this.materials = material;
    }
}

[RequireComponent(typeof(Camera))]
internal class DeferredRender : MonoBehaviour
{
    private List<RenderSet> m_renders = new List<RenderSet>();

    [SerializeField]
    private Material m_blend;

    [SerializeField]
    private List<MaterialSet> m_materials;

    [SerializeField]
    private Camera m_camera;

    [SerializeField]
    private LayerMask m_cull;

    private List<MaskedObject> m_objects;

    [SerializeField]
    private Capture m_mask;

    public void Awake()
    {
        m_camera = GetComponent<Camera>();
    }

    public void Start()
    {
        var objects = FindObjectsOfType<MaskedObject>();
        m_objects = new List<MaskedObject>();
        var renderers = new List<MeshRenderer>();
        for (int i = 0; i < objects.Length; i++)
        {
            var obj = objects[i];

            if ((1 << obj.gameObject.layer & m_cull) != 0)
            {
                m_objects.Add(obj);
                renderers.Add(obj.Renderer);
            }
        }

        m_renders.Clear();

        for (int i = 0; i < m_materials.Count; i++)
        {
            var materials = m_materials[i];
            m_renders.Add(
                new RenderSet(
                    new Capture(m_camera, m_cull, renderers, materials.materials),
                    materials));
        }

        m_mask = new Capture(m_camera,cull: ~0, renderers, null);
    }

    private bool blend = false;

    public void LateUpdate()
    {
        blend = false;

        // Grab snaps of all objects, using the override materials
        for (int i = 0; i < m_materials.Count; i++)
        {
            m_renders[i].capture.Update();
        }

        // change the objects to use the mask material
        for (int i = 0; i < m_objects.Count; i++)
        {
            var obj = m_objects[i];
            obj.ApplyMaterials(obj.Masks);
        }

        m_mask.Update();

        for(int i =0; i < m_objects.Count; i++)
        {
            var obj = m_objects[i];
            obj.ApplyMaterials(obj.Materials);
        }

        blend = true;
    }

    public void OnPostRender()
    {
        if (blend)
        {
            for (int i = 0; i < m_renders.Count; i++)
            {
                var render = m_renders[i];
                SetForeground(render.capture.texture);
                SetMask( m_mask.texture, render.materials.maskChannel);
                Blend(BlendMode.SrcAlpha, BlendMode.OneMinusSrcAlpha);
            }
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
