using UnityEngine;

internal class Brush
{
    private Settings m_settings;

    internal Brush(Settings settings)
    {
        m_settings = settings;
    }

    internal void Update()
    {
        UpdateBrush();
    }

    private void UpdateBrush()
    {
        var brush = m_settings.brush;
        var control = m_settings.control;

        brush.size += Input.mouseScrollDelta.y * control.sizeChangeSpeed;
        brush.size = Mathf.Max(brush.minSize, brush.size);
    }
}