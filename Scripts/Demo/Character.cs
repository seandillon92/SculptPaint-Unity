using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Character : MonoBehaviour
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
    private Settings m_settings;

    [SerializeField]
    private MeshRenderer m_renderer;

    private Paint m_paint;

    [SerializeField]
    private MaskedObject m_mask;

    private void Start()
    {
        m_paint = new Paint(m_settings, m_renderer);
    }

    void Update()
    {
        var forward = Input.GetKey(KeyCode.W);
        animator.SetBool("forward", forward);

        if (forward)
        {
            character.position += character.forward * speed;
        }

        m_paint.Update();
        m_mask.UpdateMask(m_paint.Texture, index:0);
    }

    public void FootMoved(string foot)
    {
        Vector3 frontFoot = Vector3.zero;
        Vector3 backFoot = Vector3.zero;

        if (foot == "left")
        {
            // Paint on front left and back right
            frontFoot = forward_left.position;
            backFoot = back_right.position;
        }
        else if (foot == "right")
        {
            // Paint on front right and back left
            frontFoot = forward_right.position;
            backFoot = back_left.position;
        }

        var localFront = m_renderer.transform.InverseTransformPoint(frontFoot);
        var localBack = m_renderer.transform.InverseTransformPoint(backFoot);

        m_paint.Write(localFront, Vector3.up, Vector3.one);
        m_paint.Write(localBack, Vector3.up, Vector3.one);


        m_mask.UpdateMask(m_paint.Texture, index:0);
    }
}
