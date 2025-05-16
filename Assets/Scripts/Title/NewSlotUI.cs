using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NewSlotUI : MonoBehaviour
{
    public int num;
    public SelectSlot selectSlot;
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Return))
        {
            selectSlot.NewSlot(num);
        }
    }

    public void ConfirmNew()
    {
        selectSlot.NewSlot(num);
    }
    
}
