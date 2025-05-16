using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Localization.Components;

public class ReactTitle : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        GetComponent<LocalizeStringEvent>().StringReference.SetReference("New Table",SceneController.Instance.sceneID);
    }

   
}
