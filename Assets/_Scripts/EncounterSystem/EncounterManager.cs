using UnityEngine;
using System.Collections.Generic;
using System;

public class EncounterManager : MonoBehaviour
{
    [SerializeField] private List<Encounter> availableEncounters;
    private Encounter _currentEncounter;

    public float minTriggerTime;
    //public float maxTriggerTime;    
    [SerializeField] private float startDelay = 5f;

    private float _levelStartTime;
    private float _currentTime;

    void Start()
    {
        _levelStartTime = Time.time;
        GameEventManager.Instance.OnEncounterEnd += StopEncounter;
        
        
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
        return _currentEncounter == null && _currentTime >= minTriggerTime;
    }

    public bool StartEncounter()
    {
        if (_currentEncounter != null)
        {
            Debug.LogWarning("An encounter is already running!");
            return false;
        }
        if (CanStartEncounter())
        {
            
            // Choose a random encounter from the list
            Encounter encounter = availableEncounters[UnityEngine.Random.Range(0, availableEncounters.Count)];
            if (encounter.CanStart())
            {
                Debug.Log($"{encounter.gameObject.name} is started");
                _currentEncounter = encounter;

                // display notification
                UIManager.Instance.DisplayEncounterNotification(_currentEncounter.encounterIcon, _currentEncounter.encounterText, startDelay);
                _currentEncounter.StartEncounter();

                // get current room
                return true;
            }
        }
        return false;
    }

    public void StopEncounter()
    {
        Debug.Log("Current enc stop");
        if (_currentEncounter != null)
        {
            _currentEncounter = null;
        }
    }
}