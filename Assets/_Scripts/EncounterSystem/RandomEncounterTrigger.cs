using UnityEngine;

public class RandomEncounterTrigger : MonoBehaviour
{
    [SerializeField] private float minInterval = 10f;
    [SerializeField] private float maxInterval = 30f;
    [SerializeField] private int minEncounters = 3;

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
                if(_encounterManager.StartEncounter())
                    _encountersTriggered++;
            }
            ScheduleNextEncounter();
        }
    }

    private void ScheduleNextEncounter()
    {
        float currentTime = Time.time - _levelStartTime;
        float remainingTime = _encounterManager.maxTriggerTime - currentTime;
        int remainingEncounters = minEncounters - _encountersTriggered;

        if (remainingEncounters <= 0 || remainingTime <= 0)
        {
            // No more encounters need to be scheduled
            _nextEncounterTime = float.MaxValue;
        }
        else
        {
            float averageInterval = remainingTime / remainingEncounters;
            _nextEncounterTime = Time.time + Random.Range(minInterval, Mathf.Min(maxInterval, averageInterval));
        }
    }
}