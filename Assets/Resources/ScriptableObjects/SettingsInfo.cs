using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "SettingsInfo", menuName = "SettingsInfo")]
public class SettingsInfo : ScriptableObject
{
    [Header("Game Settings")]
    public float mouseSens;
}
