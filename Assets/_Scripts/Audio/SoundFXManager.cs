using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundFxManager : MonoBehaviour
{
   public static SoundFxManager instance;

    [SerializeField] private AudioSource soundFXObject;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
    }

    public void PlaySoundFXClip(AudioClip audioClip, Transform audioSpawnTransform, float volume)
    {
        AudioSource _audioSource = Instantiate(soundFXObject, audioSpawnTransform.position, Quaternion.identity);
        
        _audioSource.clip = audioClip;
        _audioSource.volume = volume;

        _audioSource.Play();

        float clipLength = audioClip.length;

        //Destroy the audio source after the clip has finished playing
        Destroy(_audioSource.gameObject, clipLength);
    }
}
