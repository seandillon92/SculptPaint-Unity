using System.Collections;
using UnityEngine;

public class Meltdown: MonoBehaviour
{
    [SerializeField]
    private Settings m_settings;

    private Mask m_mask;
    private Brush m_brush;
    private Sculpt m_sculpt;
    private Control m_control;

    private void Start()
    {
        m_mask = new Mask(m_settings);
        m_brush = new Brush(m_settings);
        m_sculpt = new Sculpt(m_settings);
        m_control = new Control(m_settings);

        m_settings.mask.capture.material = m_settings.brush.material;
    }

    private void Update()
    {
        m_control.Update();
        m_brush.Update();
        var lmb = Input.GetMouseButton(0);
        if (lmb)
        {
            StartCoroutine(MeltDown());
        }

        m_mask.Update();

    }

    private IEnumerator MeltDown()
    {
        var capture = m_settings.mask.capture;
        capture.Update();
        m_mask.Write(capture.texture);
        yield return MeltGeometry();
    }

    private IEnumerator MeltGeometry()
    {
        var timer = 0.0f;
        var maxTime = m_settings.mask.delay;
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
                deformation:0.0001f * Time.deltaTime);

            yield return null;

            timer += Time.deltaTime;
        }
    }
}
