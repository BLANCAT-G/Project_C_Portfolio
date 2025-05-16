using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class FilterData
{
    public bool used;

    public FilterData(bool u)
    {
        this.used = u;
    }
}

public enum DirectionType
{
    Left,Right,Up,Down
}

public enum FilterType
{
    Null,None,Mix,Substract,OneTimeNone,OneTimeMix,OneTimeSubstract
}

public class Filter : IObject
{
    public Vector3[] dir={
        Vector3.left,Vector3.right,Vector3.up,Vector3.down
    };

    public DirectionType directionType;
    public FilterType filterType;
    public bool isOneTime, used;
    public SpriteRenderer baseArrow, baseFront, baseBack;
    public SpriteRenderer Arrow, Front, Back;
    public GameObject PlusEff, MinusEff;
    
    private WaitForSeconds ws = new WaitForSeconds(0.04f);
    private Stack<FilterData> filterLog;
    private IEnumerator coroutine;
    public Sprite  defaultBase, defaultFront, defaultBack;
    public override void Awake()
    {
        filterLog = new Stack<FilterData>();
        used = false;
    }

    public override void Interaction()
    {
    }

    public Vector3 getDirection()
    {
        return dir[(int)directionType];
    }

    public void setDirection()
    {
        switch (directionType)
        {
            case DirectionType.Left:
                transform.Rotate(0,0,90);
                break;
            case DirectionType.Down:
                transform.localScale = new Vector3(1, -1, 1);
                break;
            case DirectionType.Right:
                transform.Rotate(0,0,90);
                transform.localScale = new Vector3(1, -1, 1);
                break;
            default:
                break;
        }
    }

    public override void ColorChange(ColorType cT)
    {
        baseArrow.color = cT.ToColor();
        baseFront.color = cT.ToColor();
        baseBack.color = cT.ToColor();
        if (filterType == FilterType.Mix)
        {
            PlusEff.SetActive(true);
            PlusEff.GetComponent<SpriteRenderer>().color = cT.ToColor();
        }
        else if (filterType == FilterType.Substract)
        {
            MinusEff.SetActive(true);
            MinusEff.GetComponent<SpriteRenderer>().color = cT.ToColor();
        }
        if (isOneTime)
        {
            coroutine = OneTimeFilterCoroutine();
            StartCoroutine(coroutine);
        }

        if (isAlpha)
        {
            Arrow.color = new Color(Arrow.color.r, Arrow.color.g, Arrow.color.b, 0.5f);
            Front.color = new Color(Front.color.r, Front.color.g, Front.color.b, 0.5f);
            Back.color = new Color(Back.color.r, Back.color.g, Back.color.b, 0.5f);
            baseArrow.color = new Color(baseArrow.color.r, baseArrow.color.g, baseArrow.color.b, 0.5f);
            baseFront.color = new Color(baseFront.color.r, baseFront.color.g, baseFront.color.b, 0.5f);
            baseBack.color = new Color(baseBack.color.r, baseBack.color.g, baseBack.color.b, 0.5f);
            
        }
    }

    public void Use()
    {
        used = true;
        
        StopCoroutine(coroutine);
        baseArrow.gameObject.SetActive(false);
        Arrow.gameObject.SetActive(false);
        baseFront.color = colorType.ToColor();
        baseBack.color = colorType.ToColor();
        GetComponent<Animator>().SetTrigger("isUsed");
        Back.gameObject.GetComponent<Animator>().SetTrigger("isUsed");
        Front.gameObject.GetComponent<Animator>().SetTrigger("isUsed");
        PlusEff.SetActive(false);
        MinusEff.SetActive(false);
    }

    IEnumerator OneTimeFilterCoroutine()
    {
        float r = colorType.ToColor().r;
        float g = colorType.ToColor().g;
        float b = colorType.ToColor().b;
        while (true)
        {
            for (int i = 0; i < 20; ++i)
            {
                baseArrow.color = new Color(r, g, b, 1f - 0.03f * i);
                baseBack.color = new Color(r, g, b, 1f - 0.03f * i);
                baseFront.color = new Color(r, g, b, 1f - 0.03f * i);
                yield return ws;
            }
            for (int i = 0; i < 20; ++i)
            {
                baseArrow.color = new Color(r, g, b,  0.4f+0.03f * i);
                baseBack.color = new Color(r, g, b,  0.4f+0.03f * i);
                baseFront.color = new Color(r, g, b,  0.4f+0.03f * i);
                yield return ws;
            }
        }
    }

    public override void SaveData()
    {
        FilterData filterData = new FilterData(used);
        filterLog.Push(filterData);
    }

    public override void Undo()
    {
        FilterData lastData = filterLog.Pop();
        if (isOneTime)
        {
            if (used && !lastData.used)
            {
                used = false;
                StartCoroutine(OneTimeFilterCoroutine());
                if (filterType == FilterType.Mix) PlusEff.SetActive(true);
                else if (filterType == FilterType.Substract) MinusEff.SetActive(true);
                Arrow.gameObject.SetActive(true);
                baseArrow.gameObject.SetActive(true);
                GetComponent<Animator>().SetTrigger("Undo");
                Back.gameObject.GetComponent<Animator>().SetTrigger("Undo");
                Front.gameObject.GetComponent<Animator>().SetTrigger("Undo");
                
            }
        }
    }
}
