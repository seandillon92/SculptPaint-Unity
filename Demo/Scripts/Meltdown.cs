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
    private BrushSettings m_brushSettings;
    [SerializeField]
    private ControlSettings m_controlSettings;

    private Paint m_paint;
    private Sculpt m_sculpt;
    private Control m_control;

    [SerializeField]
    private LayerMask m_layerMask;

    [SerializeField]
    private MaskedObject m_object;

    private MeshRenderer m_renderer;
    private MeshCollider m_collider;
    private MeshFilter m_filter;

    private void Awake()
    {
        m_renderer = GetComponent<MeshRenderer>();
        m_collider = GetComponent<MeshCollider>();
        m_filter = GetComponent<MeshFilter>();
    }
    private void Start()
    {

        m_paint = new Paint(m_paintSettings, m_brushSettings, m_renderer );

        m_sculpt = new Sculpt(m_sculptSettings, m_brushSettings, m_filter);
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
                m_paint.Write(point, normal, forward);
                
                StartCoroutine(MeltGeometry(point, normal, forward));
            }
        }
        m_paint.Update();
        m_object.UpdateMask(m_paint.Texture, 0);
    }


    private IEnumerator MeltGeometry(Vector3 position, Vector3 normal, Vector3 forward)
    {
        var timer = 0.0f;
        var maxTime = m_paintSettings.delay;
        while (timer < maxTime)
        {
            m_sculpt.Update(
                position,
                normal,
                forward);

            yield return null;

            timer += Time.deltaTime;
        }
    }
}
