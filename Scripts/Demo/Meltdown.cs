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
    private MeshCollider m_collider;

    private void Awake()
    {
        m_collider = GetComponent<MeshCollider>();
    }
    private void Start()
    {
        m_renderer = GetComponent<MeshRenderer>();
        m_paint = new Paint(m_settings, m_renderer );

        m_brush = new Brush(m_settings);
        m_sculpt = new Sculpt(m_settings);
        m_control = new Control(m_settings);

        m_collider.sharedMesh = m_sculpt.mesh;
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
                var point =  hit.transform.InverseTransformPoint(hit.point);
                var normal = hit.transform.InverseTransformDirection(hit.normal);
                //RenderDocCapture.RunWithCapture(() =>
                //{
                    m_paint.Write(point, normal, transform.lossyScale);
                //}, 1);
                StartCoroutine(MeltDown(point, normal, hit.transform.lossyScale));
            }
        }
        m_paint.Update();
        m_maskMaterial.SetTexture("_MainTex", m_paint.Texture);
    }

    private IEnumerator MeltDown(Vector3 position, Vector3 normal, Vector3 scale)
    {
        yield return MeltGeometry(position, normal, scale);
    }

    private IEnumerator MeltGeometry(Vector3 position, Vector3 normal, Vector3 scale)
    {
        var timer = 0.0f;
        var maxTime = m_settings.paint.delay;
        var brushSize = m_settings.brush.size;
        var aspect = m_settings.brush.texture.width / m_settings.brush.texture.height;
        while (timer < maxTime)
        {
            m_sculpt.Update(
                position,
                normal,
                scale,
                aspect,
                brushSize,
                deformation: 0.0001f * Time.deltaTime);

            yield return null;

            timer += Time.deltaTime;
        }
    }
}
