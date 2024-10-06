using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class PostFXManager : MonoBehaviour
{

    public static PostFXManager Instance;
    [SerializeField] private Camera _mainCamera;

    private Volume _postProcessVolume;
    private ChromaticAberration _chromaticAberration;
    private MotionBlur _motionBlur;

    void Awake()
    {

        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(this);
        }

        // Get the Post-process Volume
        _postProcessVolume = _mainCamera.GetComponent<Volume>();
        


        PerformNullChecks();
    }


    // Method to apply the dizzy effect
    public IEnumerator ApplyDizzyEffect(float duration)
    {
        _postProcessVolume.weight = 1f;


        // Enable Chromatic Aberration
        try
        {
            _postProcessVolume.profile.TryGet(out _chromaticAberration);
            _chromaticAberration.intensity.Override(1f); // Maximum aberration
            Debug.Log("Dizzy effect applied.");
            
        }
        catch (System.Exception e)
        {
            Debug.LogError("Chromatic Aberration not found in the PostProcessVolume.");
        }
        yield return new WaitForSeconds(duration);
        // Remove the effects after the duration
        _chromaticAberration.intensity.Override(0f);
    }

    private void PerformNullChecks()
    {
        if (_postProcessVolume == null)
        {
            Debug.LogError("PostProcessVolume component not found on this GameObject.");
            return; // Exit early if no PostProcessVolume found
        }

        // Check if the post-process profile is assigned
        if (_postProcessVolume.profile == null)
        {
            Debug.LogError("Post-process Profile is missing from the PostProcessVolume.");
            return; // Exit early if no profile found
        }
      
    }
}
