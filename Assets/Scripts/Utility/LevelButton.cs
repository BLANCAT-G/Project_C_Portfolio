using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelButton : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField]
    private string sceneID;
    public SceneTransition transition;
    void Start()
    {
        if(SceneController.Instance.isChange)
        {
            transition.cType= SceneController.Instance.colorType;
            transition.ReverseExecute();
            transition.SetColor();
            SceneController.Instance.isChange = false;
        }
    }
    public void Clicked()
    {
        SceneController.Instance.SetSceneID(sceneID);
        transition.Execute();
        transition.SetColor(new Color(53f / 255f, 53f / 255f, 53f / 255f));
    }
}
