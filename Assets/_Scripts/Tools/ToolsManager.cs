using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToolsManager : MonoBehaviour
{
    public static ToolsManager Instance { get; private set; }
    public UnlockedToolsData PlayerData;
    private ITool _currentlyHeld;
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
        }
    }

    void Update()
    {

    }

    void SwitchItem()
    {
        // On keydown scroll switch to another item in the list
    }
}