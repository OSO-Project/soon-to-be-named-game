using UnityEngine;
using System.Collections.Generic;

public class EncounterManager : MonoBehaviour
{
    [SerializeField] private List<Encounter> availableEncounters;
    private Encounter _currentEncounter;

    public float minTriggerTime;
    public float maxTriggerTime;

    private float _levelStartTime;
    private float _currentTime;

    void Start()
    {
        _levelStartTime = Time.time;
    }

    void Update()
    {
        _currentTime = Time.time - _levelStartTime;
    }

    public void AddEncounter(Encounter encounter)
    {
        if (!availableEncounters.Contains(encounter))
        {
            availableEncounters.Add(encounter);
        }
    }

    public bool CanStartEncounter()
    {
        _currentTime = Time.time - _levelStartTime;
        return _currentEncounter == null && _currentTime >= minTriggerTime && _currentTime <= maxTriggerTime;
    }

    public bool StartEncounter()
    {
        if (_currentEncounter != null)
        {
            Debug.LogWarning("An encounter is already running!");
            return false;
        }
        Debug.Log($" is started");
        if (CanStartEncounter())
        {
            
            // Choose a random encounter from the list
            Encounter encounter = availableEncounters[Random.Range(0, availableEncounters.Count)];
            if (encounter.CanStart())
            {
                Debug.Log($"{encounter.gameObject.name} is started");
                _currentEncounter = encounter;
                _currentEncounter.StartEncounter();
                return true;
            }
        }
        return false;
    }

    public void StopEncounter()
    {
        if (_currentEncounter != null)
        {
            _currentEncounter.StopEncounter();
            _currentEncounter = null;
        }
    }
}