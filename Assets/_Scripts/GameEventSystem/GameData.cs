using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameData
{
    private int _numberOfItemsUnlocked;
    private List<Unlockables> _unlockedItems;

    public GameData()
    {
        _numberOfItemsUnlocked = 0;
        _unlockedItems = new List<Unlockables>();
    }

    public void UnlockItem(Unlockables unlocked)
    {
        if(!_unlockedItems.Contains(unlocked))
        {
            _numberOfItemsUnlocked++;
            _unlockedItems.Add(unlocked);
        }
    }
}
