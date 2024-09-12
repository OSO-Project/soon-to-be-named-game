using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface  IDirtyObject
{
    int GetDirtValue();
    void Hide();
    void UnHide();
    bool GetHidden();
}
