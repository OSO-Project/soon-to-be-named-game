using System;
using UnityEngine;
using Random = UnityEngine.Random;

public class Sock : ItemsToClean
{
    [Header("Sock Attributes")]
    [SerializeField]
    [Tooltip("Type defines the texture of the sock and assigns its color tag.")]
    private SockType _sockType;
    public SockType SockType => _sockType;

    private Material _sockMaterial;

    [SerializeField]
    private GameObject _rolledPairPrefab;
    [SerializeField]
    private GameObject _particleSystemPrefab;

    private bool _hasCollided = false;

    [Header("Sock Sound Effects")]
    //this can be later changed to AudioClip[] if we want to have multiple sounds for the same event
    [SerializeField] private AudioClip _sockCollisionSound;

    private void Awake()
    {
        AssignRandomSockType();
        _sockMaterial = GetSockMaterial(_sockType);
        ApplyMaterial(_sockMaterial);
    }

    private void OnValidate()
    {
        AssignRandomSockType();
        _sockMaterial = GetSockMaterial(_sockType);
        ApplyMaterial(_sockMaterial);
    }

    private Material GetSockMaterial(SockType sockType)
    {
        switch(sockType)
        {
            case SockType.Red:
                return Resources.Load<Material>("SockMaterials/Sock_RED");
            case SockType.Green:
                return Resources.Load<Material>("SockMaterials/Sock_GREEN");
            case SockType.Blue:
                return Resources.Load<Material>("SockMaterials/Sock_BLUE");
            default:
                return null;
        }
    }

    private void ApplyMaterial(Material material)
    {
        Renderer renderer = GetComponent<Renderer>();
        if (renderer != null && material != null)
        {
            renderer.sharedMaterial = material;
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        Sock collidedSock = collision.gameObject.GetComponent<Sock>();

        if (collidedSock != null && collidedSock.SockType == _sockType && !_hasCollided && !collidedSock._hasCollided)
        {
            _hasCollided = true; 

            Vector3 collisionPoint = collision.contacts[0].point;
            Quaternion rotation = Quaternion.Euler(-90, 0, 0); 
            SpawnRolledPair(collisionPoint, _sockMaterial, rotation);
            SpawnParticleSystem(collisionPoint, rotation);
            GameEventManager.Instance.CleanItem(GetDirtValue());
            // Destroy both socks
            Destroy(gameObject);
            Destroy(collision.gameObject);
        }
    }

    private void SpawnRolledPair(Vector3 position, Material sockMaterial, Quaternion rotation)
    {
        if (_rolledPairPrefab != null)
        {
            GameObject rolledPair = Instantiate(_rolledPairPrefab, position, rotation);
            ApplyMaterialToRolledPair(rolledPair, sockMaterial);
            SoundFxManager.instance.PlaySoundFXClip(_sockCollisionSound, transform, 1f);
        }
        else
        {
            Debug.Log("Assign Rolled Pair Prefab");
        }
    }

    private void ApplyMaterialToRolledPair(GameObject rolledPair, Material material)
    {
        Renderer renderer = rolledPair.GetComponent<Renderer>();
        if (renderer != null)
        {
            renderer.sharedMaterial = material;
        }
    }

    private void SpawnParticleSystem(Vector3 position, Quaternion rotation)
    {
        if (_particleSystemPrefab != null)
        {
            // Instantiate the Particle System prefab
            GameObject particleSystemInstance = Instantiate(_particleSystemPrefab, position, rotation);

            // Get the Particle System component
            ParticleSystem particleSystem = particleSystemInstance.GetComponent<ParticleSystem>();
            
            // Ensure the Particle System component is found
            if (particleSystem != null)
            {
                // Start emitting particles
                particleSystem.Play();             

                // Automatically destroy the Particle System GameObject after it finishes emitting
                Destroy(particleSystemInstance, particleSystem.main.duration + particleSystem.main.startLifetime.constantMax);
            }
            else
            {
                Debug.LogError("Particle System component not found in prefab.");
            }
        }
        else
        {
            Debug.Log("Assign Particle System Prefab");
        }
    }

    public void AssignRandomSockType()
    {
        // Get all values of the SockType enum
        SockType[] values = (SockType[])Enum.GetValues(typeof(SockType));

        // Generate a random index
        int randomIndex = Random.Range(0, values.Length);

        // Assign the random SockType to the sock
        _sockType = values[randomIndex];
    }

    public int getDirtValue()
    {
        return 1;
    }
}