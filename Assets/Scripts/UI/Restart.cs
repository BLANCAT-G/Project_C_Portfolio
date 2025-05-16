using UnityEngine;
using UnityEngine.SceneManagement;

public class Restart : MonoBehaviour
{
    public GameObject restartPanel;

    public void Awake()
    {
        restartPanel = gameObject;
        GameManager.Instance.RestartPanel = restartPanel;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Return))
        {
            Destroy(GameManager.Instance.gameObject);
            SceneManager.LoadScene("SampleScene");
        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            gameObject.SetActive(false);
            GameManager.Instance.Resume();
        }
    }

    public void OnRestartButtonClick()
    {
        Destroy(GameManager.Instance.gameObject);
        SceneManager.LoadScene("SampleScene");
    }

    public void OnCloseUIButtonClick()
    {
        gameObject.SetActive(false);
        GameManager.Instance.Resume();
    }
}
