using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cell : MonoBehaviour
{
    public Vector3 position;
    public CellTag.Zone zone;
    public CellSideTag.Side side;
    public bool isOccupied;
    public bool hasDecoration;

    public void MakeForbidden()
    {
        zone = CellTag.Zone.Forbidden;
    }
}
