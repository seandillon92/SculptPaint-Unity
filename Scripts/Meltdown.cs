using System.Collections;
using UnityEngine;

public class Meltdown: MonoBehaviour
{
    [SerializeField]
    private Settings m_settings;

    private Mask m_mask;
    private Brush m_brush;
    private Sculpt m_sculpt;

    private Vector3? m_lastDistanceMousePos;
    private Vector3? m_lastRotationMousePos;

    private void Start()
    {
        m_mask = new Mask(m_settings);
        m_brush = new Brush(m_settings);
        m_sculpt = new Sculpt(m_settings);

        m_settings.mask.capture.material = m_settings.brush.material;
    }

    private void Update()
    {
        UpdateRotation();
        UpdateDistance();

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
                deformation:0.0001f * Time.deltaTime, 
                direction: Vector3.down,
                space: Space.Self);

            yield return null;

            timer += Time.deltaTime;
        }
    }

    private void UpdateRotation()
    {
        if (Input.GetMouseButton(1))
        {
            var current = Input.mousePosition;
            if (m_lastRotationMousePos != null) {
                var velocity = current - m_lastRotationMousePos;
                var rotateVelocity = m_settings.control.rotateVelocity;
                transform.rotation =
                    Quaternion.AngleAxis(-velocity.Value.x * rotateVelocity, Vector3.up) *
                    Quaternion.AngleAxis(velocity.Value.y * rotateVelocity, Vector3.right) *
                     transform.rotation;
            }
            m_lastRotationMousePos = current;
        }
        else
        {
            m_lastRotationMousePos = null;
        }

    }

    private void UpdateDistance()
    {
        if (Input.GetMouseButton(2))
        {
            var mousePos = Input.mousePosition;
            if (m_lastDistanceMousePos != null)
            {
                var delta = mousePos - m_lastDistanceMousePos;
                var control = m_settings.control;
                control.distance += delta.Value.y * control.distanceChangeSpeed;
                control.distance = Mathf.Clamp01(control.distance);
            }
            m_lastDistanceMousePos = mousePos;
        }
        else
        {
            m_lastDistanceMousePos = null;
        }

        var t = m_settings.control.distance;
        var distance = Mathf.Lerp(m_settings.control.distanceMin, m_settings.control.distanceMax, t);
        transform.position = 
            m_settings.mask.camera.transform.position + m_settings.mask.camera.transform.forward * distance;
    }
}
