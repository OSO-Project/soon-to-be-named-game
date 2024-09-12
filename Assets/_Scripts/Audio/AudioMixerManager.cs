using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class AudioMixerManager : MonoBehaviour
{
    [Header("AudioMixer References")]
    [SerializeField] private AudioMixer _audioMixer;

    public void SetMasterVolume(float volume)
    {
        //This math function makes the volume decrease linearly and not logarithmically
        _audioMixer.SetFloat("MasterVolume", Mathf.Log10(volume) * 20f);
        if(_audioMixer == null)
        {
            Debug.LogError("AudioMixer issue. Check if the AudioMixer is assigned in the inspector.");
        }
    }



    public void SetMusicVolume(float volume)
    {
        _audioMixer.SetFloat("MusicVolume", Mathf.Log10(volume) * 20f);
    }


    public void SetSFXVolume(float volume)
    {
        _audioMixer.SetFloat("SFXVolume", Mathf.Log10(volume) * 20f);
    }
}
