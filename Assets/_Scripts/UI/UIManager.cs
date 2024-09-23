using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance { get; private set; }
    [Header("Progress Bar On Screen")]
    public Image ProgressBar; //Not required in Hold To Clean anymore. Progress Bar now on each object.
    public TMP_Text HintText;

    [Header("Speed Display")]
    public TMP_Text SpeedText;

    [Header("Game Timer")]
    public TMP_Text TimerText;

    [Header("Encounter Notifications")]
    public GameObject notificationPanel;
    public Image EncounterIcon;
    public TMP_Text EncounterText;
    [Header("Score")]
    public TMP_Text Score;
    [Header("Current Room")]
    public TMP_Text CurrentRoom;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);


        ProgressBar.fillAmount = 0f; //- Progress Bar now on each object adjusted separately
        HintText.gameObject.SetActive(false);
        notificationPanel.SetActive(false);
        /*EncounterIcon.gameObject.SetActive(false);
        EncounterText.gameObject.SetActive(false);*/

        Score.text = "0";
    }

    public void DisplayEncounterNotification(Sprite icon, string message, float delay)
    {
        EncounterIcon.sprite = icon;
        EncounterText.SetText(message);

        notificationPanel.SetActive(true);
        /*EncounterIcon.gameObject.SetActive(true);
        EncounterText.gameObject.SetActive(true);*/

        // Hide the notification after a delay
        Invoke(nameof(HideEncounterNotification), delay);
    }

    private void HideEncounterNotification()
    {
        notificationPanel.SetActive(false);
    }
}
