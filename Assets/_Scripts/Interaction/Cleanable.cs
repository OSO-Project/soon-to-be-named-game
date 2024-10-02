using System;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public enum CleanableType
{
    Wipable,
    Vacuumable,
}
public class Cleanable : MonoBehaviour, Interactable
{
    [SerializeField] private bool _isClean = false;
    public CleanableType cleanableType = CleanableType.Wipable;

    public bool IsClean
    {
        get => _isClean;
        set
        {
            if (value == true)
            {
                _isClean = value;
                _dustParticles.Stop(true, ParticleSystemStopBehavior.StopEmitting);
            }
            else
            {
                _isClean = value;
                _dustParticles.Play();
            }
        }
    }
    
    [Header("Setup Object")]
    [SerializeField] private Mesh _cleanableMesh = null; 
    private float _particleSurfaceArea = 0;
    private Canvas _progressBarCanvas = null;
    private ParticleSystem _dustParticles = null;

    private HighlightObject _highlight;
    private float _particleMultiplier = 2679.13f;

    [Header("CleaningMechanicVariables")]
    [SerializeField] protected float _timeToClean = 3f;
    public static Cleanable currentTarget = null; // public static because there is only one object at the time being looked at.
    
    private Wipe _wipe;

    public Action cleanSuccesfull;
    public Action cleanUnSuccesfull;

    public Image _progressBar;

    private ProgressBarFillMechanic progressBarFill;

    void Start()
    {
        // Check if the tag is already set to "Interactable"
        ToolsManager.toolSwap += OnToolSwap;
        _wipe = FindAnyObjectByType<Wipe>();

        progressBarFill = new ProgressBarFillMechanic(_progressBar, this, _timeToClean);
    }

    void OnValidate()
    {
        if (!gameObject.CompareTag("Interactable"))
        {
            // If not, set the tag to "Interactable"
            gameObject.tag = "Interactable";
            Debug.Log("Tag 'Interactable' was not set and has been assigned.");
        }
        // Attach HighlightObject component if it does not exist
        if (_highlight == null)
        {
            _highlight = gameObject.GetComponent<HighlightObject>();
            if (_highlight == null)
            {
                _highlight = gameObject.AddComponent<HighlightObject>();
                Debug.Log("HighlightObject component was missing and has been added.");
            }
        }
        if (_cleanableMesh != null)
        {
            _particleSurfaceArea = ComputeMeshSurfaceArea();

            if (_dustParticles == null)
            {
                CreateDust(_particleSurfaceArea);
            }
            else if (_dustParticles != null)
            {
                UpdateDust(_particleSurfaceArea);
            }
            
        }
        foreach (Transform child in transform)
        {
            if (child.GetComponent<ParticleSystem>() != null)
            {
                _dustParticles = child.GetComponent<ParticleSystem>();
            }
        }
        foreach (Transform child in transform)
        {
            if (child.GetComponent<Canvas>() != null)
            {
                _progressBarCanvas = child.GetComponent<Canvas>();
            }
        }

        if (_progressBarCanvas == null)
            {
                Debug.LogError("Attach prefab ProgressBarCanvas to the main object and then link RadialBar.Foreground to the ProgressBar public variable");
            }
        IsClean = _isClean;
    }

    private void OnDestroy()
    {
        InputManager.Instance.CleanAction.performed -= OnPressInteract;
        ToolsManager.toolSwap -= OnToolSwap;
    }

    private void OnToolSwap()
    {
        if (ReferenceEquals(InteractionManager.Instance.GetCurrentInteractable(), this))
        {
            if (ToolsManager.Instance._currentlyHeld is not Wipe)
            {
                if (IsClean) return;
                progressBarFill.StopAndResetProgress();
                ToggleObjectUI(false, cleanableType);
            }
            else
            {
                OnBeginLooking();
            }
        }
    }

    public void OnBeginLooking()
    {
        if (IsClean || ToolsManager.Instance._currentlyHeld is not Wipe && ToolsManager.Instance._currentlyHeld is not Vacuum) return;
        if (ToolsManager.Instance._currentlyHeld is Wipe)
        {
            ToggleObjectUI(true, cleanableType);
        }

        
    }

    public void OnFinishLooking()
    {
        if (IsClean) return;
        if (ToolsManager.Instance._currentlyHeld is Wipe)
        {
            ToggleObjectUI(false, cleanableType);
        }
    }

    public void ToggleObjectUI(bool state, CleanableType type)
    {
        if (ToolsManager.Instance._currentlyHeld is Wipe)
        {
            _highlight.SetIsHighlighted(state);
            if (state)
            {
                currentTarget = this;
            }
            else
            {
                currentTarget = null;
            }
            if (type == CleanableType.Wipable)
            {
                _progressBarCanvas.gameObject.SetActive(state);
                UIManager.Instance.HintText.gameObject.SetActive(state);
                _highlight.Highlight(Color.white);
            }
            else
            {
                UIManager.Instance.RequiresVacuum.gameObject.SetActive(state);
                _highlight.Highlight(Color.red);
            }
            if (!state)
            {
                progressBarFill.StopAndResetProgress();
            }
        }
        else if (ToolsManager.Instance._currentlyHeld is Vacuum)
        {
            _highlight.SetIsHighlighted(state);
            _progressBarCanvas.gameObject.SetActive(state);
            _highlight.Highlight(Color.white);
            if (!state)
            {
                progressBarFill.StopAndResetProgress();
            }
        }
    }

    public void OnPressInteract(InputAction.CallbackContext ctx)
    {
        // if (IsClean) return;

        // if (currentTarget == this)
        // {
        //     if (ctx.performed)
        //     {
        //         Debug.Log("cleaning started");
        //         progressBarFill.StartProgress(OnCleaningComplete);
        //     }
        // }

        // Cleaning process now triggered by the Wipe Tool.
    }

    public void HandleToolUse()
    {
        if (IsClean) return;

        if (currentTarget == this)
        {
            Debug.Log("cleaning started");
            progressBarFill.StartProgress(OnCleaningComplete);
        }
        if (ToolsManager.Instance._currentlyHeld is Vacuum)
        {
            progressBarFill.StartProgress(OnCleaningComplete);
        }
    }

    private void OnCleaningComplete()
    {
        if (_wipe.IsWipeSuccessful())
        {
            IsClean = true;
            ToggleObjectUI(false, cleanableType);
            cleanSuccesfull?.Invoke();
            Debug.Log("Wipe sucessful");
        }
        else
        {
            IsClean = false;
            cleanUnSuccesfull?.Invoke();
            progressBarFill.StopAndResetProgress();
            Debug.Log("Wipe UNsucessful");
        }
    }

    

    private float ComputeMeshSurfaceArea()
    {
        float area = 0f;

        Vector3[] vertices = _cleanableMesh.vertices;
        int[] triangles = _cleanableMesh.triangles;

        for (int i = 0; i <triangles.Length; i += 3)
        {
            Vector3 v0 = vertices[triangles[i]];
            Vector3 v1 = vertices[triangles[i+1]];
            Vector3 v2 = vertices[triangles[i+2]];

            area += TriangleArea(v0, v1, v2);
        }
        return area;
    }

    private float TriangleArea(Vector3 v0, Vector3 v1, Vector3 v2)
    {
        Vector3 edge1 = v1-v0;
        Vector3 edge2 = v2-v0;

        Vector3 crossProduct = Vector3.Cross(edge1,edge2);

        return crossProduct.magnitude * 0.5f;

    }

    void CreateDust(float _particleSurfaceArea)
    {
        // Check if any child already has a ParticleSystem component
        if (GetComponentInChildren<ParticleSystem>() == null)
        {
            // Create a new game object for the dust particles
            GameObject dust = new GameObject("Dust");

            // Add a ParticleSystem component to the newly created object
            _dustParticles = dust.AddComponent<ParticleSystem>();

            // Configure the ParticleSystem with the given surface area
            ConfigureParticleSystem(_dustParticles, _particleSurfaceArea);

            // Make the newly created object with the configured ParticleSystem a child of the current object
            _dustParticles.transform.SetParent(transform);

            // Set the child ParticleSystem's local position and rotation to match the parent object
            _dustParticles.transform.localPosition = Vector3.zero;
            _dustParticles.transform.localRotation = Quaternion.identity;

            IsClean = false;
        }
    }

    void UpdateDust(float _particleSurfaceArea)
    {
        _dustParticles.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
        // Configures Particle System.
        ConfigureParticleSystem(_dustParticles, _particleSurfaceArea);
    }

    private void ConfigureParticleSystem(ParticleSystem _dustParticles, float _particleSurfaceArea)
    {
        
        // Sets values for main module
        var main = _dustParticles.main;
        main.duration = 1.0f;
        main.loop = true;
        main.startDelay = 0.0f;
        main.startLifetime = 1.0f;
        main.startSpeed = new ParticleSystem.MinMaxCurve(0.01f, 0.001f);
        main.startColor = new Color(1f, 1f, 1f, 40f / 255f);
        main.startSize = new ParticleSystem.MinMaxCurve(0.01f, 0.02f);
        main.startRotation = new ParticleSystem.MinMaxCurve(0.0f, 360.0f);
        main.maxParticles = Mathf.RoundToInt(_particleMultiplier * _particleSurfaceArea);

        // Sets values for emission module
        var emmision = _dustParticles.emission;
        emmision.rateOverTime = Mathf.RoundToInt(_particleMultiplier * _particleSurfaceArea);

        // Sets values for Shape module
        var shape = _dustParticles.shape;
        shape.shapeType = ParticleSystemShapeType.Mesh;
        shape.meshShapeType = ParticleSystemMeshShapeType.Triangle;
        shape.mesh = _cleanableMesh;

        // Sets values for Color over Liftime module
        var colorOverLifetime = _dustParticles.colorOverLifetime;
        colorOverLifetime.enabled = true;
        Gradient gradient = new Gradient();
        gradient.mode = GradientMode.Blend;

        GradientColorKey[] colorKeys = new GradientColorKey[2];
        GradientAlphaKey[] alphaKeys = new GradientAlphaKey[4];

        colorKeys[0] = new GradientColorKey(Color.white, 0f);
        colorKeys[1] = new GradientColorKey(Color.white, 1f);

        alphaKeys[0] = new GradientAlphaKey(0f, 0f);
        alphaKeys[1] = new GradientAlphaKey(50f / 255f, 0.25f);
        alphaKeys[2] = new GradientAlphaKey(50f / 255f, 0.75f);
        alphaKeys[3] = new GradientAlphaKey(0f, 0f);

        gradient.SetKeys(colorKeys, alphaKeys);

        colorOverLifetime.color = gradient;

        // Assign default sprites for the renderer
        var renderer = _dustParticles.GetComponent<ParticleSystemRenderer>();
        renderer.material = new Material(Shader.Find("Sprites/Default"));
    }
}
