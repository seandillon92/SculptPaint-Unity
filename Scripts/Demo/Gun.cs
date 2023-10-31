using UnityEngine;

public class Gun : MonoBehaviour
{
    [SerializeField] private Transform start;
    [SerializeField] private Transform end;
    [SerializeField] private Transform pivot;


    public void Activate(bool active)
    {
        pivot.gameObject.SetActive(active);
    }
}
