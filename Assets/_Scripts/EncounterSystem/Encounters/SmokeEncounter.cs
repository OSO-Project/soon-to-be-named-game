using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SmokeEncounter : Encounter
{
    private HashSet<ISmokable> smokablesInArea = new HashSet<ISmokable>();
    [SerializeField] private ParticleSystem smokeParticleSystem;
    [SerializeField] private float particleLifeTimeWindowUp = 10f;
    [SerializeField] private float particleLifeTimeWindowSide = 5f;
    private float minParticleLifetime = 1f;
    [SerializeField] private float baseMaxTime = 60f;
    private List<OpenCloseWindow> windows;
    private GameObject currentRoom;

    private void Start()
    {
        GameEventManager.Instance.OnWindowOpen += StopEncounter;
        GameManager.Instance.OnNextRoom += NewCurrentRoom;

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
        var main = smokeParticleSystem.main;
        main.startLifetime = baseMaxTime;
        StartSmoke();
        windows = currentRoom.GetComponentsInChildren<OpenCloseWindow>().ToList();
        Debug.Log($"windows: {windows.Count}");

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
                //GameEventManager.Instance.OpenWindow(window.GetIfUp());
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
        GameEventManager.Instance.EndEncounter();
        StopAllCoroutines();
    }

    private void StartSmoke()
    {
        smokeParticleSystem?.Play();
    }

    private void StopSmoke()
    {
        UpdateParticleLifetimeBasedOnWindows();
        /*if (isWindowUp)
        {
            main.startLifetime = particleLifeTimeWindowUp;
        }
        else
        {
            main.startLifetime = particleLifeTimeWindowSide;
        }*/
        smokeParticleSystem?.Stop();
    }

    public override void StopEncounter()
    {
        
    }

    private void NewCurrentRoom(GameObject cr)
    {
        currentRoom = cr;
    }

    private void UpdateParticleLifetimeBasedOnWindows()
    {
        int totalWindows = windows.Count;
        float totalOpenness = 0f;

        // Sum the openness values of each window
        foreach (var window in windows)
        {
            totalOpenness += window.GetOpenness();  // Assuming GetOpenness() returns 0 (closed), 0.5 (half-open), or 1.0 (fully open)
        }

        // Calculate the new particle lifetime based on total openness and total windows
        float newLifetime = baseMaxTime - ((totalOpenness / totalWindows) * baseMaxTime);

        // Ensure lifetime doesn't go below a minimum threshold
        newLifetime = Mathf.Max(newLifetime, minParticleLifetime);

        var main = smokeParticleSystem.main;
        main.startLifetime = newLifetime;

        // Get current particles and update their remaining lifetime
        ParticleSystem.Particle[] particles = new ParticleSystem.Particle[smokeParticleSystem.particleCount];
        int numParticlesAlive = smokeParticleSystem.GetParticles(particles);

        for (int i = 0; i < numParticlesAlive; i++)
        {
            // Scale down the remaining lifetime of existing particles to match the new lifetime
            float remainingLifetime = particles[i].remainingLifetime;
            float totalLifetime = particles[i].startLifetime;

            // Adjust the current lifetime based on the newLifetime (proportionally)
            float newRemainingLifetime = (remainingLifetime / totalLifetime) * newLifetime;
            particles[i].remainingLifetime = newRemainingLifetime;
        }

        // Apply the changes back to the particle system
        smokeParticleSystem.SetParticles(particles, numParticlesAlive);
        Debug.Log($"Updated smoke lifetime to: {newLifetime} based on {totalOpenness}/{totalWindows} openness.");
        //return newLifetime;        
    }

}
