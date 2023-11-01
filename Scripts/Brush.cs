using UnityEngine;

internal class Brush
{
    private Settings m_settings;

    internal Brush(Settings settings)
    {
        m_settings = settings;
        m_settings.brush.material.SetTexture("_MainTex", m_settings.brush.texture);
    }

    internal void Update()
    {
        UpdateBrush();
    }

    private void UpdateBrush()
    {
        var input = m_settings.paint.camera.ScreenToViewportPoint(Input.mousePosition);
        var position = new Vector2(input.x, input.y);
        m_settings.brush.material.SetVector("mousePos", position);

        var aspect = m_settings.paint.camera.aspect;
        m_settings.brush.material.SetFloat("aspect", aspect);
        var brush = m_settings.brush;
        var control = m_settings.control;

        brush.size += Input.mouseScrollDelta.y * control.sizeChangeSpeed;
        brush.size = Mathf.Max(brush.minSize, brush.size);
        m_settings.brush.material.SetFloat("size", 1f / brush.size);
    }
}