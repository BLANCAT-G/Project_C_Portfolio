using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MyBox;
using UnityEngine.SceneManagement;
public class StageNode : SelectNode
{
    private float lastClickTime;
    private float doubleClickThreshold = 0.2f;
    // Start is called before the first frame update
    new void Start()
    {
        base.Start();
        if(isLock)
        {
            sr.sprite = sprites[0];
        }
        else
        {
            sr.sprite = sprites[1];
        }

    }

    // Update is called once per frame
    //void Update()
    //{
    //    if(Input.GetKeyDown(KeyCode.I))
    //    {
    //        if(myID==2)
    //        {
    //            if(isLock)
    //                BM_Unlock();
    //            else
    //                BM_lock();
    //        }
    //    }
    //    if (Input.GetKeyDown(KeyCode.O))
    //    {
    //        if (myID == 3)
    //        {
    //            if (isLock)
    //                BM_Unlock();
    //            else
    //                BM_lock();
    //
    //        }
    //    }
    //    if (Input.GetKeyDown(KeyCode.P))
    //    {
    //        if (myID == 4)
    //        {
    //            if (isLock)
    //                BM_Unlock();
    //            else
    //                BM_lock();
    //        }
    //    }
    //}
    [ButtonMethod]
    public void BM_lock()
    {
        isLock = true;
        sr.sprite = sprites[0];
    }
    [ButtonMethod]
    public void BM_Unlock()
    {
        isLock = false;
        sr.sprite= sprites[1];
    }
    public void OnMouseEnter()
    {
        MouseEnterEvent();
    }
    public void OnMouseExit()
    {
        MouseExitEvent();
    }
    public void OnMouseDown()
    {
        if (isLock)
            return;
        if (Time.time - lastClickTime < doubleClickThreshold)
        {
            //테스트코드
            StageMgr.Instance.SceneChange((int)myID);
        }
        else
        {
            StageMgr.Instance.FocusChange(this);
        }
            
        lastClickTime = Time.time;
    }
    public void MouseEnterEvent()
    {
        if (isLock)
            return;
        sr.sprite = sprites[2];
    }

    public void MouseExitEvent()
    {
        if (isLock)
            return;
        if (myID != StageMgr.Instance.currentStage)
            sr.sprite = sprites[1];
    }
    
}
