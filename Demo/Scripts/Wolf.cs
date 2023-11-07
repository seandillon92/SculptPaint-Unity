using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PaintSculpt;

public class Wolf : MonoBehaviour
{
    [SerializeField]
    private Animator animator;

    [SerializeField]
    private Transform character;

    [SerializeField]
    private float speed = 1;

    [SerializeField]
    private Transform forward_left;

    [SerializeField] 
    private Transform forward_right;

    [SerializeField]
    private Transform back_left;

    [SerializeField]
    private Transform back_right;

    [SerializeField]
    private MeshRenderer m_renderer;

    private Paint m_paint;

    [SerializeField]
    private MaskedObject m_mask;

    [SerializeField]
    private PaintSettings m_paintSettings;

    [SerializeField]
    private BrushSettings m_brushSettings;

    private void Start()
    {
        m_paint = new Paint(m_paintSettings, m_brushSettings, m_renderer);
    }

    void Update()
    {
        var forward = Input.GetKey(KeyCode.W);
        var backward = Input.GetKey(KeyCode.S);
        
        animator.SetBool("forward", forward);
        animator.SetBool("backward", backward);

        if (forward)
        {
            character.position += character.forward * speed;
        }
        if (backward)
        {
            character.position += -character.forward * speed;
        }

        m_paint.Update();
        m_mask.UpdateMask(m_paint.Texture, index:0);
    }

    public void FootMoved(string foot)
    {
        Transform frontFoot = null;
        Transform backFoot = null;

        if (foot == "left")
        {
            // Paint on front left and back right
            frontFoot = forward_left;
            backFoot = back_right;
        }
        else if (foot == "right")
        {
            // Paint on front right and back left
            frontFoot = forward_right;
            backFoot = back_left;
        }

        var localFront = m_renderer.transform.InverseTransformPoint(frontFoot.position);
        var localBack = m_renderer.transform.InverseTransformPoint(backFoot.position);

        m_paint.Write(localFront, Vector3.up, frontFoot.forward, Vector3.one);
        m_paint.Write(localBack, Vector3.up, backFoot.forward, Vector3.one);

        m_mask.UpdateMask(m_paint.Texture, index:0);
    }
}
