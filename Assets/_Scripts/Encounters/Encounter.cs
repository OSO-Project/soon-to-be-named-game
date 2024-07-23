/*using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Encounter : MonoBehaviour
{
    [SerializeField] protected float duration;
    private Coroutine encounter;

    private void Start()
    {
        encounter = null;
        GameEventManager.Instance.OnEncounterStart += EncounterStart;
        GameEventManager.Instance.OnEncounterEnd += EncounterEnd;
    }

    protected virtual void EncounterStart()
    {
        // start only if encounter is not alredy playing
        if(encounter == null)
        {
            encounter = StartCoroutine(EncounterPlay());
        }
        else
        {
            Debug.Log("Coroutine is already running!");
        }
    }

    protected virtual IEnumerator EncounterPlay()
    {
        yield return new WaitForSeconds(duration);
        GameEventManager.Instance.HandleEncounterEnd();
    }

    protected virtual void EncounterEnd()
    {
        encounter = null;
    }
}
*/

using UnityEngine;

public abstract class Encounter : MonoBehaviour
{
    public float duration;

    public abstract bool CanStart();
    public abstract void StartEncounter();
    public abstract void StopEncounter();
}