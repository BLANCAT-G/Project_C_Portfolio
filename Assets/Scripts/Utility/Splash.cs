using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public class Splash : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        Invoke("SceneChange", 1.5f);
    }

    public void SceneChange()
    {
        SceneManager.LoadScene("Title");
    }
}
