using UnityEngine;

[RequireComponent(typeof (Rigidbody))]
public class ItemsToClean : MonoBehaviour, IPhysics, IDirtyObject
{
    [Header("Cleaning")]
    [SerializeField] protected int dirt;
    [SerializeField] protected bool isHidden;

    private Rigidbody _rb;

    public Rigidbody Rigidbody => _rb;

    public int GetDirtValue()
    {
        return dirt;
    }

    public void Hide()
    {
        isHidden = true;
    }

    public void UnHide()
    {
        isHidden = false;
    }

    public bool GetHidden()
    {
        return isHidden;
    }

    void Start()
    {
        _rb = GetComponent<Rigidbody>();
    }
}
