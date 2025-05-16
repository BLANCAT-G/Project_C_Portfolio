using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using UnityEngine;

public class StampData
{
    public bool isActive;
    public int curIndex;

    public StampData(bool isActive, int curIndex)
    {
        this.isActive = isActive;
        this.curIndex = curIndex;
    }
}

public class Palette : IObject
{
    public ColorType[] KeyArr;
    public int curIndex=0;
    public Stack<StampData> paletteLog;
    public override void Awake()
    {
        paletteLog = new Stack<StampData>();
        spriter = GetComponent<SpriteRenderer>();
        ColorChange(colorType);
    }

    public override void Interaction()
    {
        
    }

    public void CheckOpen()
    {
        if (curIndex == KeyArr.Length)
        {
            Open(); 
            EffectManager.Instance.ExecuteEffect(EffectType.Interact, transform, colorType); SoundBox.instance.PlaySFX("Interact");
            SoundBox.instance.PlaySFX("Unlock");
        }
    }
    
    public bool CheckStamp(ColorType c)
    {
        if (curIndex < KeyArr.Length && KeyArr[curIndex] == c)
        {
            transform.Find("Key" + curIndex.ToString()).GetComponent<SpriteRenderer>().color=Color.white;
            curIndex++;
            EffectManager.Instance.ExecuteEffect(EffectType.Interact, transform, colorType); SoundBox.instance.PlaySFX("Interact");
            return true;
        }
        return false;
    }

    private void Open()
    {
        gameObject.SetActive(false);
    }
    
    public override void SaveData()
    {
        StampData newData = new StampData(gameObject.activeSelf, curIndex);
        paletteLog.Push(newData);
    }
    
    public override void Undo()
    {
        if (paletteLog.Count == 0)
        {
            Destroy(gameObject);
            return;
        }
        StampData lastData = paletteLog.Pop();
        gameObject.SetActive(lastData.isActive);
        this.curIndex = lastData.curIndex;
        if (curIndex < 3)
            transform.Find("Key" + curIndex.ToString()).GetComponent<SpriteRenderer>().color =
                KeyArr[curIndex].ToColor();
    }
    
    public override void ColorChange(ColorType cT)
    {
        
    }
}
