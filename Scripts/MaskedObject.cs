using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class MaskedObject : MonoBehaviour
{
    internal MeshRenderer Renderer { get; private set; }
    internal List<Material> Materials { get; private set; } = new List<Material>();
    internal List<Material> Masks { get; private set; } = new List<Material>();

    [SerializeField]
    private Texture debug;

    private void Awake()
    {
        Renderer = GetComponent<MeshRenderer>();
    }

    internal void ApplyMaterials(List<Material> materials)
    {
        Renderer.SetSharedMaterials(materials);
    }

    public void UpdateMask(Texture tex, int index)
    {
        Masks[index].SetTexture("_MainTex", tex);
        debug = tex;
    }

    private void Start()
    {
        Renderer.GetMaterials(Materials);
        for (int i  = 0; i < Materials.Count; i++)
        {
            Masks.Add(new Material(Shader.Find("Unlit/Texture")));
        }
    }
}
