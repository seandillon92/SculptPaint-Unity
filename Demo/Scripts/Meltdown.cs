using System.Collections;
using UnityEngine;
using PaintSculpt;

public class Meltdown : MonoBehaviour
{
    [SerializeField]
    private PaintSettings m_paintSettings;
    [SerializeField]
    private SculptSettings m_sculptSettings;
    
    [SerializeField]
    private SculptSettings m_secondarySculptSettings;

    [SerializeField]
    private BrushSettings m_brushSettings;
    [SerializeField]
    private ControlSettings m_controlSettings;

    private Paint m_paint;
    private Sculpt m_sculpt;
    private Sculpt m_sculpt_secondary;
    private Control m_control;

    [SerializeField]
    private LayerMask m_layerMask;

    [SerializeField]
    private MaskedObject m_object;

    private MeshRenderer m_renderer;
    private MeshCollider m_collider;

    private void Awake()
    {
        m_collider = GetComponent<MeshCollider>();
    }
    private void Start()
    {
        m_renderer = GetComponent<MeshRenderer>();
        m_paint = new Paint(m_paintSettings, m_brushSettings, m_renderer );

        m_sculpt = new Sculpt(m_sculptSettings, m_brushSettings);
        m_sculpt_secondary = new Sculpt(m_secondarySculptSettings, m_brushSettings);
        m_control = new Control(m_controlSettings, m_brushSettings);

        m_collider.sharedMesh = m_sculpt.Mesh;
    }

    private void Update()
    {
        m_control.Update();
        var lmb = Input.GetMouseButton(0);
        if (lmb)
        {
            var cam = Camera.main;
            var ray = cam.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out var hit, Mathf.Infinity, m_layerMask))
            {
                var point =  hit.transform.InverseTransformPoint(hit.point);
                var normal = hit.transform.InverseTransformDirection(hit.normal);
                var forward = hit.transform.InverseTransformDirection(Vector3.up);
                m_paint.Write(point, normal, forward, transform.lossyScale);
                
                StartCoroutine(MeltDown(point, normal, forward, hit.transform.lossyScale));
            }
        }
        m_paint.Update();
        m_object.UpdateMask(m_paint.Texture, 0);
    }

    private IEnumerator MeltDown(Vector3 position, Vector3 normal, Vector3 forward, Vector3 scale)
    {
        yield return MeltGeometry(position, normal, forward, scale);
    }

    private IEnumerator MeltGeometry(Vector3 position, Vector3 normal, Vector3 forward, Vector3 scale)
    {
        var timer = 0.0f;
        var maxTime = m_paintSettings.delay;
        var brushSize = m_brushSettings.size;
        var aspect = m_brushSettings.texture.width / ((float)m_brushSettings.texture.height);
        while (timer < maxTime)
        {
            m_sculpt.Update(
                position,
                normal,
                forward,
                scale,
                aspect,
                brushSize,
                deformation: 0.0001f * Time.deltaTime);

            m_sculpt_secondary.Update(
                position,
                normal,
                forward,
                scale,
                aspect,
                brushSize,
                deformation: 0.0001f * Time.deltaTime);

            yield return null;

            timer += Time.deltaTime;
        }
    }
}
