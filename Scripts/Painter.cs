using System;
using UnityEngine;

public class Painter : MonoBehaviour
{
    [SerializeField]
    private Settings m_settings;

    private Mask m_mask;
    private Brush m_brush;

    private Vector3? m_lastDistanceMousePos;
    private Vector3? m_lastRotationMousePos;

    private void Start()
    {
        var read = new RenderTexture(2048, 2048, 1, RenderTextureFormat.RGFloat);
        read.enableRandomWrite = true;
        if (!read.Create())
        {
            throw new Exception("Could not create texture");
        }

        var write = new RenderTexture(2048, 2048, 1, RenderTextureFormat.RGFloat);
        write.enableRandomWrite = true;
        if (!write.Create())
        {
            throw new Exception("Could not create texure");
        }

        m_mask = new Mask(m_settings);
        m_brush = new Brush(m_settings);

        m_settings.mask.capture.material = m_settings.brush.material;
    }

    private void Update()
    {
        UpdateRotation();
        UpdateDistance();

        m_brush.Update();
        m_mask.Update();
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
