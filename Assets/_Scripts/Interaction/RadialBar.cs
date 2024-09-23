using UnityEngine.UI;
using UnityEngine;

public class RadialBar : MonoBehaviour
{
    [SerializeField] private Image _radialProgressBar;
    private Camera _mainCamera;
    private void Start()
    {
        gameObject.SetActive(false);
        _mainCamera = Camera.main;
    }
    public void UpdateRadialBar (float max, float current)
    {
        _radialProgressBar.fillAmount = current / max;
    }

    void Update() 
    {
        transform.LookAt(_mainCamera.transform); 
    }
}
