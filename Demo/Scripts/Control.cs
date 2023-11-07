using UnityEngine;

internal class Control
{
    private Vector3? m_lastDistanceMousePos;
    private Vector3? m_lastRotationMousePos;
    private Settings m_settings;
    private Transform m_transform;

    internal Control(Settings settings)
    {
        m_settings = settings;
        m_transform = settings.control.transform;
    }

    internal void Update()
    {
        UpdateRotation();
        UpdateDistance();
    }

    private void UpdateRotation()
    {
        if (Input.GetMouseButton(1))
        {
            var current = Input.mousePosition;
            if (m_lastRotationMousePos != null)
            {
                var velocity = current - m_lastRotationMousePos;
                var rotateVelocity = m_settings.control.rotateVelocity;
                m_transform.rotation =
                    Quaternion.AngleAxis(-velocity.Value.x * rotateVelocity, Vector3.up) *
                    Quaternion.AngleAxis(velocity.Value.y * rotateVelocity, Vector3.right) *
                     m_transform.rotation;
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
        m_transform.position =
            m_settings.paint.camera.transform.position + m_settings.paint.camera.transform.forward * distance;
    }
}
