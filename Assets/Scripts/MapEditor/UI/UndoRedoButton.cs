using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UndoRedoButton : MonoBehaviour
{
    public MapEditor mapEditor;

    public void OnRedoButtonClick()
    {
        mapEditor.Redo();
    }

    public void OnUndoButtonClick()
    {
        mapEditor.Undo();
    }
}
