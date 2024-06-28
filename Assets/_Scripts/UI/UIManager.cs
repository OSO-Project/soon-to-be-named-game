using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance { get; private set; }
    [Header("Hold To Clean")]
    public Image ProgressBar;
    public TMP_Text HintText;

    [Header("Speed Display")]
    public TMP_Text SpeedText;

    [Header("Game Timer")]
    public TMP_Text TimerText;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);


        ProgressBar.fillAmount = 0f;
        HintText.gameObject.SetActive(false);


    }
}
