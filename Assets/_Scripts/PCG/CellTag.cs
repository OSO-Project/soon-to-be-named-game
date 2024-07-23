using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CellTag
{
    public enum Zone
    {
        Inner,
        Outer,
        Forbidden
    }

    public Zone zoneName;
}
