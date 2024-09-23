using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SettingsManager : MonoBehaviour
{

    [SerializeField] private SettingsInfo settingsInfo;

    public void ChangeSensitivity(float newSens)
    {
        settingsInfo.mouseSens = newSens;
    }
}
