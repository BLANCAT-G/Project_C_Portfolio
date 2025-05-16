using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
public class TitleTimer : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI title;
    // Start is called before the first frame update
    void Start()
    {
        Invoke("CBTimer", 2f);
    }
    public void CBTimer()
    {
        SceneController.Instance.LoadScene("SampleScene");
    }
    
}
