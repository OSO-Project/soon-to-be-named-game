using System;
using UnityEngine;
using Random = UnityEngine.Random;

public class Wall : MonoBehaviour
{
    [SerializeField][Tooltip("Defines a color of the ceiling strip.")]
    private Colors _ceilingStripColor;
    public Colors CeilingStripColor => _ceilingStripColor;

    [SerializeField][Tooltip("Defines a color of the front of the wall.")]
    private Colors _wallFrontColor;
    public Colors WallFrontColor => _wallFrontColor;

    [SerializeField][Tooltip("Defines a color of the back of the wall.")]
    private Colors _wallBackColor;
    public Colors WallBackColor => _wallBackColor;

    [SerializeField][Tooltip("Defines a color of the floor strip.")]
    private Colors _floorStripColor;
    public Colors FloorStripColor => _floorStripColor;  

    [SerializeField][Tooltip("Defines a color of the window sill (if applicable).")]
    private Colors _windowSillColor;
    public Colors WindowSillColor => _windowSillColor;  

    private Material _wallFrontMaterial;
    private Material _wallBackMaterial;
    private Material _ceilingStripMaterial;
    private Material _floorStripMaterial;
    private Material _windowSillMaterial;

    private void Awake()
    {
        UpdateMaterials();
    }

    private void OnValidate()
    {
        UpdateMaterials();
    }

    private void UpdateMaterials()
    {
        _ceilingStripMaterial = GetMaterial(_ceilingStripColor);
        _wallFrontMaterial = GetMaterial(_wallFrontColor);
        _wallBackMaterial = GetMaterial(_wallBackColor);
        _floorStripMaterial = GetMaterial(_floorStripColor);
        _windowSillMaterial = GetMaterial(_windowSillColor);

        ApplyMaterials(_ceilingStripMaterial, _wallFrontMaterial, _wallBackMaterial, _floorStripMaterial, _windowSillMaterial);
    }

    private Material GetMaterial(Colors Color)
    {
        switch(Color)
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

    private void ApplyMaterials(Material _ceilingStripMaterial, Material _wallFrontMaterial, Material _wallBackMaterial, Material _floorStripMaterial, Material _windowSillMaterial)
    {
        Renderer renderer = GetComponent<Renderer>();
        
        if (renderer != null)
        {
            Material[] materials = renderer.sharedMaterials;

            // Ensure the materials array is large enough
            if (materials.Length == 3)
            {
                materials[0] = _ceilingStripMaterial;
                materials[2] = _wallFrontMaterial;
                materials[1] = _wallBackMaterial;
                renderer.sharedMaterials = materials; // Assign new materials
            }
            else if (materials.Length == 4)
            {
                materials[1] = _ceilingStripMaterial;
                materials[3] = _wallFrontMaterial;
                materials[2] = _wallBackMaterial;
                materials[0] = _floorStripMaterial;
                renderer.sharedMaterials = materials; // Assign new materials
            }
            else if (materials.Length == 5)
            {
                materials[1] = _ceilingStripMaterial;
                materials[3] = _wallFrontMaterial;
                materials[2] = _wallBackMaterial;
                materials[0] = _floorStripMaterial;
                materials[4] = _windowSillMaterial;
                renderer.sharedMaterials = materials; // Assign new materials
            }
            else
            {
                Debug.LogWarning("Materials array is not large enough. Check the number of materials slots in the MeshRenderer.");
            }
        }
    }
}
