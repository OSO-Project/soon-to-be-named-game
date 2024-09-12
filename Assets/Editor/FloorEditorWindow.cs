using UnityEngine;
using UnityEditor;

public class FloorEditorWindow : EditorWindow
{
    private Floor sourceFloor;

    [MenuItem("Tools/Floor Editor")]
    public static void ShowWindow()
    {
        GetWindow<FloorEditorWindow>("Floor Editor");
    }

    private void OnGUI()
    {
        GUILayout.Label("Copy Floor Mesh and Materials", EditorStyles.boldLabel);

        // Display a field to assign a source Floor object
        sourceFloor = (Floor)EditorGUILayout.ObjectField("Source Floor", sourceFloor, typeof(Floor), true);

        if (GUILayout.Button("Apply to All Selected"))
        {
            if (sourceFloor == null)
            {
                EditorUtility.DisplayDialog("Error", "Please select a source Floor.", "OK");
                return;
            }

            ApplyToAllSelectedObjects();
        }
    }

    private void ApplyToAllSelectedObjects()
    {
        // Get all selected objects in the scene
        foreach (GameObject obj in Selection.gameObjects)
        {
            // Check if the object has a Floor component
            Floor targetFloor = obj.GetComponent<Floor>();
            if (targetFloor != null)
            {
                ApplyMeshAndMaterials(sourceFloor, targetFloor);
            }
            else
            {
                Debug.LogWarning($"Object {obj.name} does not have a Floor component. Skipping.");
            }
        }
    }

    private void ApplyMeshAndMaterials(Floor sourceFloor, Floor targetFloor)
    {
        // Set the same FloorType, PrimaryColor, SecondaryColor, and GroutColor
        SerializedObject serializedTarget = new SerializedObject(targetFloor);

        serializedTarget.FindProperty("_floorType").enumValueIndex = (int)sourceFloor.FloorType;
        serializedTarget.FindProperty("_floorPrimaryColor").enumValueIndex = (int)sourceFloor.FloorPrimaryColor;
        serializedTarget.FindProperty("_floorSecondaryColor").enumValueIndex = (int)sourceFloor.FloorSecondaryColor;
        serializedTarget.FindProperty("_floorGroutColor").enumValueIndex = (int)sourceFloor.FloorGroutColor;

        serializedTarget.ApplyModifiedProperties();

        // Call the UpdateMaterialsAndMesh function to update the Mesh and Materials
        targetFloor.UpdateMaterialsAndMesh();

        Debug.Log($"Applied floor settings from {sourceFloor.name} to {targetFloor.name}");
    }
}