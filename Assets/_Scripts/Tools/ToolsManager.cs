using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class ToolsManager : MonoBehaviour
{
    public static ToolsManager Instance { get; private set; }
    public UnlockedToolsData PlayerData;
    public Tool _currentlyHeld;
    [SerializeField] private List<Tool> availableTools;

    private Dictionary<int, Tool> toolMap;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            InitializeToolMap();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void InitializeToolMap()
    {
        toolMap = new Dictionary<int, Tool>
        {
            { 1, availableTools[0] }, // Wipe
            { 2, availableTools[1] }, // Vacuum
            { 3, availableTools[2] }, // TrashBag
            { 4, availableTools[3] }  // MagicVacuumCleaner
        };
    }

    public void EquipTool(int toolIndex)
    {
        Debug.Log($"Attempting to equip tool with index {toolIndex}");

        if (_currentlyHeld != null)
        {
            if (_currentlyHeld != toolMap[toolIndex])
            {
                Debug.Log($"Unequipping {_currentlyHeld.Name} and equipping {toolMap[toolIndex].Name}");
                _currentlyHeld.UnEquip();
                _currentlyHeld = toolMap[toolIndex];
                _currentlyHeld.Equip();
            }
            else
            {
                Debug.Log($"Unequipping {_currentlyHeld.Name} and setting _currentlyHeld to null");
                _currentlyHeld.UnEquip();
                _currentlyHeld = null;
            }
        }
        else
        {
            Debug.Log($"Equipping {toolMap[toolIndex].Name}");
            _currentlyHeld = toolMap[toolIndex];
            _currentlyHeld.Equip();
        }
    }
}