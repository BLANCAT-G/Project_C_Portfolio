using System.Diagnostics;
using UnityEngine;
using UnityEngine.UI;
public class SceneTransition : MonoBehaviour
{
    public Image theImage;
    public float transitionSpeed;
    [SerializeField]
    private bool isSelectStage;
    [SerializeField]
    private bool isInGame;
    public bool low2up;
    public bool up2low;
    public ColorType cType;
    private Material dynamicMaterial;
    private bool isActive = false;

    public string destination;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        theImage = GetComponent<Image>();
        dynamicMaterial = new Material(theImage.material);
        theImage.material = dynamicMaterial;
        
        //게임씬에서 로비로 나올 경우 매니저에서 정보가져와서 예외처리하기
    }

    // Update is called once per frame
    void Update()
    {        
        if (low2up)
        {
            theImage.material.SetFloat("_Progress", Mathf.MoveTowards(theImage.material.GetFloat("_Progress"),1.05f,transitionSpeed*Time.deltaTime));
            
            if(theImage.material.GetFloat("_Progress")>1f)
            {
                if(!isActive){
                    SceneController.Instance.LoadScene(destination);
                    isActive = true;
                }
               
            }
               
        }
        if(up2low)
        {
            theImage.material.SetFloat("_Progress", Mathf.MoveTowards(theImage.material.GetFloat("_Progress"), -1.05f, transitionSpeed * Time.deltaTime));
            if (theImage.material.GetFloat("_Progress") < -1f)
            {
                if(SceneController.Instance.isChange)
                {
                    if (!isActive)
                    {
                        SceneController.Instance.LoadScene(destination);
                        isActive = true;
                    }
                }
            }
        }


    }

    public void SetColor()
    {
        theImage.material.SetColor("_TransitionColor", cType.ToColor());
        SceneController.Instance.colorType = cType;
    }
    public void SetColor(Color c)
    {
        theImage.material.SetColor("_TransitionColor", c);
    }
    public void Execute()
    {
        low2up = true;
        up2low = false;
        theImage.material.SetFloat("_Progress", -1.05f);
    }
    public void ReverseExecute()
    {
        up2low = true;
        low2up = false;
        theImage.material.SetFloat("_Progress", 1.05f);
    }
}
