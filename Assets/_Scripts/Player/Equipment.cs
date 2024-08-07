using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Equipment : MonoBehaviour
{
    [SerializeField] bool _hasWipe = true;
    public bool hasWipe()
    {
        return _hasWipe;
    }
}
