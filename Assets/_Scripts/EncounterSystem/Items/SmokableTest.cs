using UnityEngine;

public class SmokableTest : MonoBehaviour, ISmokable
{
    public GameObject dust;

    public void AddSmokeDirt()
    {
        GameObject childObject = Instantiate(dust);
        childObject.transform.SetParent(transform);

        gameObject.AddComponent<HoldToClean>();
        gameObject.AddComponent<HighlightObject>();

        childObject.transform.localPosition = Vector3.zero;
        childObject.transform.localRotation = Quaternion.identity;
        childObject.transform.localScale = Vector3.one;

    }

    /*private void Start()
    {
        GameObject childObject = Instantiate(dust);
        childObject.transform.SetParent(transform);

        gameObject.AddComponent<HoldToClean>();
        gameObject.AddComponent<HighlightObject>();

        childObject.transform.localPosition = Vector3.zero;
        childObject.transform.localRotation = Quaternion.identity;
        childObject.transform.localScale = Vector3.one;
    }*/
}
