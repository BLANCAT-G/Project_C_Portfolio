using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeleteSlotUI : MonoBehaviour
{
    public int num;
    public SelectSlot selectSlot;
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Return))
        {
            selectSlot.DeleteSlot(num);
        }
    }

    public void ConfirmDelete()
    {
        selectSlot.DeleteSlot(num);
    }
}
