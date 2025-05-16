using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MapNode : MonoBehaviour
{
    public int id;
    public SpriteRenderer sr;
    public Sprite[] sprites;
    public MapNode leftNode,rightNode,upNode,downNode;
    public TextMeshProUGUI mapNumber;
    
    private string filename;
    private bool isLock;
    public bool isEmpty;
    public void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
        if (mapNumber)
        {
            //Vector2 ratio = new Vector2(1280f / Screen.width, 720f / Screen.height);
            //mapNumber.rectTransform.anchoredPosition = Camera.main.WorldToScreenPoint(transform.position)*ratio;
            mapNumber.gameObject.transform.position=gameObject.transform.position;
        }
        if(isLock)
        {
            sr.sprite = sprites[0];
        }
        else
        {
            sr.sprite = sprites[1];
        }
    }
    
    public MapNode MoveToRight()
    {
        if (rightNode == null || rightNode.isLock== true)
            return this;
        return rightNode;
    }
    public MapNode MoveToLeft()
    {
        if (leftNode == null || leftNode.isLock== true)
            return this;
        return leftNode;
    }
    public MapNode MoveToUp()
    {
        if (upNode == null || upNode.isLock== true)
            return this;
        return upNode;
    }
    public MapNode MoveToDown()
    {
        if (downNode == null || downNode.isLock== true)
            return this;
        return downNode;
    }

    public void Lock()
    {
        isLock = true;
        sr.sprite = sprites[0];
    }

    public void UnLock()
    {
        isLock = false;
        sr.sprite = sprites[1];
    }

    public void SetFileName(string filename)
    {
        this.filename = filename;
    }

    public string GetFileName()
    {
        return filename;
    }
   
}
