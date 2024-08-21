using System.Collections;
using System.Linq;
using UnityEngine;

public class EarthquakeEncounter : Encounter
{
    [SerializeField] private float forceMagnitude = 10f;
    

    public override bool CanStart()
    {
        // Add condition to check if the encounter can start
        return true;
    }

    public override void StartEncounter()
    {
        IPhysics[] physicsObjects = FindObjectsOfType<MonoBehaviour>().OfType<IPhysics>().ToArray();
        foreach (IPhysics obj in physicsObjects)
        {
            obj.Rigidbody.isKinematic = false;
            Vector3 randomForce = Random.insideUnitSphere * forceMagnitude;
            obj.Rigidbody.AddForce(randomForce, ForceMode.Impulse);
        }

        IShake[] shakeObjects = FindObjectsOfType<MonoBehaviour>().OfType<IShake>().ToArray();
        foreach (IShake obj in shakeObjects)
        {
            obj.Shake(duration);
        }

        // Start a coroutine to stop the encounter after the duration
        StartCoroutine(StopAfterDuration());
    }

    private IEnumerator StopAfterDuration()
    {
        yield return new WaitForSeconds(duration);
        GameEventManager.Instance.EndEncounter();
    }

    public override void StopEncounter()
    {
        IPhysics[] physicsObjects = FindObjectsOfType<MonoBehaviour>().OfType<IPhysics>().ToArray();
        foreach (IPhysics obj in physicsObjects)
        {
            obj.Rigidbody.isKinematic = true;
        }

        /*IShake[] shakeObjects = FindObjectsOfType<MonoBehaviour>().OfType<IShake>().ToArray();
        foreach (IShake obj in shakeObjects)
        {
            // Stop shaking logic here if needed (prolly yes)
        }*/
    }
}