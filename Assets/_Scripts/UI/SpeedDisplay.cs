using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SpeedDisplay : MonoBehaviour
{
    [SerializeField] private PlayerController playerController;

    private void Update()
    {
        float speed = playerController.GetCurrentSpeed();
        UIManager.Instance.SpeedText.text = "Movement Speed: " + speed.ToString("F2");
    }
}
