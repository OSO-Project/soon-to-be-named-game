using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Decoration Asset", menuName = "Decoration Asset")]
public class DecorationAsset : ScriptableObject
{
    public GameObject prefab;
    public Vector2 area;
    public CellTag.Zone zone;

    [Range(0, 1)]
    public float chances;

    public bool randomRotation;

    public int minObjNum = -1; // Minimum number of objects required to be spawned (-1 means no restriction)
    public int maxObjNum = -1; // Maximum number of objects allowed to be spawned (-1 means no restriction)

    public bool canSpawnTrash;
    public List<DecorationAsset> trashObjects;
}
