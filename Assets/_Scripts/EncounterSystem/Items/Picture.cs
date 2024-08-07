using UnityEngine;

public class Picture : MonoBehaviour, IPhysics
{
    private Rigidbody _rb;

    public Rigidbody Rigidbody => _rb;

    void Awake()
    {
        _rb = GetComponent<Rigidbody>();
    }
}
