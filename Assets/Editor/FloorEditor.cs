using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(Floor))]
public class FloorEditor : Editor
{
    public override void OnInspectorGUI()
    {
        Floor floor = (Floor)target;

        // Automatically update materials and mesh on every GUI redraw.
        floor.UpdateMaterialsAndMesh();

        // Draw the default inspector fields.
        DrawDefaultInspector();
    }
}