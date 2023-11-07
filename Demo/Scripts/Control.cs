using PaintSculpt;
using System;
using UnityEngine;


[Serializable]
internal class ControlSettings
{
    [SerializeField]
    internal float rotateVelocity = 0.1f;

    [SerializeField]
    internal float sizeChangeSpeed = 0.1f;

    [SerializeField]
    internal float distanceMin = 1;

    [SerializeField]
    internal float distanceMax = 10;

    [SerializeField]
    [Range(0, 1)]
    internal float distance = 0.5f;

    [SerializeField]
    internal float distanceChangeSpeed = 1.0f;

    [SerializeField]
    internal Transform transform;

    [SerializeField]
    internal Transform cameraTransform;
}

internal class Control
{
    private Vector3? m_lastDistanceMousePos;
    private Vector3? m_lastRotationMousePos;
    private ControlSettings m_settings;
    private Transform m_transform;
    private BrushSettings m_brushSettings;

    internal Control(ControlSettings settings, BrushSettings brushSettings)
    {
        m_settings = settings;
        m_transform = settings.transform;
        m_brushSettings = brushSettings;
    }

    internal void Update()
    {
        UpdateRotation();
        UpdateDistance();
        UpdateBrushSize();
    }

    private void UpdateRotation()
    {
        if (Input.GetMouseButton(1))
        {
            var current = Input.mousePosition;
            if (m_lastRotationMousePos != null)
            {
                var velocity = current - m_lastRotationMousePos;
                var rotateVelocity = m_settings.rotateVelocity;
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
                var control = m_settings;
                control.distance += delta.Value.y * control.distanceChangeSpeed;
                control.distance = Mathf.Clamp01(control.distance);
            }
            m_lastDistanceMousePos = mousePos;
        }
        else
        {
            m_lastDistanceMousePos = null;
        }

        var t = m_settings.distance;
        var distance = Mathf.Lerp(m_settings.distanceMin, m_settings.distanceMax, t);
        m_transform.position =
            m_settings.cameraTransform.position + m_settings.cameraTransform.forward * distance;
    }

    private void UpdateBrushSize()
    {
        m_brushSettings.size += m_settings.sizeChangeSpeed * Time.deltaTime * Input.mouseScrollDelta.y;
        m_brushSettings.size = Mathf.Max(m_brushSettings.size, m_brushSettings.minSize);
    }
}
