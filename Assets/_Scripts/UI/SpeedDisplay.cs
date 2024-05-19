using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SpeedDisplay : MonoBehaviour
{
    [SerializeField] private PlayerController playerController;
    [SerializeField] private TMP_Text speedText;

    private void Update()
    {
        float speed = playerController.GetCurrentSpeed();
        speedText.text = "Movement Speed: " + speed.ToString("F2");
    }
}
