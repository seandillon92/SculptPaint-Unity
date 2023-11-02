using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Meltdown : MonoBehaviour
{
    [SerializeField]
    private Settings m_settings;

    private Paint m_paint;
    private Brush m_brush;
    private Sculpt m_sculpt;
    private Control m_control;

    [SerializeField]
    private LayerMask m_layerMask;

    [SerializeField]
    private Material m_maskMaterial;

    private MeshRenderer m_renderer;

    private void Start()
    {
        m_renderer = GetComponent<MeshRenderer>();
        m_paint = new Paint(m_settings, new List<MeshRenderer>() { m_renderer });

        m_brush = new Brush(m_settings);
        m_sculpt = new Sculpt(m_settings);
        m_control = new Control(m_settings);
    }

    private void Update()
    {
        m_control.Update();
        m_brush.Update();
        var lmb = Input.GetMouseButton(0);
        if (lmb)
        {
            var cam = Camera.main;
            var ray = cam.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out var hit, Mathf.Infinity, m_layerMask))
            {
                //RenderDocCapture.RunWithCapture(() => { 
                m_paint.Write(hit.point, hit.normal);
                // }, 1);
                StartCoroutine(MeltDown(hit.point, hit.normal));

            }
        }
        m_paint.Update();
        m_maskMaterial.SetTexture("_MainTex", m_paint.Texture);
    }

    private IEnumerator MeltDown(Vector3 position, Vector3 normal)
    {
        yield return MeltGeometry(position, normal);
    }

    private IEnumerator MeltGeometry(Vector3 position, Vector3 normal)
    {
        var timer = 0.0f;
        var maxTime = m_settings.paint.delay;

        var model = m_settings.sculpt.mesh.transform.localToWorldMatrix;
        var brushSize = m_settings.brush.size;
        while (timer < maxTime)
        {
            m_sculpt.Update(
                model,
                position,
                normal,
                brushSize,
                deformation: 0.0001f * Time.deltaTime);

            yield return null;

            timer += Time.deltaTime;
        }
    }
}
