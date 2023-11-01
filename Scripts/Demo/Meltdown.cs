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
                //Debug.LogFormat("Hit: {0}", hit.point);
                //m_paint.Write(hit.point);
                //RenderDocCapture.RunWithCapture(() => { 
                var texture = m_paint.Write(hit.point);
                m_renderer.sharedMaterial.SetTexture("_MainTex", texture);

                //}, 1);

            }
            //StartCoroutine(MeltDown());
        }

        m_paint.Update();
    }

    private IEnumerator MeltDown()
    {
        yield return MeltGeometry();
    }

    private IEnumerator MeltGeometry()
    {
        var timer = 0.0f;
        var maxTime = m_settings.paint.delay;
        var position = Input.mousePosition;

        var model = m_settings.sculpt.mesh.transform.localToWorldMatrix;
        var view = m_settings.sculpt.camera.worldToCameraMatrix;
        var projection = m_settings.sculpt.camera.projectionMatrix;
        var mvp = projection * view * model;

        while (timer < maxTime)
        {
            m_sculpt.Update(
                mvp: mvp,
                position,
                deformation: 0.0001f * Time.deltaTime);

            yield return null;

            timer += Time.deltaTime;
        }
    }
}
