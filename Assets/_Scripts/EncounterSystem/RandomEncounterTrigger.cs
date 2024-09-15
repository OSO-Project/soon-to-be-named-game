using UnityEngine;

public class RandomEncounterTrigger : MonoBehaviour
{
    [SerializeField] private float minInterval = 10f;
    [SerializeField] private float maxInterval = 30f;
    [SerializeField] private int totalEncounters = 3;
    private int minEncounters = 3;
    private float encounterNumberMultiplier;

    private EncounterManager _encounterManager;
    private float _nextEncounterTime;
    private float _levelStartTime;
    private int _encountersTriggered;

    void Start()
    {
        _encounterManager = FindObjectOfType<EncounterManager>();
        _levelStartTime = Time.time;
        _encountersTriggered = 0;
        ScheduleNextEncounter();
    }

    void Update()
    {
        if (Time.time >= _nextEncounterTime)
        {
            if (_encounterManager.CanStartEncounter() && _encountersTriggered < minEncounters)
            {
                if (_encounterManager.StartEncounter())
                    _encountersTriggered++;
            }
            ScheduleNextEncounter();
        }
    }

    private void ScheduleNextEncounter()
    {
        float currentTime = Time.time - _levelStartTime;
        //float remainingTime = _encounterManager.maxTriggerTime - currentTime;
        int remainingEncounters = minEncounters - _encountersTriggered;
        if (remainingEncounters <= 0)
        {
            // No more encounters need to be scheduled
            Debug.Log("no more enc");
            _nextEncounterTime = float.MaxValue;
        }
        else
        {
            //float averageInterval = remainingTime / remainingEncounters;
            _nextEncounterTime = Time.time + Random.Range(minInterval, maxInterval);
        }
    }

    public void NextRoomEncounter()
    {
        IncreaseEncounterMultiplier();
        totalEncounters += (int) encounterNumberMultiplier;

        ResetEncounters();
    }

    private void ResetEncounters()
    {
        minEncounters = totalEncounters;
    }

    private void IncreaseEncounterMultiplier()
    {
        encounterNumberMultiplier += 0.1f;
    }
}