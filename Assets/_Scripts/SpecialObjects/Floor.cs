using UnityEngine;

public class Floor : MonoBehaviour
{
    [SerializeField][Tooltip("Defines a type of the floor.")]
    private FloorType _floorType;
    public FloorType FloorType => _floorType;

    [SerializeField][Tooltip("Defines a primary color of the floor.")]
    private Colors _floorPrimaryColor;
    public Colors FloorPrimaryColor => _floorPrimaryColor;

    [SerializeField][Tooltip("Defines a secondary color of the floor")]
    private Colors _floorSecondaryColor;
    public Colors FloorSecondaryColor => _floorSecondaryColor;

    [SerializeField][Tooltip("Defines a color of the grout.")]
    private Colors _floorGroutColor;
    public Colors FloorGroutColor => _floorGroutColor;

    private Mesh _floorMesh;
    private Material _floorPrimaryMaterial;
    private Material _floorSecondaryMaterial;
    private Material _floorGroutMaterial;

    private void Awake()
    {
        UpdateMaterialsandMesh();
    }

    private void OnValidate()
    {
        UpdateMaterialsandMesh();
    }

    private void UpdateMaterialsandMesh()
    {
        _floorMesh = GetMesh(_floorType);
        AdjustMaterialSlots(_floorType); // Adjust the material slots based on the floor type
        _floorPrimaryMaterial = GetMaterial(_floorPrimaryColor);
        _floorSecondaryMaterial = GetMaterial(_floorSecondaryColor);
        _floorGroutMaterial = GetMaterial(_floorGroutColor);

        ApplyMaterialsAndMesh(_floorPrimaryMaterial, _floorSecondaryMaterial, _floorGroutMaterial, _floorMesh);
    }

    private Material GetMaterial(Colors color)
    {
        switch (color)
        {
            case Colors.A_Blue_20:
                return Resources.Load<Material>("WallMaterials/A_Blue_20");
            case Colors.A_Blue_40:
                return Resources.Load<Material>("WallMaterials/A_Blue_40");
            case Colors.A_Blue_60:
                return Resources.Load<Material>("WallMaterials/A_Blue_60");
            case Colors.A_Blue_80:
                return Resources.Load<Material>("WallMaterials/A_Blue_80");
            case Colors.A_Beige_20:
                return Resources.Load<Material>("WallMaterials/A_Beige_20");
            case Colors.A_Orange_20:
                return Resources.Load<Material>("WallMaterials/A_Orange_20");
            case Colors.A_Orange_40:
                return Resources.Load<Material>("WallMaterials/A_Orange_40");
            case Colors.A_Red_60:
                return Resources.Load<Material>("WallMaterials/A_Red_60");
            case Colors.A_Red_80:
                return Resources.Load<Material>("WallMaterials/A_Red_80");
            case Colors.A_Green_20:
                return Resources.Load<Material>("WallMaterials/A_Green_20");
            case Colors.A_Green_40:
                return Resources.Load<Material>("WallMaterials/A_Green_40");
            case Colors.A_Brown_20:
                return Resources.Load<Material>("WallMaterials/A_Brown_20");
            case Colors.A_Brown_40:
                return Resources.Load<Material>("WallMaterials/A_Brown_40");
            case Colors.A_Brown_60:
                return Resources.Load<Material>("WallMaterials/A_Brown_60");
            case Colors.A_Brown_80:
                return Resources.Load<Material>("WallMaterials/A_Brown_80");
            case Colors.A_Lipstick_20:
                return Resources.Load<Material>("WallMaterials/A_Lipstick_20");
            case Colors.A_Lipstick_40:
                return Resources.Load<Material>("WallMaterials/A_Lipstick_40");
            case Colors.A_Lipstick_60:
                return Resources.Load<Material>("WallMaterials/A_Lipstick_60");
            case Colors.A_Lipstick_80:
                return Resources.Load<Material>("WallMaterials/A_Lipstick_80");
            case Colors.Black:
                return Resources.Load<Material>("WallMaterials/Black");
            case Colors.White:
                return Resources.Load<Material>("WallMaterials/White");
            default:
                return null;
        }
    }

    private Mesh GetMesh(FloorType floorType)
    {
        switch (floorType)
        {
            case FloorType.Plain:
                return Resources.Load<Mesh>("FloorMeshes/Plain");
            case FloorType.Tile_Small:
                return Resources.Load<Mesh>("FloorMeshes/Tile_Small");
            case FloorType.Tile_Big:
                return Resources.Load<Mesh>("FloorMeshes/Tile_Big");
            case FloorType.Wooden:
                return Resources.Load<Mesh>("FloorMeshes/Wooden");
            default:
                return null;
        }
    }

    private void AdjustMaterialSlots(FloorType floorType)
    {
        Renderer renderer = GetComponent<Renderer>();

        if (renderer != null)
        {
            int desiredMaterialCount = 0;

            // Determine the desired number of materials based on the floor type
            switch (floorType)
            {
                case FloorType.Plain:
                    desiredMaterialCount = 1; // Only primary color
                    break;
                case FloorType.Tile_Big:
                case FloorType.Wooden:
                    desiredMaterialCount = 2; // Primary and grout
                    break;
                case FloorType.Tile_Small:
                    desiredMaterialCount = 3; // Primary, grout, and secondary
                    break;
                default:
                    Debug.LogError("Unexpected floor type: " + floorType);
                    return;
            }

            Material[] currentMaterials = renderer.sharedMaterials;
            if (currentMaterials.Length != desiredMaterialCount)
            {
                // Create a new array with the desired size and copy existing materials
                Material[] newMaterials = new Material[desiredMaterialCount];
                for (int i = 0; i < desiredMaterialCount && i < currentMaterials.Length; i++)
                {
                    newMaterials[i] = currentMaterials[i];
                }

                // Assign the new materials array to the renderer
                renderer.sharedMaterials = newMaterials;
            }
        }
        else
        {
            Debug.LogError("Renderer component is missing.");
        }
    }

    private void ApplyMaterialsAndMesh(Material floorPrimaryMaterial, Material floorSecondaryMaterial, Material floorGroutMaterial, Mesh floorMesh)
    {
        Renderer renderer = GetComponent<Renderer>();
        MeshFilter meshFilter = GetComponent<MeshFilter>();

        if (renderer != null && meshFilter != null)
        {
            if (floorMesh != null)
            {
                meshFilter.sharedMesh = floorMesh;
            }
            else
            {
                Debug.LogWarning("No mesh available for the selected floor type.");
            }

            Material[] materials = renderer.sharedMaterials;

            // Assign materials to the specific slots
            if (_floorType == FloorType.Wooden)
            {
                // Special case for Wooden: Grout in slot 0, Primary in slot 1
                if (materials.Length > 0)
                    materials[0] = floorGroutMaterial; // Grout color in slot 0
                if (materials.Length > 1)
                    materials[1] = floorPrimaryMaterial; // Primary color in slot 1
            }
            else
            {
                // Default case: Primary in slot 0, Grout in slot 1, Secondary in slot 2
                if (materials.Length > 0)
                    materials[0] = floorPrimaryMaterial; // Primary color always in slot 0
                if (materials.Length > 1)
                    materials[1] = floorGroutMaterial; // Grout color always in slot 1
                if (materials.Length > 2)
                    materials[2] = floorSecondaryMaterial; // Secondary color always in slot 2
            }

            renderer.sharedMaterials = materials;
        }
        else
        {
            Debug.LogError("Renderer or MeshFilter component is missing.");
        }
    }
}
