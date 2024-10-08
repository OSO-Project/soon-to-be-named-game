using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SmokeEncounter : Encounter
{
    private HashSet<ISmokable> smokablesInArea = new HashSet<ISmokable>();
    [SerializeField] private ParticleSystem smokeParticleSystem;

    private void Start()
    {
        GameEventManager.Instance.OnWindowOpen += StopEncounter;

    }
    public override bool CanStart()
    {
        foreach (var window in FindObjectsOfType<OpenCloseWindow>())
        {
            if (window.isOpen)
            {
                Debug.Log("window opened");
                return false;
            }
        }

        return true;
    }

    public override void StartEncounter()
    {
        smokeParticleSystem = GameObject.Find("SmokeSpawner").GetComponentInChildren<ParticleSystem>();
        StartSmoke();

        foreach (ISmokable smokable in smokablesInArea)
        {
            smokable.AddSmokeDirt();
        }
        StartCoroutine(SmokeRunning());
    }

    // used to check if there are any windows open that weren't caught by CanStart()
    private IEnumerator SmokeRunning()
    {
        foreach (var window in FindObjectsOfType<OpenCloseWindow>())
        {
            if (window.isOpen)
            {
                Debug.Log("window opened after");
                GameEventManager.Instance.OpenWindow();
            }
        }
        yield return null;
    }

    private void OnTriggerEnter(Collider other)
    {
        // Check if the object entering the trigger area implements ISmokable
        ISmokable smokable = other.GetComponent<ISmokable>();
        if (smokable != null)
        {
            smokablesInArea.Add(smokable);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        // Remove the object from the set when it exits the trigger area
        ISmokable smokable = other.GetComponent<ISmokable>();
        if (smokable != null)
        {
            smokablesInArea.Remove(smokable);
        }
    }

    public override void StopEncounter()
    {
        StopSmoke();       
        smokablesInArea.Clear();
        StopAllCoroutines();
        GameEventManager.Instance.EndEncounter();
        return;
    }

    private void StartSmoke()
    {
        smokeParticleSystem?.Play();
    }

    private void StopSmoke()
    {
        smokeParticleSystem?.Stop();
    }
}
