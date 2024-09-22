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

    [Header("Encounter Notifications")]
    public GameObject notificationPanel;
    public Image EncounterIcon;
    public TMP_Text EncounterText;
    [Header("Score")]
    public TMP_Text Score;
    [Header("Current Room")]
    public TMP_Text CurrentRoom;


    [Header("Crosshairs")]
    public Image mainCrosshair;
    public Image handOpenCrosshair;
    public Image handClosedCrosshair;

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
        notificationPanel.SetActive(false);

        //HIDE THE HAND CROSSHAIRS
        handOpenCrosshair.gameObject.SetActive(false);
        handClosedCrosshair.gameObject.SetActive(false);

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

    public void ShowThisCrosshairAndHideOthers(Image crosshairToShow)
    {
        if (crosshairToShow == mainCrosshair)
        {
            mainCrosshair.gameObject.SetActive(true);
            handOpenCrosshair.gameObject.SetActive(false);
            handClosedCrosshair.gameObject.SetActive(false);
        }
        else if (crosshairToShow == handOpenCrosshair)
        {
            mainCrosshair.gameObject.SetActive(false);
            handOpenCrosshair.gameObject.SetActive(true);
            handClosedCrosshair.gameObject.SetActive(false);
        }
        else if (crosshairToShow == handClosedCrosshair)
        {
            mainCrosshair.gameObject.SetActive(false);
            handOpenCrosshair.gameObject.SetActive(false);
            handClosedCrosshair.gameObject.SetActive(true);
        }
    }
}
