using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToolsManager : MonoBehaviour
{
    public static ToolsManager Instance { get; private set; }
    public UnlockedToolsData PlayerData;
    private string _currentlyHeld;
    public List<ITool> Tools = new List<ITool>();

    void Start()
    {
        // populate list of items from unlocked items data
    }

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Update()
    {
        if (Input.GetKeyDown("1") && PlayerData.wipeUnlocked && _currentlyHeld != "Wipe")
        {
            EquipWipe();
        }
    }

    void EquipWipe()
    {
        // pull strings from enums
        _currentlyHeld = "Wipe";
    }

    void SwitchItem()
    {
        // On keydown scroll switch to another item in the list
    }
}